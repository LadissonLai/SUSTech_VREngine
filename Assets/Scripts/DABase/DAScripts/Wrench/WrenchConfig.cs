using Fxb.CsvConfig;
using Fxb.DA;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.DA
{
    public class WrenchConfig : DynamicCsvConfig<WrenchConfig.Item>
    {
        private Dictionary<string, Item> modelNameDataMap;
 
        public class Item
        {
            public string Id { get; private set; }

            public string Name { get; private set; }

            public WrenchPartsType Type { get; private set; }

            public string[] KitArray { get; private set; }
            
            public string Kit
            {
                get
                {
                    throw new Exception("使用KitArray");
                }
                private set
                {
                    if(!string.IsNullOrEmpty(value))
                    {
                        KitArray = value.Split(',');
                    }
                }
            }
            
            public string ImagePath { get; private set; }

            public string PrefabName { get; private set; }
        }

        public override bool AddRow(Item rowData)
        {
            if (base.AddRow(rowData))
            {
                modelNameDataMap = modelNameDataMap ?? new Dictionary<string, Item>();

                Debug.Assert(!modelNameDataMap.ContainsKey(rowData.PrefabName), "PrefabPath 重复:" + rowData.PrefabName);

                modelNameDataMap.Add(rowData.PrefabName, rowData);
            }

            return false;
        }

        protected override int GetIndexVal(Item rData)
        {
            return rData.Id.GetHashCode();
        }

        public Item FindRDByModelName(string prefabName)
        {
            if (modelNameDataMap.ContainsKey(prefabName))
                return modelNameDataMap[prefabName];

            return null;
        }
    }
}
