using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CsvConfig
{
    public static class CsvSerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="TConfig">配置</typeparam>
        /// <typeparam name="TItem">配置项</typeparam>
        /// <param name="csvStr">csv文本</param>
        /// <param name="startRow">开始行</param>
        /// <returns></returns>
        public static TConfig Serialize<TConfig, TItem>
            (string csvStr, int startRow = 0)
            where TItem : class, new() where TConfig : DynamicCsvConfig<TItem>
        {
            return Serialize<TConfig, TItem>(CsvParser.Parse(csvStr), startRow);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="TConfig">配置</typeparam>
        /// <typeparam name="TItem">配置项</typeparam>
        /// <param name="jsonData">json str list</param>
        /// <param name="startRow">开始行</param>
        /// <returns></returns>
        public static TConfig Serialize<TConfig,TItem>
            (IReadOnlyList<IReadOnlyList<string>> jsonData, int startRow = 0)
            where TItem : class, new() where TConfig : DynamicCsvConfig<TItem>
        {
            Debug.Assert(jsonData?.Count > 2);

            var titleData = jsonData[startRow];

            var titleMap = new Dictionary<string, int>();

            var i = 0;

            var len = 0;

            for (len = titleData.Count; i < len; i++)
            {
                if (!string.IsNullOrEmpty(titleData[i]))
                    titleMap.Add(titleData[i], i);
            }

            var config = Activator.CreateInstance<TConfig>();

            var rowDataGenerator = config.CreateRowDataGenerator();
             
            for (i = startRow + 1, len = jsonData.Count; i < len; i++)
            {
                var rowData = rowDataGenerator.FromListDatas(jsonData[i], titleMap);
 
                config.AddRow(rowData);
            }

            return config;
        }
    }
}
