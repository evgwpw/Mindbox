namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// одна возможность изменения статуса
    /// </summary>    
    public class StateMachineTransitionItem
    {
        /// <summary>
        /// текущий статус
        /// </summary>
        public int CurrentStatus { get; private set; }

        /// <summary>
        /// статус, в который можно перейти
        /// </summary>
        public int NextStatus { get; private set; }

        /// <summary>
        /// идентификатор машины состояний
        /// </summary>
        public int StateMachineId { get; private set; }

        /// <summary>
        /// идентификатор критерия
        /// </summary>
        public int CriteriaId { get; private set; }

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="currentStatus">текущий статус</param>
        /// <param name="nextStatus">статус, в который можно перейти</param>
        /// <param name="stateMachineId">идентификатор машины состояний</param>
        /// <param name="criteriaId"></param>
        public StateMachineTransitionItem(int currentStatus, int nextStatus, int stateMachineId, int criteriaId)
        {
            CurrentStatus = currentStatus;
            NextStatus = nextStatus;
            StateMachineId = stateMachineId;
            CriteriaId = criteriaId;
        }

        public override string ToString()
        {
            const string template = @"CurrentStatus-{0}\r\nNextStatus-{1}\r\nStateMachineId-{2}\r\nCriteriaId-{3}";
            return string.Format(template, CurrentStatus, NextStatus, StateMachineId, CriteriaId);
        }
    }
}
