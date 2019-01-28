using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// одна запись  из core_reference_item
    /// </summary>
    public class RefItem
    {
        IList<RefItem> _children;
        public IList<RefItem> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<RefItem>();
                }
                return _children;
            }
        }
        public int ItemId { get; private set; }
        public string Code { get; private set; }
        public string Value { get; private set; }

        public RefItem(int itemId, string code, string value, IEnumerable<RefItem> children)
        { 
            ItemId = itemId;
            Code = code;
            Value = value;
            if (children != null)
            {
                Children.CopyEnunerableToList(children);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddChild(RefItem item)
        {
            Children.Add(item);
        }
        public void RemoveChild(RefItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            var ind = Children.IndexOf(item);
            if (ind < 0)
                throw new IndexOutOfRangeException();
            Children.RemoveAt(ind);
        }
        public override bool Equals(object obj)
        {
            var tmp = obj as RefItem;
            if (tmp == null)
                return false;
            return ItemId == tmp.ItemId;
        }
        public override int GetHashCode()
        {
            return ItemId ^ ~Code.GetStringHashCode() ^ Value.GetStringHashCode();
        }
    }
    /// <summary>
    /// работаем c core_reference_item
    /// </summary>
    public class RefItemList: Collection<RefItem>
    {
        public RefItemList(int referenceId)
        {
            ReferenceId = referenceId;
        }

        public int ReferenceId { get; private set; }
    }
}
