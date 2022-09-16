using Fxb.CsvConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class SoftwareCsvConfig : DynamicCsvConfig<SoftwareCsvConfig.Item>
    {
        public class Item
        {
            public string ID { get; private set; }
            public string Channel { get; private set; }
            public string ProductDate { get; private set; }
            public string Framework { get; private set; }
            public string Color { get; private set; }
            public string Version { get; private set; }
            public string SoftwareName { get; private set; }
            public string Miles { get; private set; }
            public string Icon { get; private set; }
            public string PropObfuscated { get; private set; }
            public string Demo { get; private set; }
            public string InfoTitle { get; private set; }
            public string Information { get; private set; }

        }

        public override bool CheckRowDataIngore(Item rowData)
        {
            return string.IsNullOrEmpty(rowData.ID);
        }

        protected override int GetIndexVal(Item rData)
        {
            return rData.ID.GetHashCode();
        }
    }

}