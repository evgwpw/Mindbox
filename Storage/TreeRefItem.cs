using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inec.StateMachine.Storage
{
    /// <summary>
    /// вспомогательный класс для древовидных справочников
    /// </summary>
    public class TreeRefItem
    {
        /// <summary>
        /// ид родителя
        /// </summary>
        public int ParentId { get; private set; }
        /// <summary>
        /// ид ребенка
        /// </summary>
        public int ChildId { get; private set; }
        /// <summary>
        /// справочник родителя
        /// </summary>
        public int ReferenceId { get; private set; }
        /// <summary>
        /// справочник ребенка
        /// </summary>
        public int ChildReferenceId { get; private set; }
        public TreeRefItem(int parentId, int childId, int referenceId, int childReferenceId)
        {
            ParentId = parentId;
            ChildId = childId;
            ReferenceId = referenceId;
            ChildReferenceId = childReferenceId;
        }
        public override string ToString()
        {
            const string template = @"ParentId-{0}\r\nChildId-{1}\r\nReferenceId-{2}\r\nChildReferenceId-{3}";
            return string.Format(template, ParentId, ChildId, ReferenceId, ChildReferenceId);
        }
    }
}
