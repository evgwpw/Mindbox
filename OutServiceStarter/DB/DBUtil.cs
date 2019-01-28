using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace Inec.StateMachine.OutServiceStarter.DB
{
    public class DBUtil
    {
        public static T ConnectionFunction<T>(Func<OracleConnection, T> func)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(Configuration.ConfigManager.ConnectionString))
                {
                    connection.Open();
                    return func(connection);
                }
            }
            catch (Exception exc)
            {
                var lExc = new StateMachineException("Ошибка при работе с БД " + exc.Message, exc);
                Core.ErrorManagment.ErrorManager.LogErrorInWinSrvEnv(lExc);
                throw lExc;
            }
        }
        /// <summary>
        /// результат процедуры RegInDocControl
        /// </summary>
        public class RegInDocControlResult
        {
            public static readonly RegInDocControlResult Empty = new RegInDocControlResult();
            public int IDDoc { get; private set; }
            public string NumReg { get; private set; }
            public DateTime DateReg { get; private set; }
            public string Return { get; private set; }
            private RegInDocControlResult() { }
            public RegInDocControlResult(int iDDoc, string numReg, DateTime dateReg, string @return)
            {
                IDDoc = iDDoc;
                NumReg = numReg;
                DateReg = dateReg;
                Return = @return;
            }
            public override string ToString()
            {
                const string template = "IDDoc - {0}; NumReg - {1}; DateReg - {2}; Return - {3}";
                return string.Format(template, IDDoc, NumReg, DateReg, Return);
            }
        }
        public class asguf2_pkg
        {
            static string StoredProcedureRegInDocControlFullName
            {
                get
                {
                    return Configuration.ConfigManager.PackageName + "RegInDocControl";
                }
            }
            static string FunctionGetBarCodeFullName
            {
                get
                {
                    return Configuration.ConfigManager.PackageName + "GetBarCode";
                }
            }
            /*
                 FUNCTION RegInDocControl
     ( outcomenumber    IN VARCHAR2,
       authororgname    IN VARCHAR2,
       outcomedate      IN DATE,
       author           IN VARCHAR2,
       typerequest      IN NUMBER,
       docdescr         IN VARCHAR2,
       barcode          IN VARCHAR2,
       pometki          IN VARCHAR2,
       idrubr           IN NUMBER,
       ind              IN NUMBER,
       addr             IN VARCHAR2,
       phone            IN VARCHAR2,
       mail             IN VARCHAR2,
       onhand           IN NUMBER,
       appltype         IN VARCHAR2,
       cadnum           IN VARCHAR2,
       klass            IN NUMBER,
       indexi           IN NUMBER,
       docid            OUT NUMBER,
       docnum           OUT VARCHAR2,
       docdate          OUT DATE )
    RETURN VARCHAR2;
             */
            public static RegInDocControlResult RegInDocControl(string outcomenumber, string authororgname, DateTime outcomedate, string author, decimal typerequest, string docdescr, string barcode,
                string pometki, decimal idrubr, decimal ind, string addr, string phone, string mail, decimal onhand, string appltype, string cadnum, decimal klass, decimal indexi)
            {
                var res = ConnectionFunction(connection => 
                {
                    var command = connection.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = StoredProcedureRegInDocControlFullName;
                    var p = command.Parameters.Add("retValue", OracleDbType.Varchar2, System.Data.ParameterDirection.ReturnValue);
                    p.Size = 2000;
                    p = command.Parameters.Add("outcomenumber", outcomenumber);
                    p = command.Parameters.Add("authororgname", authororgname);
                    p = command.Parameters.Add("outcomedate", outcomedate);
                    p = command.Parameters.Add("author", author);
                    p = command.Parameters.Add("typerequest", typerequest);
                    p = command.Parameters.Add("docdescr", docdescr);
                    p = command.Parameters.Add("barcode", barcode);
                    p = command.Parameters.Add("pometki", pometki);
                    p = command.Parameters.Add("idrubr", idrubr);
                    p = command.Parameters.Add("ind", ind);
                    p = command.Parameters.Add("addr", addr);
                    p = command.Parameters.Add("phone", phone);
                    p = command.Parameters.Add("mail", mail);
                    p = command.Parameters.Add("onhand", onhand);
                    p = command.Parameters.Add("appltype", appltype);
                    p = command.Parameters.Add("cadnum", cadnum);
                    p = command.Parameters.Add("klass", klass);
                    p = command.Parameters.Add("indexi", indexi);
                    p = command.Parameters.Add("docid", OracleDbType.Decimal, System.Data.ParameterDirection.Output);
                    p = command.Parameters.Add("docnum", OracleDbType.Varchar2, System.Data.ParameterDirection.Output);
                    p.Size=100;
                    p = command.Parameters.Add("docdate", OracleDbType.Date, System.Data.ParameterDirection.Output);
                    command.ExecuteNonQuery();
                    string pRetValue = Convert.ToString(command.Parameters["retValue"].Value);
                    var docId = (Oracle.DataAccess.Types.OracleDecimal)command.Parameters["docid"].Value;
                    int pDocid = -1;
                    if (!docId.IsNull)
                    {
                        pDocid = docId.ToInt32();
                    }
                    else
                    {
                        Console.WriteLine("docId = null");
                    }
                    var pDocnum = Convert.ToString(command.Parameters["docnum"].Value);
                    DateTime Docdate = default(DateTime);
                    var pDocdate = (Oracle.DataAccess.Types.OracleDate)command.Parameters["docdate"].Value;
                    if (!pDocdate.IsNull)
                    {
                        Docdate = pDocdate.Value;
                    }
                    else                    
                    {
                        Console.WriteLine("Docdate = null");
                    }
                    //var pDocdate = Convert.ToDateTime();
                    var tmp = new RegInDocControlResult(pDocid, pDocnum, Docdate, pRetValue);
                    Console.WriteLine("Результат выполнения RegInDocControl\r\n" + tmp);
                    return tmp;
                });
                return res;                
            }
            /// <summary>
            /// получаем результат работы asguf2_pkg.GetBarCode
            /// </summary>
            /// <returns></returns>
            public static string GetBarCode()
            {
                var res = ConnectionFunction(connection => 
                {
                    var command = connection.CreateCommand();
                    command.CommandText = FunctionGetBarCodeFullName;
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    var p = command.Parameters.Add("retValue", OracleDbType.Varchar2, System.Data.ParameterDirection.ReturnValue);
                    p.Size = 100;
                    command.ExecuteNonQuery();
                    var tmp = command.Parameters["retValue"].Value;
                    if (tmp == null || tmp == DBNull.Value)
                    {
                        throw new StateMachineException("Не удалось выполнить asguf2_pkg.GetBarCode");
                    }
                    return Convert.ToString(tmp);
                });
                return res;
            }
        
        }
        public static string GetRublicator(int statusId, int stateMachineId)
        {
            var res = ObjectModel.OMStateMachineOuterProcess.Where(x => x.StatusId == statusId && x.StateMachineId == stateMachineId)
                .SelectAll()
                .Execute()
                .Select(x => x.Resolution)
                .FirstOrDefault();
            return res;
        }
    }
}
