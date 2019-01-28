using System;
using System.Linq;
using Core.Register.QuerySubsystem;
using Core.ErrorManagment;
using System.Threading.Tasks;
using ObjectModel;

namespace Inec.StateMachine.Storage
{
    public class Criteria 
    {
        /// <summary>
        /// получаем все критерии для одной машины состояний
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        public static CriteriaItemList GetCriteria(int stateMachineId, int? objectId = null)
        {
            var data = OMStateMachineCriteria.Where(c => c.StateMachineId == stateMachineId).Select(x => x.QsqueryXml).Select(x => x.Statusid)
                .Select(x=>x.CriteriaDescription);
            var tmp = data.Execute()
                .Select(x => new CriteriaItem((int)x.Id, 1, x.QsqueryXml, Convert.ToInt32(x.Statusid), x.CriteriaDescription));
            CriteriaItemList res = new CriteriaItemList(stateMachineId, tmp);
            return res;
        }
        /// <summary>
        /// по идентификатору машины состояний и идентификатору объекта формируем запрос типа
        /// emp_id=
        /// </summary>
        /// <param name="stateMachineId">идентификатор машины состояний</param>
        /// <param name="objectId">идентификатор объекта</param>
        /// <returns>дополнительное условие вида emp_id=nnnnn</returns>
        public static QSCondition AddEmpIdCondition(int stateMachineId, int objectId)
        {
            try
            {
                var regId = StateMachineHelper.GetRegisterIdFromStateMachine(stateMachineId);
                var tmp = (int)ObjectModel.Core.Register.OMAttribute.Where(x => x.RegisterId == regId && x.IsPrimaryKey)
                    .Select(x => x.Id)
                    .Execute()
                    .Select(x=>x.Id)
                    .FirstOrDefault();
                QSConditionSimple res = new QSConditionSimple(new QSColumnSimple(tmp), QSConditionType.Equal, objectId);
                return res;
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Ошибка при формировании дополнительного условия по emp_id", exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
                throw sexc;
            }
        }
        /// <summary>
        /// обновляем список критериев для машины состояний
        /// забота о том, что Order монотонно возрастает в рамках одного списка, на программисте
        /// </summary>
        /// <param name="list"></param>
        public static void UpdateCritetiaList(CriteriaItemList list)
        {
            CheckCriteriaList(list);
            OMStateMachineCriteria r = new OMStateMachineCriteria();
            throw new NotImplementedException();
        }
        static void CheckCriteriaList(CriteriaItemList list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            var flag = list.GroupBy(x => x).Any(x => x.Count() > 1);
            if (flag)
                throw new StateMachineException("Список содержит повторяющиеся поля Order!");

        }
        /// <summary>
        /// ищем объект, вставленные во время текущей обработки
        /// если находим, то для менее приоритетных критериев не вставляем
        /// </summary>
        /// <param name="objectId">идентификатор объекта</param>
        /// <param name="S_">дата С</param>
        /// <returns>null () или Id найденного объекта</returns>
        public static int? FindObjectCurrentTimeInsert(int objectId, DateTime S_)
        {
            var res = OMStateMachineStatuses.Where(x => x.Objectid == objectId);
            res.SetCondition(res.GetCondition().And(new QSConditionSimple(new QSColumnSpecial(QSColumnSpecialType.S, 703), QSConditionType.Equal, S_)));
            var tmp = res.Execute().Select(x => x.Id).FirstOrDefault();
            if (tmp > 0)
                return (int)tmp;
            return null;
        }
        /// <summary>
        /// поиск существующей записи для историчности
        /// </summary>
        /// <param name="objectId">идентификатор объекта</param>
        /// <returns>(идентификатор объекта, идентификатор статуса, удентификатор объекта(различается с первым для 2 типа хранения))</returns>
        public static Tuple<int, int, int> FindObject(int objectId, int registerId)
        {
            var res = OMStateMachineStatuses.Where(x => x.Objectid == objectId && x.Registerid == registerId)
                .Select(x => new { x.Id, x.Statuscode, x.Objectid });// x.Id).Select(x => x.Statuscode);
            var tmp = res.Execute().Select(x => x.Objectid != null ? (x.Statuscode != null ? Tuple.Create((int)x.Objectid, (int)x.Statuscode, (int) x.Id) : null) : null)
                .FirstOrDefault();
            return tmp;
        }
    }
}
