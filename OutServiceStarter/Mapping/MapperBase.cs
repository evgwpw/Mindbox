using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Data;
using System.Xml;

namespace Inec.StateMachine.OutServiceStarter.Mapping
{
    /// <summary>
    /// загружает данные полученные из RSM на результирующий DataSet
    /// </summary>
    public class MapperBase
    {
        #region поля
        DataSet _dataSet;
        bool IsDataSetLoad = false;
        static readonly char[] Separator = new char[] { '.' };
        #endregion
        #region свойства
        public string Resolution { get; private set; }
        public DataTable Data { get; private set; }
        /// <summary>
        /// загруженные данные
        /// </summary>
        public DataSet DataSet
        {
            get
            {
                if (!IsDataSetLoad)
                    throw new StateMachineException("Перед получением свойства DataSet необходимо вызвать метод Run");
                return _dataSet;
            }
        }
        #endregion
        public MapperBase(string resolution, DataTable data) 
        {
            if (string.IsNullOrEmpty(resolution))
                throw new ArgumentNullException("resolution");
            if (data == null)
                throw new ArgumentNullException("data");
            Resolution = resolution;
            Data = data;
        }
        /// <summary>
        /// проверка того, что псевдонимы столбцов соответсвуют таблицам и столбцам в датасете
        /// </summary>
        /// <returns></returns>
        protected void ValidateDataTable()
        {
            if (Data.Rows.Count < 1)
                throw new StateMachineException("Таблица не содержит данных");
            var columns = Data.Columns.Cast<DataColumn>()
                    .Select(x => x.ColumnName)
                    .Where(x => !string.IsNullOrEmpty(x) && x.Contains('.'))
                    .Select(x => x.Split(Separator))
                    .Select(x => new { Table = x[0], Column = x[1] });
            foreach (var tab in columns)
            {
                if (!_dataSet.Tables.Contains(tab.Table))
                {
                    throw new Exception("В результирующем наборе данных содержится не опознанная таблица " + tab.Table);
                }
                var tbl = _dataSet.Tables[tab.Table];
                if (!tbl.Columns.Contains(tab.Column))
                {
                    throw new Exception(String.Format("В результирующем набборе данных в таблице {0} содержится не опознанная колонка {1}", tab.Table, tab.Column));
                }
            }
        }

        public virtual MapperBase Run()
        {
            Load();
            ValidateDataTable();
            Map();
            IsDataSetLoad = true;
            return this;
        }

        protected virtual void Map()
        {
            try
            {
                var columns = Data.Columns.Cast<DataColumn>()
                    .Select(x => x.ColumnName)
                    .Where(x => !string.IsNullOrEmpty(x) && x.Contains('.'))
                    .Select(x => x.Split(Separator))
                    .Select(x => new { Table = x[0], Column = x[1] });
                foreach (var tab in columns)
                {
                    DataRow row;
                    var tbl = _dataSet.Tables[tab.Table];
                    if (tbl.Rows.Count < 1)
                    {
                        row = tbl.NewRow();
                        tbl.Rows.Add(row);
                    }
                    row = tbl.Rows[0];
                    var alias = tab.Table + "." + tab.Column;
                    row[tab.Column] = Data.Rows[0][alias];
                }
                
            }
            catch (Exception exc)
            {
                Core.ErrorManagment.ErrorManager.LogErrorInWinSrvEnv(exc);
            }
        }
        /// <summary>
        /// грузим датасет из файла
        /// </summary>
        protected virtual void Load()
        {
            string fileName = Path.GetFileNameWithoutExtension(Resolution);
            _dataSet = Core.ConfigParam.Configuration.GetParam<DataSet>(fileName, @"\SPDDataSets\");
        }
    }
}
