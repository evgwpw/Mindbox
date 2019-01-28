using System;
using System.Linq;
using ObjectModel;


namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// определяет возможность изменения одного статуса на другой
    /// </summary>
    public class StateMachineTransitions
    {
        static StateMachineTransitionItemsList _GetTransitionItemsListForStateMachine(int criteriaId)
        {
            var stateMachineId = GetStateMachineFromCriteria(criteriaId);
            var res = OMStateMachineTransitions.Where(x => x.CtriteriaId == criteriaId)
                .Select(x => x.Curentstatus)
                .Select(x => x.Nextstatus)
                .Select(x => x.CtriteriaId)
                .Execute()
                .Select(
                    x =>
                        new StateMachineTransitionItem(Convert.ToInt32(x.Curentstatus), Convert.ToInt32(x.Nextstatus),
                            Convert.ToInt32(stateMachineId), Convert.ToInt32(x.CtriteriaId)));
                //.Select(x => stateMachineId != null ? 
                //    (x.CtriteriaId != null ? 
                //    (x.Nextstatus != null ? 
                //    (x.Curentstatus != null ? 
                //    new StateMachineTransitionItem((int)x.Curentstatus, (int)x.Nextstatus, (int)stateMachineId, (int)x.CtriteriaId) : 
                //    null) : 
                //    null) : 
                //    null) : 
                //    null);
            return new StateMachineTransitionItemsList(res);
            //throw new NotImplementedException();

        }
        static Func<int, StateMachineTransitionItemsList> _getTransitionItemsListForStateMachine = new Func<int, StateMachineTransitionItemsList>(_GetTransitionItemsListForStateMachine).MemoizeFun();
        /// <summary>
        /// получает список возможных переходов для конкретной машины состояний
        /// </summary>
        /// <param name="stateMachineId">идентификатор машины состояний</param>
        /// <returns>список переходов</returns>
        public static StateMachineTransitionItemsList GetTransitionItemsListForStateMachine(int stateMachineId)
        {
            return _GetTransitionItemsListForStateMachine(stateMachineId);
        }
        /// <summary>
        /// возможен ли переход для определенной машины состояний их статуса в статус
        /// </summary>
        /// <param name="criteriaId">идентификатор машины состояний</param>
        /// <param name="currentStatusId">текущий статус</param>
        /// <param name="nextStatusId">возможный статус</param>
        /// <returns>true - переход возможен, false - не возможен</returns>
        public static bool IsItPossibleToTransition(int criteriaId, int currentStatusId, int nextStatusId)
        {
            return GetTransitionItemsListForStateMachine(criteriaId)
                .Any(x => x.CurrentStatus == currentStatusId && x.NextStatus == nextStatusId && x.CriteriaId == criteriaId);
        }
        /// <summary>
        /// получем текущий критерий для определенного объекта
        /// </summary>
        /// <param name="registerId">идентификатор реестра</param>
        /// <param name="objectId">идентификатор объекта</param>
        /// <param name="currentDate">дата, на которую нужны сведения</param>
        /// <returns>идентификатор статуса или null</returns>
        public static int? GetCurrentStatus(int registerId, int objectId, DateTime? currentDate = null)
        {
            var query = OMStateMachineStatuses.Where(x => x.Registerid == registerId && x.Objectid == objectId);
            if (currentDate.HasValue)
            {
                query.SetActualDate(currentDate.Value);
            }
            var res = query.Select(x => x.Statuscode).Execute().Select(x => x.Statuscode != null ? (int)x.Statuscode : 0).FirstOrDefault();
            if (res > 0)
                return res;
            return null;
        }

        private static long? GetStateMachineFromCriteria(int criteriaId)
        {
            var res =
                OMStateMachineCriteria.Where(x => x.Id == criteriaId)
                    .Select(x => x.StateMachineId)
                    .Execute()
                    .Select(x=>x.StateMachineId)
                    .FirstOrDefault();
            return res;
        }

    }
}
