using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Register.RegisterEntities.CoreEvents.Attributes;
using Core.Register.RegisterEntities.CoreEvents;
using Inec.StateMachine.Storage;
using System.Threading;
using Core.ErrorManagment;
using System.Threading.Tasks;

namespace Inec.StateMachine
{
    /// <summary>
    /// подписывается на события AfterSave платформы
    /// </summary>
    [CoreEventTypeAttribute]
    public class EventWrapper
    {
        static Timer Timer;
        static object Locker = new object();
        static EventHandler<CoreEventArgs> AllMachineHandler;
        static AutoResetEvent AutoReset;
        static List<int> Machines;
        public static void Process(object obj, CoreEventArgs args)
        {
            Init();
            AllMachineHandler(obj, args);
        }
        static EventWrapper()
        {
            AutoReset = new AutoResetEvent(true);
            Timer = new Timer(o => Update(), null, 0, 60000);
        }
        static void Update()
        {
            try
            {
                if (AllMachineHandler == null)
                {
                    try
                    {
                        AutoReset.WaitOne();
                        var ms = StateMachineItemList.InSaveList.Select(x => (int)x.Id).OrderBy(x => x).ToList();
                        if (ms.SequenceEqual(Machines))
                            return;
                        Machines = ms;
                        UpdateHandlers();
                    }
                    finally
                    {
                        AutoReset.Set();
                    }
                }
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Ошибка обновления машины состояний", exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
            }
        }
        static void Init()
        {
            try
            {
                if (AllMachineHandler == null)
                {
                    try
                    {
                        AutoReset.WaitOne();
                        if (AllMachineHandler == null)
                        {
                            Machines = StateMachineItemList.InSaveList.Select(x => (int)x.Id).OrderBy(x => x).ToList();
                            UpdateHandlers();
                        }
                    }
                    finally
                    {
                        AutoReset.Set();
                    }
                }
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Ошибка инициализации машины состояний", exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
            }
        }

        private static void UpdateHandlers()
        {
            List<EventHandler<CoreEventArgs>> list = new List<EventHandler<CoreEventArgs>>();
            //машин может быть много
            foreach (var sId in Machines)
            {
                StateMachineProcessor p = new StateMachineProcessor(sId);
                list.Add(new EventHandler<CoreEventArgs>(p.Process));
            }
            AllMachineHandler = (EventHandler<CoreEventArgs>)Delegate.Combine(list.ToArray());
        }
    }
}
