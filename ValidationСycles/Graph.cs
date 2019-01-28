using System.Collections.Generic;
using System.Linq;

namespace Inec.StateMachine.ValidationСycles
{
    /// <summary>
    /// представление графа в виде вершин и направленных дуг
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// дуги
        /// </summary>
        public GraphArcList ArcList { get; private set; }

        /// <summary>
        /// вершины
        /// </summary>
        public GraphVertexList VertexList { get; private set; }

        public Stack<VertexGraph> Stack { get; private set; }

        private Graph()
        {
            ArcList = new GraphArcList();
            VertexList = new GraphVertexList();
            Stack = new Stack<VertexGraph>();
        }

        /// <summary>
        /// создание графа для всех машин
        /// </summary>
        /// <returns></returns>
        public static Graph Create()
        {
            var res = new Graph();
            var list = new StateMachineDataList();
            foreach (var item in list) //дабавляем все вершины(машины состояний)
            {
                res.VertexList.Add(item);
            }
            foreach (VertexGraph vertex in res.VertexList)
            {
                IEnumerable<VertexGraph> slaves = GetSlavesFromMachine(res.VertexList, vertex);//находим всех потомков
                foreach (VertexGraph slave in slaves)
                {
                    var acr = new GraphArc(vertex, slave);
                    if (!res.ArcList.Contains(acr))//если нет такой дуги
                    {
                        res.ArcList.Add(acr); //добавляем дугу (путь от сработавшей машины vertex (slave), к машине, запук которой может вызвать vertex)
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// находим все циклы
        /// </summary>
        /// <param name="smd"></param>
        /// <returns></returns>
        public ResultCircumventionDepth GetCircumventionDepth(VertexGraph smd)
        {
            var res = new ResultCircumventionDepth();
            Bypass(res, smd);
            return res;
        }

        #region приватные служебные методы
        /// <summary>
        /// находит список всех подчиненых для данной машины
        /// </summary>
        /// <param name="graphVertexList"></param>
        /// <param name="smd"></param>
        /// <returns></returns>
        private static IEnumerable<VertexGraph> GetSlavesFromMachine(IEnumerable<VertexGraph> graphVertexList,
            StateMachineData smd)
        {
            var regId = GetRegId(smd);//регистр, на который повлияет срабатывание машины состояний(сам основной реестр или специальный реестр статусов)
            var vertexList = graphVertexList.Where(x => FilterAffecting(x, regId));//все машины, к запуску которых может привезти, запуск smd
            return vertexList;
        }
        /// <summary>
        /// обход в глубину и поиск циклов
        /// </summary>
        /// <param name="res"></param>
        /// <param name="stateMachineData"></param>
        private void Bypass(ResultCircumventionDepth res, VertexGraph stateMachineData)
        {
            //if (stateMachineData.IsVisited)//уже посещали
            //    return;
          //  stateMachineData.SetVisited();
            if (Stack.Contains(stateMachineData)) //уже содержиться в пути - цикл
            {
                Stack.Push(stateMachineData);
                res.AddCycle(Stack.ToArray());
                Stack.Pop();
                return;
            }
            Stack.Push(stateMachineData);
            var list = GetSlavesFromMachine(VertexList, stateMachineData);
            var vertexGraphs = list as VertexGraph[] ?? list.ToArray();
            if (!vertexGraphs.Any())// если нет дочерних, просто выходим - лист, тут еще можно можно вернуть полученную ветку
            {
                Stack.Pop();
                return;
            }
            foreach (var vertexGraph in vertexGraphs)//рекурсивно для дочерних
            {
                Bypass(res, vertexGraph);
            }
            Stack.Pop();
        }

        /// <summary>
        /// идентификатор реестра, на который влияет срабатывание машины
        /// </summary>
        /// <param name="smd"></param>
        /// <returns></returns>
        private static long GetRegId(VertexGraph smd)
        {
            return smd.Vertex.StatusesRegisterId ?? smd.Vertex.RegisterId;
        }

        /// <summary>
        /// фильтр для поиска зависимых реестров
        /// </summary>
        /// <param name="smd">вершина графа</param>
        /// <param name="regId">идентификатор реестра</param>
        /// <returns></returns>
        private static bool FilterAffecting(VertexGraph smd, long regId)
        {
            //если реестр, который измениться при срабатывании машины является главным реестром машины или содержиться в смежных
            return smd.Vertex.RegisterId == regId || smd.Vertex.ContiguousReesters.Contains(regId);
        }
        #endregion
    }
}
