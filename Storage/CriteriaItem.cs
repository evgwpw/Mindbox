using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Register.QuerySubsystem;

namespace Inec.StateMachine.Storage
{
    public class CriteriaItem
    {
        /// идентификатор критерия
        /// </summary>
        public int CriteriaId { get; private set; }
        /// <summary>
        /// порядок применения
        /// </summary>
        public int Order { get; private set; }
        /// <summary>
        /// тело запроса
        /// </summary>
        public QSQuery Query
        {
            get
            {
                return Util.QSQueryUtil.Deserialize(XMLQuery);
            }
        }
        /// <summary>
        /// идентификатор статуса
        /// </summary>
        public int StatusId { get; private set; }
        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="order">порядок применения</param>
        /// <param name="query">тело запроса</param>
        /// <param name="statusId">идентификатор статуса</param>
        public string XMLQuery { get; private set; }
        public string Description { get; private set; }
        public CriteriaItem(int criteriaId, int order, string xMLQuery, int statusId, string description)
        {
            if (criteriaId < 0)
                throw new ArgumentOutOfRangeException("criteriaId");
            if (order < 0)
                throw new ArgumentOutOfRangeException("order");
            if (xMLQuery == null)
                throw new ArgumentNullException("query");
            if (statusId < 0)
                throw new ArgumentOutOfRangeException("statusId");
            Order = order;
            XMLQuery = xMLQuery;
            StatusId = statusId;
            CriteriaId = criteriaId;
            Description = description;
        }
        const string ToStringTemplate = @"Order - {0}
StatusId - {1}
QSQuery - {2}
";
        public override string ToString()
        {
            return string.Format(ToStringTemplate, Order, StatusId, Util.QSQueryUtil.Serialize(Query));
        }
    }
}
