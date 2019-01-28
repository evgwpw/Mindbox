using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Register.QuerySubsystem;
using System.Linq.Expressions;
using Core.ObjectModel.CustomAttribute;

namespace Inec.StateMachine
{
    /// <summary>
    /// различные методы расширения
    /// </summary>
    public static class StateMachineExtensions
    {
        /// <summary>
        /// копируем элементы из последовательности в список
        /// </summary>
        /// <typeparam name="T">тип элемента</typeparam>
        /// <param name="list">список</param>
        /// <param name="src">последовательность</param>
        public static void CopyEnunerableToList<T>(this IList<T> list, IEnumerable<T> src, Func<T, bool> check = null)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (src == null)
                throw new ArgumentNullException("src");
            if (list.IsReadOnly)
                throw new StateMachineException("Не возможно добавить элементы в список, так как он ReadOnly");
            foreach (var e in src)
            {
                if (check == null || check(e))
                {
                    list.Add(e);
                }
            }
        }
        /// <summary>
        /// выводим первые 10 элементов последовательности
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string EnumerableOfTToString<T>(this IEnumerable<T> src)
        {
            if (src == null)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (var t in src.Take(10))
            {
                sb.AppendLine(Convert.ToString(t));
            }
            return sb.ToString();
        }
        /// <summary>
        /// возвращает стандартный хеш код для строки и если она null->0 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetStringHashCode(this string str)
        {
            if (str == null)
                return 0;
            return str.GetHashCode();
        }
        ////public static QSQuery<TSource> SelectMulty<TSource, TResult>(this QSQuery<TSource> src, Expression<Func<TSource, TResult>> property) where TSource:class
        //{
        //    if (property.Body.NodeType != ExpressionType.MemberAccess &&
        //        property.Body.NodeType != ExpressionType.Call &&
        //        property.Body.NodeType!= ExpressionType.New)
        //        throw new InvalidOperationException("Возможно только свойство реестра");
        //    var t = property.Body.NodeType;

        //    if (property.Body is MemberExpression)
        //    {
        //        MemberExpression member = property.Body as MemberExpression;
        //        src.AddColumn(GetSimpleColumn(member));
        //    }
        //    else if (property.Body is MethodCallExpression)
        //    {
        //        var exp = property.Body as MethodCallExpression;
        //        if (exp.Arguments.Count > 1 && exp.Arguments[1] is LambdaExpression)
        //        {
        //            MemberExpression member = (exp.Arguments[1] as LambdaExpression).Body as MemberExpression;
        //            src.AddColumn(GetSimpleColumn(member));
        //        }
        //    }
        //    else if (property.Body is NewExpression)
        //    {
        //        var exp = property.Body as NewExpression;
        //        foreach (var a in exp.Arguments.OfType<MemberExpression>())
        //        {
        //            src.AddColumn(GetSimpleColumn(a));
        //        }
        //    }
        //    return src;
        //}
        private static QSColumnSimple GetSimpleColumn(MemberExpression member)
        {
            var field = member.Member.Name;
            var customAttributesData = member.Member.GetCustomAttributesData().FirstOrDefault(x => x.Constructor.DeclaringType == typeof(RegisterAttributeAttribute) ||
                                                                                                   x.Constructor.DeclaringType == typeof(PrimaryKeyAttribute));
            var attributeID = (int)customAttributesData.NamedArguments[0].TypedValue.Value;

            var columnType = QSColumnSimpleType.Value;

            if (field.EndsWith("_Code"))
                columnType = QSColumnSimpleType.Code;

            return GetQSColumnSimple(attributeID, attributeID.ToString(), columnType);
        }

        private static QSColumnSimple GetQSColumnSimple(int attributeID, string alias, QSColumnSimpleType type)
        {
            return new QSColumnSimple() { AttributeID = attributeID, Alias = alias, Type = type };
        }

    }
}
