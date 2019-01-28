using System;
using System.Collections.Generic;
using System.Linq;
using Core.Register.QuerySubsystem;
using Inec.StateMachine.Storage;
using System.Data;
using Core.Td;
using System.Threading.Tasks;
using Core.ErrorManagment;
using Inec.StateMachine.Loging;
using ObjectModel;
using ObjectModel.Core.Register;

namespace Inec.StateMachine
{
    public interface IRunner
    {
        int StateMachineId { get; set; }
        void Run();
    }
    public abstract class ARunner: IRunner
    {
        static readonly object Locker = new object();
        protected HashSet<int> HashSet = new HashSet<int>();
        public abstract void Run();
        public int StateMachineId { get; set; }
        public int RegisterId
        {
            get
            {
                return (int)StateMachineHelper.GetRegisterIdFromStateMachine(StateMachineId);
            }
        }

        public StorageType StorageType
        {
            get
            {
                return StateMachineHelper.GetStorageType(StateMachineId);
            }
        }
        /// <summary>
        /// дата С
        /// </summary>
        public DateTime S { get; protected set; }

        /// <summary>
        /// дата ПО
        /// </summary>
        public DateTime Po { get; protected set; }
        public int TdInstanceId { get; protected set; }
        protected virtual void SetNullStatus(int objId)
        {
            /*
             s.StatusCode = status.Id;
                s.StatusValue = status.Value;
                s.RegisterId = RegisterId;
                s.ObjectId = id;
                s.Save(TDInstanceId, S_, PO_);
             */
           
            var itemId = StateMachineHelper.GetDefaultStatus(StateMachineId);
            var tmp = CoreRefItem.GetRefItem(itemId);

            StorageBase.Create(StateMachineId, tmp.Item1, tmp.Item2, objId)
                .Save();
            //var s = ObjectModel.OMStateMachineStatuses.CreateEmpty();



            //s.Statuscode = tmp.Item1;
            //s.Statusvalue = tmp.Item2;
            //s.Registerid = RegisterId;
            //s.Objectid = objId;
            //s.Save(TDInstanceId, S_, PO_);
        }
        protected virtual void RunInternal(IEnumerable<CriteriaItem> list)
        {
            foreach (var l in list)
            {
                RunOneCriteria(l);
            }
        }
        /// <summary>
        /// выполняем работу по одному критерию
        /// </summary>
        /// <param name="l">критерий</param>
        protected virtual void RunOneCriteria(CriteriaItem l)
        {
            try
            {
                int packageIndex = 0;
                while (true)
                {
                    var query = l.Query;
                    query.PackageIndex = packageIndex;
                    var ids = query.ExecuteQuery().Rows.Cast<DataRow>().Select(x => Convert.ToInt32(x["ID"])).Distinct().ToList();
                    if (ids.Count < 1)
                        break;
                    packageIndex++;
                    foreach (var id in ids)
                    {
                        RunOneRow(id, l.StatusId);
                    }
                }
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Ошибка при обработки критерия", exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
            }
        }
        protected virtual void RunInternal(CriteriaItemList list)
        {
            foreach (var l in list)
            {
                RunOneCriteria(l);
            }
        }
        /// <summary>
        /// проверяем, была ли загрузка в текущее время
        /// для объекта удовлетворяющего критерию и имеющему такой же статус, который хотим установить
        /// ничего не делаем, но заносим в список, что бы в случае чего пропустить в дальнейшем
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        protected bool CheckExists(int objectId)
        {
            lock (Locker)
            {
                return HashSet.Contains(objectId);
            }
        }
        /// <summary>
        /// проверяем, нужно ли менять статус
        /// если текущий и присваемовый равны, просто заносим идентификатор в список обработанных
        /// что бы не обрабатывать в дальнейшем
        /// </summary>
        /// <param name="objectId">идентификатор объекта</param>
        /// <param name="currentStatusId">текущий статус</param>
        /// <param name="nextStatus">новый статус</param>
        /// <returns>нужно ли менять статус</returns>
        protected bool CheckUpdateStatus(int objectId, int currentStatusId, int nextStatus)
        {
            lock (Locker)
            {
                HashSet.Add(objectId);
            }
            return currentStatusId == nextStatus;//статус не должен поменяться
        }

        /// <summary>
        /// вставляем одну строку
        /// </summary>
        /// <param name="id">идентификатор объекта реестра</param>
        /// <param name="statusId"></param>
        protected virtual void RunOneRow(int id, int statusId)
        {
            try
            {
                var obj = Criteria.FindObjectCurrentTimeInsert(id, S);
                if (obj.HasValue)//вставлен в текущее время и удовлетворяет более приоритетному критерию
                    return;
                var obj1 = FindObject(RegisterId, id);// Criteria.FindObject(id, RegisterId);
                var s = OMStateMachineStatuses.CreateEmpty();                    
                var status = Statuse.GetStatuses(StateMachineId).FirstOrDefault(x => x.Id == statusId);
                if (status == null)
                {
                    throw new StateMachineException("Не удалось найти статус с Id=" + statusId);
                }
                if (obj1 != null)
                {
                    s.Id = obj1.Item3;
                    var currentStatus = StateMachineTransitions.GetCurrentStatus(RegisterId, obj1.Item1);
                    if (currentStatus.HasValue)
                    {
                        var flag = StateMachineTransitions.IsItPossibleToTransition(StateMachineId, currentStatus.Value, status.Id);
                        if (!flag)
                        {
                            return;//Переход не возможен, выходим
                        }
                        if (CheckExists(obj1.Item1))
                        {
                            return;
                        }
                        if (CheckUpdateStatus(obj1.Item1, obj1.Item2, status.Id))
                        {
                            return;
                        }
                    }
                }
                s.Statuscode = status.Id;
                s.Statusvalue = status.Value;
                s.Registerid = RegisterId;
                s.Objectid = id;
                s.Save(TdInstanceId, S, Po);
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Ошибка при сохранении статуса объекта", exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
            }
        }
        /// <summary>
        /// поиск объекта в зависимости от 
        /// </summary>
        /// <param name="registerId"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        protected virtual Tuple<int, int, int> FindObject(int registerId, int objectId)
        {
            switch (StorageType)
            {
                    case StorageType.INSEPARATEREGISTRE:
                    return Criteria.FindObject(objectId, registerId);
                    case StorageType.INREGISTRE:
                    return FindFromInRegister(objectId);

                default:
                    throw  new StateMachineException("Не удалось найти объект");
            }
        }

        Tuple<int, int, int> FindFromInRegister(int objectId)
        {
            var sm = OMStateMachine.Where(x => x.Id == StateMachineId).SelectAll().Execute().FirstOrDefault();
            if (sm == null)
                throw new StateMachineException("Не удалось найти статусную модель с идентификатором " + StateMachineId);
            var idCol = GetIdColumn((int)sm.RegisterId);
            var query = new QSQuery((int)sm.RegisterId);
            query.Columns = query.Columns ?? new List<QSColumn>();
            query.Columns.Add(new QSColumnSimple(Convert.ToInt32(sm.AttributeId), "statusCode", QSColumnSimpleType.Code));
            QSCondition con = new QSConditionSimple(new QSColumnSimple(idCol), QSConditionType.Equal, objectId);
            query.Condition = con;
            var data = query.ExecuteQuery();
            if (data != null && data.Rows.Count > 0)
            {
                var statusCode = Convert.ToInt32(data.Rows[0]["statusCode"]);
                return Tuple.Create(objectId, statusCode, objectId);
            }
            return null;
        }

        protected int GetIdColumn(int registerId)
        {
            var data =
                OMAttribute.Where(x => x.RegisterId == registerId && x.IsPrimaryKey)
                    .SelectAll()
                    .Execute()
                    .FirstOrDefault();
            if(data==null)
                throw new StateMachineException("Не удалось найти первичный клич реестра " + registerId);
            return (int)data.Id;
        }
    }
    public class AllRunner : ARunner
    {

        public AllRunner(int stateMachineId)
        {
            StateMachineId = stateMachineId;
            S = DateTime.Now;
            Po = default(DateTime);
            TdInstanceId = DocProcessor.CreateDefaultTD();
        }
        
        public override void Run()
        {
            try
            {
                var criList = Criteria.GetCriteria(StateMachineId).OrderedList;
                RunInternal(criList);
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Ошибка при обработке объекта", exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
            }
        }

    }
    /// <summary>
    /// запуск машины для одного объекта
    /// </summary>
    public class OneRunner : ARunner
    {
        /// <summary>
        /// идентификатор объекта
        /// </summary>
        public int ObjectId { get; private set; }
        public OneRunner(int stateMachineId, int objectId)
        {
            new LogEntity("Работа машины " + stateMachineId + " объект " + objectId, LogEntityType.RESULT, null, objectId, stateMachineId).WriteToDB();
            StateMachineId = stateMachineId;
            ObjectId = objectId;
            S = DateTime.Now;
            Po = default(DateTime);
            TdInstanceId = DocProcessor.CreateDefaultTD();
        }
        public override void Run()
        {
            try
            {
              //  new LogEntity("Начало обработки одиночной записи", LogEntityType.INFO, RegisterId, ObjectId, StateMachineId).WriteToDB();
                var criList = Criteria.GetCriteria(StateMachineId, ObjectId).OrderedList;
               // var str = "Первые 10 критериев\r\n" + criList.EnumerableOfTToString();
              //  new LogEntity(str, LogEntityType.INFO, RegisterId, ObjectId, StateMachineId).WriteToDB();
                RunInternal(criList);
            }
            catch (Exception exc)
            {
                new LogEntity("Фатальная ошибка \r\n" + exc, LogEntityType.ERROR, RegisterId, ObjectId, StateMachineId).WriteToDB();
                var sexc = new StateMachineException("Ошибка при обработке объекта", exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
            }
        }
        protected override void RunInternal(IEnumerable<CriteriaItem> list)
        {
            var lst = new CriteriaItemList(StateMachineId, list);
            RunInternal(lst);
        }
        protected override void RunInternal(CriteriaItemList list)
        {
            var isProcessing = false;
         //   ObjectModel.OMStateMachineStatuses s;
            foreach (var l in list)
            {
                var query = l.Query;
                var cond = query.Condition;
                cond = cond.And(Criteria.AddEmpIdCondition(StateMachineId, ObjectId));
                query.Condition = cond;
                var tmp = Util.QSQueryUtil.Serialize(query);
                //находим объект
                var id = query.ExecuteQuery().Rows.Cast<DataRow>().Select(x => Convert.ToInt32(x["ID"])).FirstOrDefault();
                new LogEntity("query для поиска \r\n" + tmp, LogEntityType.INFO, RegisterId, ObjectId, StateMachineId).WriteToDB();
                var sql = query.GetSql();
                new LogEntity("sql для поиска \r\n" + sql, LogEntityType.INFO, RegisterId, ObjectId, StateMachineId).WriteToDB();
                if (id == 0)
                {
                    new LogEntity("Объект не удовлетворяет критерию \r\n" + l.Description, LogEntityType.RESULT, RegisterId, ObjectId, StateMachineId).WriteToDB();
                    continue;//не удовлетворяет статусу - переходим к следующему                    
                }
                new LogEntity("Объект с id=" + id + " удовлетворяет критерию \r\n" + l.Description, LogEntityType.RESULT, RegisterId, ObjectId, StateMachineId).WriteToDB();
                var obj = FindObject(RegisterId, id);// Criteria.FindObject(id, RegisterId);//ищем исторический
            //    s = ObjectModel.OMStateMachineStatuses.CreateEmpty();
                if (obj != null)
                {
                    new LogEntity(string.Format("Статус уже существует, Statuscode-{0}, Objectid-{1}", obj.Item2, obj.Item1), LogEntityType.INFO, RegisterId, ObjectId, StateMachineId).WriteToDB();
                   // s.Id = obj.Item3;
                   // var currentStatus = StateMachineTransitions.GetCurrentStatus(RegisterId, ObjectId);
                    var currentStatus = obj.Item2;
                   // if (currentStatus.HasValue)
                    {
                        var flag = StateMachineTransitions.IsItPossibleToTransition(l.CriteriaId, currentStatus, l.StatusId);
                        if (!flag)
                        {
                            var m = "для объекта с id = " + ObjectId + " реестра - " + RegisterId + " не возможен переход из статуса - " + currentStatus + " в статус " + l.StatusId;
                            new LogEntity(m, LogEntityType.RESULT, RegisterId, ObjectId, StateMachineId).WriteToDB();
                            isProcessing = true;
                            continue;//Переход не возможен, выходим
                        }
                        //if (CheckExists(obj.Item1))
                        //{
                        //    return;
                        //}
                        if (CheckUpdateStatus(obj.Item1, obj.Item2, l.StatusId))
                        {
                            var m = "для объекта с id = " + ObjectId + " реестра - " + RegisterId + " статус не изменился";
                            new LogEntity(m, LogEntityType.RESULT, RegisterId, ObjectId, StateMachineId).WriteToDB();
                            return;
                        }
                    }
                }
                else
                {
                    const string m = "Исторический объект не найден";
                    new LogEntity(m, LogEntityType.INFO, RegisterId, ObjectId, StateMachineId).WriteToDB();
                }
                var status = Statuse.GetStatuses(StateMachineId).FirstOrDefault(x => x.Id == l.StatusId);
                if (status == null)
                {
                    throw new StateMachineException("Не удалось найти статус с Id=" + l.StatusId);
                }

                StorageBase.Create(StateMachineId, status.Id, status.Value, ObjectId)
                    .Save();
                //s.Statuscode = status.Id;
                //s.Statusvalue = status.Value;
                //s.Registerid = RegisterId;
                //s.Objectid = ObjectId;
                //s.Save(TDInstanceId, S_, PO_);
                isProcessing = true;//проставили значение
                var m1 = "<B style='color:Red;'>Присвоили статус " + status.Id + ":" + status.Value + " для объекта " + ObjectId + " реестра " + RegisterId+"</B>";
                new LogEntity(m1, LogEntityType.RESULT, RegisterId, ObjectId, StateMachineId).WriteToDB();
                break;
            }
            if (!isProcessing)//не удовлетворяет не одному критерию, ставим статус по умолчанию
            {
                SetNullStatus(ObjectId);
                var m1 = "Установили статус по умолчанию для объекта " + ObjectId + " реестра " + RegisterId;
                new LogEntity(m1, LogEntityType.RESULT,RegisterId, ObjectId, StateMachineId).WriteToDB();
            }

        }

    }
    
    /// <summary>
    /// не удовлетворяющие критерию
    /// </summary>
    public class DoNotMeetCriteria : IRunner
    {
        public int StateMachineId { get;  set; }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}

