using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.ObjectModel;
using ObjectModel;
using ObjectModel.Core.Shared;

namespace Inec.StateMachine.Storage
{
    public class Statuse
    {
        /// <summary>
        /// возвращает статусы относящиеся к данной машине состояний.
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        static StatuseItemList _GetStatuses(int stateMachineId)
        {
            var referenceIds = OMStateMachine.Where(x => x.Id == stateMachineId).Select(x => x.ReferenceId).Execute();
            if (referenceIds.Count < 1)
                return StatuseItemList.Empty;
            var referenceId = referenceIds[0].ReferenceId;
            var data = OMReferenceItem.Where(x => x.ReferenceId == referenceId)
                .Select(x => x.ItemId).Select(x => x.Value).Execute()
                .Select(x => new StatuseItem((int)x.ItemId, x.Value)).OrderBy(x=>x.Value);
            return new StatuseItemList(stateMachineId, data);
        }
        static Func<int, StatuseItemList> __GetStatuses = new Func<int, StatuseItemList>(_GetStatuses).MemoizeFun();
        /// <summary>
        /// возвращает статусы относящиеся к данной машине состояний.
        /// </summary>
        /// <param name="stateMachineId"></param>
        /// <returns></returns>
        public static StatuseItemList GetStatuses(int stateMachineId) 
        {
            return _GetStatuses(stateMachineId);
        }
        /// <summary>
        /// устанавливаем статус для объекта реестра
        /// </summary>
        /// <param name="registerId">идентификатор реестра</param>
        /// <param name="objectId">идентификатор объекта</param>
        /// <param name="TDInstanceId">TDInstanceId</param>
        /// <param name="S_">дата С</param>
        /// <param name="PO_">дата ПО</param>
        public static int SetStatuse(int registerId, int objectId, StatuseItem status, int TDInstanceId = -1, DateTime S_ = default(DateTime), DateTime PO_ = default(DateTime))
        {
            OMStateMachineStatuses tmp = new OMStateMachineStatuses();
            tmp.Registerid = registerId;
            tmp.Objectid = objectId;
            tmp.Statuscode = status.Id;
            tmp.Statusvalue = status.Value;
            var emp_id = tmp.Save(TDInstanceId, S_, PO_);
            return emp_id;
        }
       // public OMStateMachineStatuses FindStatus(int registerId, int objectId, 
    }
}
