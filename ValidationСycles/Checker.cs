using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectModel;

namespace Inec.StateMachine.ValidationСycles
{
    /// <summary>
    /// проверка машины состояний на циклы
    /// </summary>
    public class Checker
    {
        public static string CheckGraph(long stateMachineId)
        {
            var tmp = Check(stateMachineId);
            var res = CreateWarningString(tmp);
            return res;
        }

        /// <summary>
        /// поиск циклов
        /// </summary>
        /// <param name="stateMachineId">идентификатор машины</param>
        /// <returns>возможное сообщение об наличии циклов</returns>
        static ResultCircumventionDepth Check(long stateMachineId)
        {
            var graph = Graph.Create();//граф всех имеющихся машин
            var vertex = graph.VertexList.FirstOrDefault(x => x.Vertex.StateMachineId == stateMachineId);//вершина, из которой начинаем обход в глубину
            return vertex != null ? graph.GetCircumventionDepth(vertex) : new ResultCircumventionDepth();
        }

        public const string NoCycles = "Циклов не обнаружено";
        /// <summary>
        /// получение информации о наличии циклов
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        static string CreateWarningString(ResultCircumventionDepth result)
        {
            if (result == null || !result.Cycles.Any())
                return NoCycles;
            var sb = new StringBuilder();
            foreach (var cycle in result.Cycles)
            {
                GetStringFomCycle(cycle, sb);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static void GetStringFomCycle(IEnumerable<VertexGraph> list, StringBuilder sb)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (sb == null)
                throw new ArgumentNullException("sb");
            list.Aggregate(sb, (a, b) => a.Append(GetMachineName(b.Vertex.StateMachineId)).Append("-->"));
        }

        private static Func<long, string> GetMachineName = new Func<long, string>(_GetMachineName).MemoizeFun();
        static string _GetMachineName(long id)
        {
            var res =
            OMStateMachine.Where(x => x.Id == id)
                .Select(x => x.Name)
                .Execute()
                .Select(x => x.Name)
                .FirstOrDefault();
            return res;
        }
    }
}
