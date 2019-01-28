namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// тип запуска машины состояний
    /// </summary>
    public enum ProcessingType
    {
        /// <summary>
        /// не определено
        /// </summary>
        None=0,
        /// <summary>
        /// по событию в платформе в методе Save
        /// </summary>
        InSaveMethod=1,
        /// <summary>
        /// иное, чем Save
        /// </summary>
        OutSaveMethodStart=2
    }
}
