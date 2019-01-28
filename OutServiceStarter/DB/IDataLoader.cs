using System.Data;

namespace Inec.StateMachine.OutServiceStarter.DB
{
    public interface IDataLoader
    {
        DataTable Table();
    }
}
