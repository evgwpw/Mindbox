using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inec.StateMachine.Storage
{
    public class StatuseItem
    {
        public int Id { get; private set; }
        public string Value { get; private set; }
        public StatuseItem(int id, string value)
        {
            Id = id; 
            Value = value;
        }
        public override int GetHashCode()
        {
            return Id ^ Int32.MaxValue;
        }
        public override bool Equals(object obj)
        {
            var tmp = obj as StatuseItem;
            if (tmp == null)
                return false;
            return tmp.Id == this.Id;
        }
        public override string ToString()
        {
            const string template = @"Id - {0} Value - {1}";
            return string.Format(template, Id, Value);
        }
    }
}
