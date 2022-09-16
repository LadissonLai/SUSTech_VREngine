using Fxb.CsvConfig;

namespace Fxb.CMSVR
{
    public class TaskCsvConfig : DynamicCsvConfig<TaskCsvConfig.Item>
    {
        public class Item
        {
            public string ID { get; private set; }
            public string Title { get; private set; }
            public string Description { get; private set; }
            public string Targets { get; private set; }
            public string StepGroupID { get; private set; }
            public float Weight { get; private set; }
            public string ObfuscatedID { get; private set; }
            public string Type { get; private set; }
            public string System { get; private set; }

            public string Icon { get; private set; }

            public float Level { get; private set; }
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
