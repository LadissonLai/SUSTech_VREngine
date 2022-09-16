using Fxb.CsvConfig;

namespace Fxb.CMSVR
{
    public class TaskStepGroupCsvConfig : DynamicCsvConfig<TaskStepGroupCsvConfig.Item>
    {
        public class Item
        {
            public string StepGroupID { get; private set; }
            public string StepIDs { get; private set; }
            public string Description { get; private set; }
            public float Weight { get; private set; }
        }

        public override bool CheckRowDataIngore(Item rowData)
        {
            return string.IsNullOrEmpty(rowData.StepGroupID);
        }

        protected override int GetIndexVal(Item rData)
        {
            return rData.StepGroupID.GetHashCode();
        }
    }
}
