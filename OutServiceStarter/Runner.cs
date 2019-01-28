using System;
using System.Linq;
using System.Text;
using System.Data;
using Inec.StateMachine.OutServiceStarter.Configuration;
using System.IO;
using System.Xml;

namespace Inec.StateMachine.OutServiceStarter
{
    public class Runner
    {
        public long StatusId { get; private set; }
        public long StateMachineId { get; private set; }
        public long ObjectId { get; private set; }
        public Runner(long statusId, long stateMachineId, long objectId)
        {
            StatusId = statusId;
            StateMachineId = stateMachineId;
            ObjectId = objectId;
        }
        public void Run()
        {
            try
            {
                //данные из реестров
                var data = new DB.DataLoader(StateMachineId, StatusId, ObjectId).Table();
                new Loging.LogEntity("Данные для загрузки\r\n" + TableToXml(data), Loging.LogEntityType.INFO).WriteToDB();
                //идентификатор рублики
                var rublicator = DB.DBUtil.GetRublicator((int)StatusId, (int)StateMachineId);
                //маппинг данных на датасет
                var mapper = new Mapping.MapperBase(rublicator, data);
                mapper.Run();
                var set = mapper.DataSet;

                new Loging.LogEntity("DataSet после маппинга данных\r\n" + TableToXml(set), Loging.LogEntityType.INFO).WriteToDB();
                int iRublikator;
                if (!int.TryParse(rublicator, out iRublikator))
                {
                    throw new StateMachineException("Не удалось преобразовать значение " + rublicator + " в int");
                }
                SetDocumentsId(set, iRublikator);
                var res = CallRegInDocControl(iRublikator);
                new Loging.LogEntity("Результат вызова RegInDocControl\r\n" + res, Loging.LogEntityType.INFO).WriteToDB();
                var service = new WSReferences.Service {Url = ConfigManager.ServiceUrl};
                var getApplicationsRes = service.GetApplications(iRublikator, res.NumReg, res.DateReg, set);
                new Loging.LogEntity("Результат вызова GetApplications\r\n" + getApplicationsRes, Loging.LogEntityType.INFO).WriteToDB();
            }
            catch (Exception exc)
            {
                Core.ErrorManagment.ErrorManager.LogErrorInWinSrvEnv(exc);
            }
        }

        public static void SetDocumentsId(DataSet set, int iDoc)
        {
            const string idDocuments = "id_documents";
            foreach (var t in set.Tables.Cast<DataTable>())
            {
                if (t.Columns.Cast<DataColumn>().Any(x => x.ColumnName == idDocuments) && t.Rows.Count > 0)
                {
                    t.Rows[0][idDocuments] = iDoc;
                }
            }
        }

        public static DB.DBUtil.RegInDocControlResult CallRegInDocControl(int rublicator)
        {
            var res = DB.DBUtil.asguf2_pkg.RegInDocControl(Guid.NewGuid().ToString(), ConfigManager.NameApplicantOrganization, DateTime.Now,
                ConfigManager.ApplicantFIO, 1, ConfigManager.Comment, DB.DBUtil.asguf2_pkg.GetBarCode(),
                ConfigManager.Markups, rublicator, ConfigManager.ZipCode, ConfigManager.MailingAddress, ConfigManager.Phone, ConfigManager.Email,
                3, "Ю", ConfigManager.Comment, 801, 402);
            return res;
        }

        string TableToXml(DataTable t) 
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var xw = XmlWriter.Create(sw))
                {
                    t.WriteXml(xw, XmlWriteMode.IgnoreSchema);
                }                
            }
            var res = sb.ToString();
            return res;
        }
        string TableToXml(DataSet t)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var xw = XmlWriter.Create(sw))
                {
                    t.WriteXml(xw, XmlWriteMode.IgnoreSchema);
                }
            }
            var res = sb.ToString();
            return res;
        }
    }
}
