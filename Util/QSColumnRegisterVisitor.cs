using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Register.QuerySubsystem;

namespace Inec.StateMachine.Util
{
    /// <summary>
    /// вытаскиваем номера реестров из QSColumn
    /// </summary>
    public class QSColumnRegisterVisitor : IColumnVisitor
    {
        public IEnumerable<int> Registers
        {
            get
            {
                return _registers.Distinct();
            }
        }
        IList<int> _registers = new List<int>();
        public void Visit(QSColumnConstant c)
        {

        }

        public void Visit(QSColumnFunction c)
        {
            foreach (var col in c.Operands)
            {
                col.AcceptVisitor(this);
            }
        }

        public void Visit(QSColumnFunctionExternal c)
        {
            foreach (var col in c.Operands)
            {
                col.AcceptVisitor(this);
            }
        }

        public void Visit(QSColumnQuery c)
        {
            QSConditionColumnsVisitor v = new QSConditionColumnsVisitor();
            c.SubQuery.Condition.AcceptVisitor(v);
            foreach (var col in v.Columns)
            {
                col.AcceptVisitor(this);//собираем все регистры
            }
        }

        public void Visit(QSColumnSimple c)
        {
            var r = QSQueryUtil.GetRegisterFromAttribute(c.AttributeID);
            _registers.Add(r);
        }

        public void Visit(QSColumnSpecial c)
        {
            _registers.Add(c.RegisterID);
        }

        public void Visit(QSColumnIf c)
        {
            QSConditionColumnsVisitor v = new QSConditionColumnsVisitor();
            foreach (var b in c.Blocks)
            {
                b.Condition.AcceptVisitor(v);//собираем все столбцы всех условий
            }
            foreach (var col in v.Columns)
            {
                col.AcceptVisitor(this);//собираем все регистры
            }
        }

        public void Visit(QSColumnSwitch c)
        {
            c.ValueToCompare.AcceptVisitor(this);
            foreach (var b in c.Blocks)
            {
                b.ValueToCompare.AcceptVisitor(this);
            }
        }


    }
}
