using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Inec.StateMachine.Storage
{
    public class Cache
    {
        static Entity TmpEmtity = new Entity();
        Timer _timer;
        class Entity : IEquatable<Entity>
        {
            static object Locker = new object();
            static EntityComparer _Comparer;
            static EntityComparer Comparer
            {
                get
                {
                    if (_Comparer == null)
                    {
                        lock (Locker)
                        {
                            if (_Comparer == null)
                            {
                                _Comparer = new EntityComparer();
                            }
                        }
                    }
                    return _Comparer;
                }
            }
            public int EmpId { get; set; }
            public DateTime Expire { get; set; }


            public bool Equals(Entity x, Entity y)
            {
                return Comparer.Equals(x, y);
            }

            public int GetHashCode(Entity obj)
            {
                return Comparer.GetHashCode(this);
            }
            public override bool Equals(object obj)
            {
                if (obj is Entity)
                {
                    var tmp = (Entity)obj;
                    return Equals(this, tmp);
                }
                return false;
            }
            public override int GetHashCode()
            {
                return Comparer.GetHashCode(this);
            }

            public bool Equals(Entity other)
            {
                if (other == null)
                    return false;
                return this.EmpId == other.EmpId;
            }
        }
        class EntityComparer : IEqualityComparer<Entity>
        {
            public bool Equals(Entity x, Entity y)
            {
                if (x == null && y != null)
                    return false;
                if (x != null && y == null)
                    return false;
                return x.EmpId == y.EmpId;
            }

            public int GetHashCode(Entity obj)
            {
                if (obj == null)
                    return 0;
                return obj.EmpId ^ Int32.MaxValue;
            }
        }
        SpinLock sLock = new SpinLock();
        LinkedList<Entity> hashSet = new LinkedList<Entity>();// (new EntityComparer());
        public int ExpireTime { get; private set; }
        
        /// <summary>
        /// врумя в секундах, через которое устаревают значения
        /// </summary>
        /// <param name="expireTime">время, на которое ложим в кеш</param>
        public Cache(int expireTime)
        {
            ExpireTime = expireTime;
        }
        void ClearCache()
        {
            
            bool gotLock = false;
            try
            {
                sLock.Enter(ref gotLock);
                DateTime dt = DateTime.Now;
                var tmp = hashSet.Where(e => e.Expire <= dt).ToList();
                foreach (var l in tmp)
                {
                    hashSet.Remove(l);
                }
            }
            finally
            {
                if (gotLock)
                    sLock.Exit();
            }
        }
     

        public bool Constraints(int empId)
        {
            bool gotLock = false;
            try
            {
                sLock.Enter(ref gotLock);
                TmpEmtity.EmpId = empId;
                return hashSet.Contains(TmpEmtity);
            }
            finally
            {
                if (gotLock)
                    sLock.Exit();
            }
        }
        /// <summary>
        /// Добавляем, если не существует
        /// </summary>
        /// <param name="empId">идентификатор объекта</param>
        /// <returns>true - добавили новый, false - уже существовал</returns>
        public bool AddIfNotExists(int empId)
        {
            bool gotLock = false;
            try
            {
                sLock.Enter(ref gotLock);
                var dt = DateTime.Now;
                //ищем значение с данныи идентификатором и еще не устаревшее
                if (!hashSet.Any(e => e.EmpId == empId && e.Expire <= dt))
                {
                    var tmp = new Entity() { EmpId = empId, Expire = dt.AddSeconds(ExpireTime) };
                    hashSet.AddFirst(tmp);
                    return true;
                }
                return false;
            }
            finally
            {
                if (gotLock)
                    sLock.Exit();
            }
        }
    }

}
