using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inec.StateMachine.Util;
using System.Threading.Tasks;
using Core.ErrorManagment;
using Core.Register;
using Inec.StateMachine.Storage;
using System.Data;
using Core.Register.QuerySubsystem;
using Inec.StateMachine.Loging;
using Core.Register.RegisterEntities.CoreEvents;
using System.Web;

namespace Inec.StateMachine
{
    public class StateMachineProcessor
    {
        #region поля
        /// <summary>
        /// уникальный идентификатор цикла работы машины состояний
        /// </summary>
        [ThreadStatic]
        public static Guid UniqueId;
        /// <summary>
        /// идентификатор реестра
        /// </summary>
        [ThreadStatic]
        public static int RegisterId;
        /// <summary>
        /// идентификатор объекта
        /// </summary>
        [ThreadStatic]
        public static int ObjectId;
        #endregion
        #region свойства
        /// <summary>
        /// идентификатор машины состояний
        /// </summary>
        public int StateMachineId { get; private set; }
        /// <summary>
        /// список критериев для машины состояний
        /// </summary>
        public Lazy<StateMachine.Storage.CriteriaItemList> CriteriaItems { get; private set; }
        /// <summary>
        /// смежные реестры
        /// </summary>
        public IEnumerable<int> Registers { get; private set; }
        /// <summary>
        /// основной реестр
        /// </summary>
        public Lazy<int> MainRegisterId { get; private set; }
        
        
        /// <summary>
        /// переделать, могут все равно пересекаться
        /// </summary>
        public List<int> GetLinksId
        {
            get
            {
                var res = CriteriaItems.Value
                    .SelectMany(x => x.Query.RegisterLinks)
                    .Distinct()
                    .ToList();
                return res;
            }
        }

        #endregion
        #region конструкторы
        public StateMachineProcessor(int stateMachineId)
        {
            this.StateMachineId = stateMachineId;
            Init();
        }
        #endregion
        /*
          <int>58</int>
    <int>73</int>
    <int>30</int>
    <int>80</int>
         */
        #region методы
        /// <summary>
        /// генерируем запрос, который по ид смежного реестра найдет ид основного объекта
        /// </summary>
        /// <param name="mainRegisterId">идентификатор основного реестра</param>
        /// <param name="contiguousRegisterId">идентификатор смежного реестра</param>
        /// <param name="contiguousEmpId">ид смежного реестра</param>
        /// <returns>запрос на получение ид основного реестра</returns>
        public QSQuery GetContiguousQuery(int mainRegisterId, int contiguousRegisterId, int contiguousEmpId)
        {
            var query = new QSQuery(mainRegisterId);
            var attr = QSQueryUtil.GetRegisterIdAttribute(contiguousRegisterId);
            query.Condition = new QSConditionSimple(new QSColumnSimple(attr), QSConditionType.Equal, contiguousEmpId);
            query.RegisterLinks = GetLinksId;
            query.JoinType = QSJoinType.Left;
            return query;
        }
        public void Process(object obj, CoreEventArgs args)
        {
            Process(args.RegisterObject);
        }
        public static void Process(int reestrId, int objectId)
        {
            var context = HttpContext.Current;
            //var ms = StateMachineItemList.InSaveList.Select(x => (int)x.Id).OrderBy(x => x)
            //    .Select(x => new StateMachineProcessor(x))
            //    .Select(x => x.ProcessOnline(reestrId,objectId,context))
            //    .Select(x => new Action(() => x(reestrId, objectId)))
            //    .ToArray();
            foreach (var smp in StateMachineItemList.InSaveList.Select(x => (int)x.Id).OrderBy(x => x).Select(a => new StateMachineProcessor(a)))
            {
                smp.ProcessOnline(reestrId, objectId, context);
            }

            //  Parallel.Invoke(ms);
          //  RegisterObject regObj = new RegisterObject(reestrId, objectId);
          //  Process(regObj);
        }
        public static void ProcessOther(int reestrId, int objectId)
        {
            var context = HttpContext.Current;
            foreach (var smp in StateMachineItemList.OutSaveMethodList.Select(x => (int)x.Id).OrderBy(x => x).Select(a => new StateMachineProcessor(a)))
            {
                smp.ProcessOnline(reestrId, objectId, context);
            }
        }

        public void ProcessOnline(int reestrId, int objectId, HttpContext context=null)
        {
            var regObj = new RegisterObject(reestrId, objectId);
            Process(regObj, context);
        }
        public void Process(RegisterObject ro, HttpContext context=null)
        {
            var currentContext = context ?? HttpContext.Current;
            var uniqueId = Guid.NewGuid();
            Action process = () =>
            {
                if (currentContext != null)
                {
                    HttpContext.Current = currentContext;
                }
                UniqueId = uniqueId;
                var regId = ro.RegisterID;
                var objId = ro.ObjectID;
                RegisterId = regId;
                ObjectId = objId;                
                if (regId == MainRegisterId.Value)
                {
                    var le = new LogEntity("Запуск", LogEntityType.INFO, regId, objId, StateMachineId);
                    le.WriteToDB();
                    new LogEntity("Начало обработки основного реестра -" + regId, LogEntityType.RESULT, regId, objId, StateMachineId).WriteToDB();
                    var run = new OneRunner(StateMachineId, objId);
                    run.Run();
                }
                if (Registers.Contains(regId))
                {
                    var le = new LogEntity("Запуск", LogEntityType.INFO, regId, objId, StateMachineId);
                    le.WriteToDB();
                    new LogEntity("Обработка смежного реестра - " + regId + " для основного " + MainRegisterId, LogEntityType.RESULT, regId, null, StateMachineId).WriteToDB();
                    var query = GetContiguousQuery(MainRegisterId.Value, regId, objId);
                    var xml = QSQueryUtil.Serialize(query);
                    var sql = query.GetSql();
                    new LogEntity("Запрос на поиск объектов основного реестра - " + MainRegisterId + " по смежному - " + regId + " для obj_id = " + objId + Environment.NewLine + xml, 
                        LogEntityType.INFO, regId, objId, StateMachineId).WriteToDB();
                    new LogEntity("Запрос на поиск объектов основного реестра - " + MainRegisterId + " по смежному - " + regId + " для obj_id = " + objId + Environment.NewLine + sql,
                       LogEntityType.INFO, regId, objId, StateMachineId).WriteToDB();
                    var dt = query.ExecuteQuery();
                    
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var ids = dt.Rows.Cast<DataRow>().Select(r => Convert.ToInt32(r["Id"]));
                        foreach (var id in ids)
                        {
                            new LogEntity("Начало обработки смежного реестра ", LogEntityType.INFO, regId, id, StateMachineId).WriteToDB();
                            var run = new OneRunner(StateMachineId, id);
                            run.Run();
                        }
                    }
                    else
                    {
                        new LogEntity("Запрос не дал результатов- xml\r\n" + xml, LogEntityType.INFO, regId, objId, StateMachineId).WriteToDB();
                        new LogEntity("Запрос не дал результатов - sql\r\n" + sql, LogEntityType.INFO, regId, objId, StateMachineId).WriteToDB();
                    }
                }
            };
            Task.Factory.StartNew(process);
        }
        void Init()
        {
            try
            {
                CriteriaItems = new Lazy<CriteriaItemList>(() => Criteria.GetCriteria(StateMachineId),
                    System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
                Registers = QSQueryUtil.GetRegisters(CriteriaItems.Value.Select(x => x.Query));
                
                MainRegisterId = new Lazy<int>(() => GetRegisterFromStateMachineId(StateMachineId));
                Registers = Registers.Where(x => x != MainRegisterId.Value);
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Не удалось инициализировать StateMachineProcessor", exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
                throw sexc;
            }
        }
        /// <summary>
        /// получаем идентификатор основного реестра по идентификатору машины состояний
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        public static int GetRegisterFromStateMachineId(int stateMachineId)
        {
            var res = ObjectModel.OMStateMachine.Where(x => x.Id == stateMachineId)
                .Select(x => x.RegisterId)
                .Execute()
                .Select(x => x.RegisterId)
                .FirstOrDefault();
            if (res == 0)
                throw new StateMachineException("Не удалось найти основной реестр для машины состояний с ид = " + stateMachineId);
            return (int)res;
        }
        #endregion

    }
}
