using System;
using System.Collections.Concurrent;

// ReSharper disable once CheckNamespace
namespace Inec.StateMachine
{
    /// <summary>
    /// многопоточная мемоизация
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class UniversalCache<TKey, TValue>
    {
        readonly ConcurrentDictionary<TKey, TValue> _cache;
        public Func<TKey, TValue> GetValueFunc { get; private set; }
        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="getValueFunc">функция, позволяющая получить значение по ключу</param>
        public UniversalCache(Func<TKey, TValue> getValueFunc)
        {
            if (getValueFunc == null) 
                throw new ArgumentNullException("getValueFunc");
            GetValueFunc = getValueFunc;
            _cache = new ConcurrentDictionary<TKey, TValue>();

        }
        /// <summary>
        /// получает значение из кеша
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue GetValue(TKey key)
        {
            TValue res;
            if (_cache.TryGetValue(key, out res))
                return res;
            res = GetValueFunc(key);
            _cache.TryAdd(key, res);
            return res;
        }
    }
    /// <summary>
    /// кеш для функций в которых для параметраметра не переопределен Equals
    /// </summary>
    /// <typeparam name="TPar">тип параметра</typeparam>
    /// <typeparam name="TKey">тип ключа</typeparam>
    /// <typeparam name="TValue">тип результата</typeparam>
    public class UniversalCache<TPar, TKey, TValue>
    {
        readonly ConcurrentDictionary<TKey, TValue> _cache;
        public Func<TPar, TValue> GetValueFunc { get; private set; }
        public Func<TPar, TKey> GetKeyFunc { get; private set; }
        public UniversalCache(Func<TPar, TValue> getValueFunc, Func<TPar, TKey> getKeyFunc)
        {
            if (getValueFunc == null)
                throw new ArgumentNullException("getValueFunc");
            if (getKeyFunc == null)
                throw new ArgumentNullException("getKeyFunc");
            GetValueFunc = getValueFunc;
            GetKeyFunc = getKeyFunc;
            _cache = new ConcurrentDictionary<TKey, TValue>();
        }

        /// <summary>
        /// получает значение из кеша
        /// </summary>
        /// <returns></returns>
        public TValue GetValue(TPar par)
        {
            TValue res;
            TKey key = GetKeyFunc(par);
            if (_cache.TryGetValue(key, out res))
                return res;
            res = GetValueFunc(par);
            //res = 
            _cache.TryAdd(key, res);
            return res;
        }
    }
    public class UniversalCache2<TP1, TP2, TValue>
    {
        readonly ConcurrentDictionary<Tuple<TP1,TP2>, TValue> _cache;
        public Func<TP1,TP2, TValue> GetValueFunc { get; private set; }
        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="getValueFunc">функция, позволяющая получить значение по ключу</param>
        public UniversalCache2(Func<TP1,TP2, TValue> getValueFunc)
        {
            if (getValueFunc == null)
                throw new ArgumentNullException("getValueFunc");
            GetValueFunc = getValueFunc;
            _cache = new ConcurrentDictionary<Tuple<TP1, TP2>, TValue>();

        }

        /// <summary>
        /// получает значение из кеша
        /// </summary>
        /// <returns></returns>
        public TValue GetValue(TP1 p1, TP2 p2)
        {
            var t = Tuple.Create(p1, p2);
            TValue res;
            if (_cache.TryGetValue(t, out res))
                return res;
            res = GetValueFunc(p1, p2);
            _cache.TryAdd(t, res);
            return res;
        }
    }
    public class UniversalCache3<TP1, TP2, TP3, TValue>
    {
        readonly ConcurrentDictionary<Tuple<TP1, TP2, TP3>, TValue> _cache;
        public Func<TP1, TP2, TP3, TValue> GetValueFunc { get; private set; }
        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="getValueFunc">функция, позволяющая получить значение по ключу</param>
        public UniversalCache3(Func<TP1, TP2, TP3, TValue> getValueFunc)
        {
            if (getValueFunc == null)
                throw new ArgumentNullException("getValueFunc");
            GetValueFunc = getValueFunc;
            _cache = new ConcurrentDictionary<Tuple<TP1, TP2, TP3>, TValue>();

        }

        /// <summary>
        /// получает значение из кеша
        /// </summary>
        /// <returns></returns>
        public TValue GetValue(TP1 p1, TP2 p2, TP3 p3)
        {
            var t = Tuple.Create(p1, p2, p3);
            TValue res;
            if (_cache.TryGetValue(t, out res))
                return res;
            res = GetValueFunc(p1, p2, p3);
            _cache.TryAdd(t, res);
            return res;
        }
    }
    public static class MemoizeClass
    {
        /// <summary>
        /// мемоизация функции с одним параметром
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="firstFun"></param>
        /// <returns></returns>
        public static Func<TParam, TResult> MemoizeFun<TParam, TResult>(this Func<TParam, TResult> firstFun)
        {
            var cache = new UniversalCache<TParam, TResult>(firstFun);
            return cache.GetValue;
        }

        /// <summary>
        /// мемоизация функции с одним параметром
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="firstFun"></param>
        /// <param name="keyFun"></param>
        /// <returns></returns>
        public static Func<TParam, TResult> MemoizeFun<TParam, TKey, TResult>(this Func<TParam, TResult> firstFun, Func<TParam, TKey> keyFun)
        {
            var cache = new UniversalCache<TParam, TKey, TResult>(firstFun, keyFun);
            return cache.GetValue;
        }
        /// <summary>
        /// мемоизация функции с 2 параметрами
        /// </summary>
        /// <typeparam name="TParam1"></typeparam>
        /// <typeparam name="TParam2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="firstFun"></param>
        /// <returns></returns>
        public static Func<TParam1, TParam2, TResult> MemoizeFun<TParam1, TParam2, TResult>(this Func<TParam1, TParam2, TResult> firstFun)
        {
            var cache = new UniversalCache2<TParam1, TParam2, TResult>(firstFun);
            return cache.GetValue;
        }
        /// <summary>
        /// мемоизация функции с 3 параметрами
        /// </summary>
        /// <typeparam name="TParam1"></typeparam>
        /// <typeparam name="TParam2"></typeparam>
        /// <typeparam name="TParam3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="firstFun"></param>
        /// <returns></returns>
        public static Func<TParam1, TParam2, TParam3, TResult> MemoizeFun<TParam1, TParam2, TParam3, TResult>(this Func<TParam1, TParam2, TParam3, TResult> firstFun)
        {
            var cache = new UniversalCache3<TParam1, TParam2, TParam3, TResult>(firstFun);
            return cache.GetValue;
        }
    }
}
