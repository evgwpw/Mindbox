using System;
using System.Linq;
using ObjectModel;

namespace Inec.StateMachine.Storage
{
    public class StateMachineHelper
    {
        static StateMachineHelper()
        {
            __GetRefisterIdFromStateMachine = new Func<long, long>(_GetRefisterIdFromStateMachine).MemoizeFun();
            __GetDefaultStatus = _GetDefaultStatus;
        }
        
        static long _GetRefisterIdFromStateMachine(long stateMachineId)
        {
            var res = OMStateMachine.Where(x => x.Id == stateMachineId).Select(x=>x.RegisterId).Execute();
            if (res.Count > 0)
                return (int)res[0].RegisterId;
            throw new StateMachineException("Не удалось найти машину состояний с Id=" + stateMachineId);
        }
        /// <summary>
        /// получаем идентификатор реестра по идентификатору машины состояний
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        public static long GetRegisterIdFromStateMachine(long stateMachineId)
        {
            return __GetRefisterIdFromStateMachine(stateMachineId);
        }
        static Func<long, long> __GetRefisterIdFromStateMachine;

        static int _GetDefaultStatus(int stateMachineId)
        {
            var res = OMStateMachine.Where(x => x.Id == stateMachineId)
                .Select(x => x.DefaultStatusId)
                .Execute()
                .Select(x => x.DefaultStatusId)
                .FirstOrDefault();
            if (res == 0)
                throw new StateMachineException("Не удалось найти машину состояний с идентификатором = " + stateMachineId);
            return (int)res;
        }
        static Func<int, int> __GetDefaultStatus;
        /// <summary>
        /// возвращает статус "Не определено" для машины состояний
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        public static int GetDefaultStatus(int stateMachineId)
        {
            return __GetDefaultStatus(stateMachineId);
        }

        static StorageType _GetStorageType(int stateMachineId)
        {
            var res = OMStateMachine.Where(x => x.Id == stateMachineId)
                .Select(x => x.StorageType)
                .Execute()
                .Select(x => x.StorageType)
                .FirstOrDefault();
            return (StorageType)res;
        }
        static Func<int, StorageType> __GetStorageType = new Func<int, StorageType>(_GetStorageType).MemoizeFun();
        /// <summary>
        /// возвращает тип хранения, внутри самого реестра или в отдельном реестре
        /// </summary>
        /// <param name="stateMachineId">идентификатор машины состояний</param>
        /// <returns>тип хранения</returns>
        public static StorageType GetStorageType(int stateMachineId)
        {
            return __GetStorageType(stateMachineId);
        }

        static int _GetAttributeStarageFromStateMachineId(int stateMachineId)
        {
            var res = OMStateMachine.Where(x => x.Id == stateMachineId)
                .Select(x => x.AttributeId)
                .Execute()
                .Select(x => x.AttributeId)
                .FirstOrDefault();
            if (!res.HasValue)
                throw new StateMachineException("Не удалось получить идентефикатор атрибута для хранения в реестре");
            return (int)res.Value;
        }
        static readonly Func<int, int> __GetAttributeStarageFromStateMachineId = new Func<int, int>(_GetAttributeStarageFromStateMachineId).MemoizeFun();
        /// <summary>
        /// идентификатор атрибута по идентификатору машины состояний
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        public static int GetAttributeStarageFromStateMachineId(int stateMachineId)
        {
            return __GetAttributeStarageFromStateMachineId(stateMachineId);
        }
        /// <summary>
        /// сохранять ли историю
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        static bool _IsSaveHistory(int stateMachineId)
        {
            var res = OMStateMachine.Where(x => x.Id == stateMachineId)
                   .Select(x => x.SaveHistory)
                   .Execute()
                   .Select(x => x.SaveHistory)
                   .FirstOrDefault();
            return res;
        }

        private static Func<int, bool> __IsSaveHistory = new Func<int, bool>(_IsSaveHistory).MemoizeFun();

        /// <summary>
        /// сохранять ли историю
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        public static bool IsSaveHistory(int stateMachineId)
        {
            return __IsSaveHistory(stateMachineId);
        }
    }
}
