using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inec.StateMachine.Loging
{
    /// <summary>
    /// тип записи в лог машины состояний
    /// </summary>
    [Flags]
    [Serializable]
    public enum LogEntityType
    {
        /// <summary>
        /// нет значения
        /// </summary>
        NONE = 0,
        /// <summary>
        /// информация
        /// </summary>
        INFO = 1,
        /// <summary>
        /// предупреждение
        /// </summary>
        WARNING = 2,
        /// <summary>
        /// ошибка
        /// </summary>
        ERROR = 4,
        /// <summary>
        /// результат работы машины
        /// </summary>
        RESULT = 5
    }
}
