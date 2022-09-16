using Fxb.CsvConfig;
using UnityEngine;
using System.Collections.Generic;

namespace Fxb.CMSVR
{
    public class DACsvConfig : DynamicCsvConfig<DACsvConfig.Item>
    {
        private Dictionary<string, Item> modelNameDataMap;

        public class Item
        {
            public string Id { get; private set; }

            public string Name { get; private set; }
            public string PropID { get; private set; }
            public string ModelName { get; private set; }
            public string DependParts { get; private set; }
            public string DependSnapFits { get; private set; }
            public string SmallParts { get; private set; }
        }

        public override bool AddRow(Item rowData)
        {
            if( base.AddRow(rowData))
            {
                modelNameDataMap = modelNameDataMap ?? new Dictionary<string, Item>();

                Debug.Assert(!modelNameDataMap.ContainsKey(rowData.ModelName), "model name 重复:" + rowData.ModelName);

                modelNameDataMap.Add(rowData.ModelName, rowData);

                return true;
            }

            return false;
        }

        public override bool CheckRowDataIngore(Item rowData)
        {
            //id项为空表示当前项暂不应用
            return string.IsNullOrEmpty(rowData.Id) || string.IsNullOrEmpty(rowData.ModelName);
        }

        protected override int GetIndexVal(Item rData)
        {
            return rData.Id.GetHashCode();
        }

        public Item FindRDByModelName(string modelName)
        {
            if (modelNameDataMap.ContainsKey(modelName))
                return modelNameDataMap[modelName];

            return null;
        }
    }
}
