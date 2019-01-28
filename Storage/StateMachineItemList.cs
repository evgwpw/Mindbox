using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using ObjectModel;
using Core.ErrorManagment;

namespace Inec.StateMachine.Storage
{
    public class StateMachineItemList : Collection<StateMachineItem>
    {
        public StateMachineItemList(IEnumerable<StateMachineItem> src)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            this.CopyEnunerableToList(src);
        }
        /// <summary>
        /// все машины
        /// </summary>
        public static StateMachineItemList AllMachines
        {
            get
            {
                try
                {
                    var res = OMStateMachine.Where(x => true)
                        .SelectAll()
                        .Execute()
                        .Select(x => new StateMachineItem(x));
                  //  new LogEntity(res.EnumerableOfTToString(), LogEntityType.INFO).WriteToDB();
                    return new StateMachineItemList(res);
                }
                catch (Exception exc)
                {
                    var sexc = new StateMachineException("Ошибка при получении всех  машин", exc);
                    ErrorManager.LogErrorInWinSrvEnv(sexc);
                    throw sexc;
                }
            }
        }
        /// <summary>
        /// мащины, которые исполняются в методе save
        /// </summary>
        public static StateMachineItemList InSaveList
        {
            get
            {
                var data = AllMachines.Where(x => x.ProcessingType == ProcessingType.InSaveMethod && x.Enabled);
                return new StateMachineItemList(data);
            }
        }
        /// <summary>
        /// мащины, которые не исполняются в методе save
        /// </summary>
        public static StateMachineItemList OutSaveMethodList
        {
            get
            {
                var data = AllMachines.Where(x => x.ProcessingType == ProcessingType.OutSaveMethodStart && x.Enabled);
                return new StateMachineItemList(data);
            }
        }
    }
}
