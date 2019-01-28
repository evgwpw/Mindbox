using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Register.QuerySubsystem;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Core.ErrorManagment;
using Core.Shared.Extensions;

namespace Inec.StateMachine.Util
{
    public class QSQueryUtil
    {
        static object Locker = new object();
        static ConcurrentDictionary<string, QSQuery> Cache = new ConcurrentDictionary<string, QSQuery>();
        static ConcurrentDictionary<int, XmlSerializer> _Serializers = new ConcurrentDictionary<int, XmlSerializer>();
        static XmlSerializer Serializer
        {
            get
            {
                XmlSerializer res;
                var tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
                if (_Serializers.TryGetValue(tid, out res))
                    return res;
                res = new XmlSerializer(typeof(QSQuery));
                _Serializers.TryAdd(tid, res);
                return res;
            }
        }
        static Type[] constTypes = { typeof(int[]), typeof(double[]), typeof(string[]), typeof(long[]) };
        /// <summary>
        /// из строки содержащей xml десериализует объект класса QSQuery
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static QSQuery Deserialize(string str)
        {            
            return str.DeserializeFromXml<QSQuery>(constTypes);
            //if (string.IsNullOrEmpty(str))
            //    throw new ArgumentNullException("str");
            //QSQuery res;
            //if (Cache.TryGetValue(str, out res))
            //    return res;

            //using (TextReader tr = new StringReader(str))
            //{
            //    res = Serializer.Deserialize(tr) as QSQuery;
            //    Cache.TryAdd(str, res);
            //}
            //return res;
        }
        /// <summary>
        /// сериализует QSQuery в xml, который возвращает в виде строки 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string Serialize(QSQuery query)
        {
            return query.SerializeToXml();
            //if (query == null)
            //    throw new ArgumentNullException("query");
            //StringBuilder sb = new StringBuilder();
            //using (TextWriter tw = new StringWriter(sb))
            //{
            //    try
            //    {
            //        Serializer.Serialize(tw, query);
            //    }
            //    catch (Exception exc)
            //    {
            //        var str = sb.ToString();
            //    }
            //}
            //return sb.ToString();
        }
        /// <summary>
        /// получить смежные
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<QSQuery> CreateContiguous(QSQuery query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            throw new NotImplementedException();
        }
        /// <summary>
        /// полычаем смежные реестры
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetRegisters(IEnumerable<QSQuery> src)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            var data = src.Select(x => GetRegisters(x))
                .SelectMany(x => x)
                .Distinct()
                .ToList();
            return data;
        }
        /// <summary>
        /// список смежных реестров
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetRegisters(QSQuery query)
        {
            return __GetRegisters(query);
        }
        static Func<QSQuery, IEnumerable<int>> __GetRegisters = new Func<QSQuery, IEnumerable<int>>(_GetRegisters).MemoizeFun(q => q.GetSql());
        static IEnumerable<int> _GetRegisters(QSQuery query)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            try
            {
                QSColumnRegisterVisitor v = new QSColumnRegisterVisitor();
                var v1 = new QSConditionColumnsVisitor();
                var cond = query.Condition;
                if (cond == null)
                    return Enumerable.Empty<int>();
                cond.AcceptVisitor(v1);
                foreach (var c in v1.Columns)
                {
                    c.AcceptVisitor(v);
                }
                return v.Registers;
            }
            catch (Exception exc)
            {
                var sexc = new StateMachineException("Не удалось получить список смежных реестров\r\n" + query.GetSql(), exc);
                Task.Factory.StartNew(() => ErrorManager.LogErrorInWinSrvEnv(sexc));
                throw sexc;
            }
        }
        /// <summary>
        /// генерируем запрос, который по ид смежного реестра найдет ид основного объекта
        /// </summary>
        /// <param name="mainRegisterId">идентификатор основного реестра</param>
        /// <param name="contiguousRegisterId">идентификатор смежного реестра</param>
        /// <param name="contiguousEmpId">ид смежного реестра</param>
        /// <returns>запрос на получение ид основного реестра</returns>
        public static QSQuery GetContiguousQuery(int mainRegisterId, int contiguousRegisterId, int contiguousEmpId)
        {
            QSQuery query = new QSQuery(mainRegisterId);
            var attr = GetRegisterIdAttribute(contiguousRegisterId);
            query.Condition = new QSConditionSimple(new QSColumnSimple(attr), QSConditionType.Equal, contiguousEmpId);
            return query;
        }
        #region вспомогательные методы, обращения к БД
        /// <summary>
        /// получаем одентификатор 
        /// </summary>
        /// <param name="registerId"></param>
        /// <returns></returns>
        public static int GetRegisterIdAttribute(int registerId)
        {
            return __GetRegisterIdAttribute(registerId);
        }
        static Func<int, int> __GetRegisterIdAttribute = new Func<int, int>(_GetRegisterIdAttribute).MemoizeFun();
        static int _GetRegisterIdAttribute(int registerId)
        {
            var res = ObjectModel.Core.Register.OMAttribute.Where(x => x.RegisterId == registerId && x.IsPrimaryKey)
                .Select(x => x.Id)
                .Execute()
                .Select(x => (int)x.Id)
                .FirstOrDefault();
            if (res == 0)
            {
                throw new StateMachineException("Не удалось найти аттрибут Id для реестра " + registerId);
            }
            return res;
        }



        static int _GetRegisterFromAttribute(int attributeId)
        {
            var res = ObjectModel.Core.Register.OMAttribute.Where(x => x.Id == attributeId)
                .Select(x => x.RegisterId)
                .Execute()
                .Select(x => x.RegisterId)
                .FirstOrDefault();
            return Convert.ToInt32(res);
        }
        static Func<int, int> __GetRegisterFromAttribute = _GetRegisterFromAttribute;
        /// <summary>
        /// получаем идентификатор реестра по аттрибуту
        /// </summary>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public static int GetRegisterFromAttribute(int attributeId)
        {
            return __GetRegisterFromAttribute(attributeId);
        }
        #endregion
    }
}
