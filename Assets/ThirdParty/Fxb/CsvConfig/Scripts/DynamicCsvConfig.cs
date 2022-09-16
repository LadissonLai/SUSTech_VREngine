using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Fxb.CsvConfig
{
    public class DynamicCsvConfig<T> where T : class, new()
    {
        public List<T> DataArray { get; private set; } = new List<T>();

        protected Dictionary<int, T> indexMap;

        public virtual IConfigRowDataGenerat<T> CreateRowDataGenerator()
        {
            return new RowDataGenerator<T>();
        }

        public virtual bool CheckRowDataIngore(T rowData)
        {
            return false;
        }

        public virtual bool AddRow(T rowData)
        {
            if (CheckRowDataIngore(rowData))
                return false;

            var indexVal = GetIndexVal(rowData);
 
            if (indexVal != -1)
            {
                if (indexMap == null)
                    indexMap = new Dictionary<int, T>();
                 
                Debug.Assert(!indexMap.ContainsKey(indexVal), $"第{indexMap.Count}索引内容重复:");

                indexMap.Add(indexVal, rowData);
            }

            DataArray.Add(rowData);

            return true;
        }
 
        protected virtual int GetIndexVal(T rData)
        {
            return -1;
        }
         
        /// <summary>
        /// 行数
        /// </summary>
        /// <value></value>
        public int RowCount
        {
            get
            {
                return DataArray.Count;
            }
        }
         
        /// <summary>
        /// 获取对应行的数据列表
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public virtual T GetRowDatas(int row)
        {
            if (row < 0 || row >= RowCount)
                return null;

            return DataArray[row];
        }

        public virtual T FindRowDatas(System.ValueType key)
        {
            return FindRowDatasByIndex(key.GetHashCode());
        }

        /// <summary>
        /// 通过索引项的内容获取对应行数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T FindRowDatas(string key)
        {
            return FindRowDatasByIndex(key.GetHashCode());
        }

        protected T FindRowDatasByIndex(int key)
        {
            if (indexMap == null)
            {
                Debug.LogWarning("未创建索引:" + GetType());

                return null;
            }

            if (indexMap.TryGetValue(key, out var rowData))
                return rowData;

            return null;
        }

        public virtual void Print()
        {
            foreach (var rData in DataArray)
            {
                Print(rData);
            }
        }

        protected virtual void Print(T rData)
        {
            var sb = new StringBuilder();

            var properties = typeof(T).GetProperties();

            foreach (var propInfo in properties)
            {
                sb.AppendFormat("{0}:{1}\t", propInfo.Name, propInfo.GetValue(rData));
            }

            Debug.Log(sb.ToString());
        }
    }
}