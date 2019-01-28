using System;
using System.Collections.Generic;
using System.Threading;

namespace Inec.StateMachine.Util
{
    /// <summary>
    /// множество с поддержкой многопоточного доступа
    /// </summary>
    /// <typeparam name="T">тип элементов</typeparam>
    public class ConcutentHashSet<T> : ISet<T>
    {
        readonly HashSet<T> _set;
        SpinLock _sLock;
        public ConcutentHashSet()
        {
            _set = new HashSet<T>();
            _sLock = new SpinLock();
        }
        public ConcutentHashSet(IEnumerable<T> src):this()
        {
            if (src == null)
                throw new ArgumentNullException("src");
            foreach (var x in src)
            {
                Add(x);
            }
        }
        void LockAct<TA>(Action<TA> act, TA obj)
        {            
            var gotLock = false;
            try
            {
                _sLock.Enter(ref gotLock);
                act(obj);
            }
            finally
            {
                if (gotLock)
                    _sLock.Exit();
            }
        }
        TA LockFun<TA>(Func<TA> fun)
        {
            var gotLock = false;
            try
            {
                _sLock.Enter(ref gotLock);
                return fun();
            }
            finally
            {
                if (gotLock)
                    _sLock.Exit();
            }
        }
        public bool Add(T item)
        {
            return LockFun(() => _set.Add(item));
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            LockAct(_set.ExceptWith , other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            LockAct(_set.IntersectWith, other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return LockFun(() => _set.IsProperSubsetOf(other));
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return LockFun(() => _set.IsProperSupersetOf(other));
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return LockFun(() => _set.IsSubsetOf(other));
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return LockFun(() => _set.IsSupersetOf(other));
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return LockFun(() => _set.Overlaps(other));
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return LockFun(() => _set.SetEquals(other));
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            LockAct(_set.SymmetricExceptWith, other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            LockAct(_set.UnionWith, other);
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            LockAct(x => _set.Clear(), _set);
        }

        public bool Contains(T item)
        {
            return LockFun(() => _set.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            LockAct(x => _set.CopyTo(array, arrayIndex), _set);
        }

        public int Count
        {
            get { return _set.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return LockFun(() => _set.Remove(item));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return LockFun(() => _set.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
