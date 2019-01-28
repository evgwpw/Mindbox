using System.Collections.Generic;
using System.Linq;
using ObjectModel;

namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// смежные реестры
    /// </summary>
    public class StateMachineAdjacentRegisters
    {
        /// <summary>
        /// получение смежных реестров по машине
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        public static IEnumerable<long> GetUsedAdjacentRegisters(long stateMachineId)
        {
            var res = OMStateMachineAdjacentRegs.Where(x => x.StateMachineId == stateMachineId && x.IsUsed)//все смежные реестры, т. е. такие, изменение которых приведит к запуску машины
                .Select(x => x.RegisterId)
                .Execute()
                .Select(x => x.RegisterId);
            return res;
        }
        /// <summary>
        /// получение смежных реестров по машине
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        public static IEnumerable<long> GetAdjacentRegisters(long stateMachineId)
        {
            var res = OMStateMachineAdjacentRegs.Where(x => x.StateMachineId == stateMachineId)//все смежные реестры, т. е. такие, изменение которых приведит к запуску машины
                .Select(x => x.RegisterId)
                .Execute()
                .Select(x => x.RegisterId);
            return res;
        }
        /// <summary>
        /// добавление смежного реестра
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <param name="registerId"></param>
        /// <returns></returns>
        public static long AddAdjacentRegister(long stateMachineId, long registerId)
        {
            var data = OMStateMachineAdjacentRegs.Where(x => x.RegisterId == registerId && x.StateMachineId == stateMachineId)
                .SelectAll()
                .Execute()
                .FirstOrDefault();
            if (data != null)
                return -1;
            data = OMStateMachineAdjacentRegs.CreateEmpty();
            data.StateMachineId = stateMachineId;
            data.RegisterId = registerId;
            data.Save();
            return data.Id;
        }
        /// <summary>
        /// удаление объекта
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <param name="registerId"></param>
        public static void DeleteAdjacentRegister(long stateMachineId, long registerId)
        {
            var data = OMStateMachineAdjacentRegs.Where(x => x.RegisterId == registerId && x.StateMachineId == stateMachineId)
                           .SelectAll()
                           .Execute()
                           .FirstOrDefault();
            if (data != null)
            {
                data.Destroy();
            }
        }
    }
}
