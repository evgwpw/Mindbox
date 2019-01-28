namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// заглушка, если нашли объект с одинаковым реестром, объектом и статусом, то ничего не делаем
    /// </summary>
    public class NonStorage : StorageBase
    {
        public override int Save()
        {
            return -1;
        }
    }
}
