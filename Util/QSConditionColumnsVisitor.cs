using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Register.QuerySubsystem;

namespace Inec.StateMachine.Util
{
    /// <summary>
    /// собираем все QSColumn
    /// </summary>
    public class QSConditionColumnsVisitor : IConditionVisitor
    {
        /// <summary>
        /// найденные столбцы
        /// </summary>
        public IEnumerable<QSColumn> Columns
        {
            get
            {
                return _columns;
            }
        }
        IList<QSColumn> _columns = new List<QSColumn>();
        public void Visit(QSConditionGroup c)
        {
            foreach (var con in c.Conditions)
            {
                con.AcceptVisitor(this);
            }
        }

        public void Visit(QSConditionSimple c)
        {
            if (c.LeftOperand != null)
                _columns.Add(c.LeftOperand);
            if (c.RightOperand != null)
                _columns.Add(c.RightOperand);
        }
    }
}
