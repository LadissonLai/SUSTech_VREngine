using Fxb.CsvConfig;

namespace Fxb.CMSVR
{
    public class RecordCsvConfig : DynamicCsvConfig<RecordCsvConfig.Item>
    {
        public class Item
        {
            public string ID { get; private set; }

            public string Title { get; private set; }

            public RecordStepType Type { get; private set; }

            public string Params { get; private set; }
        }

        public override bool CheckRowDataIngore(Item rowData)
        {
            //id项为空表示当前项暂不应用
            return string.IsNullOrEmpty(rowData.ID);
        }

        protected override int GetIndexVal(Item rData)
        {
            return rData.ID.GetHashCode();
        }
    }

    public class RecordErrorCsvConfig : DynamicCsvConfig<RecordErrorCsvConfig.Item>
    {
        public class Item
        {
            public string ID { get; private set; }

            public string Title { get; private set; }

            public float Score { get; private set; }
        }

        public enum Key
        {
            ID, Title, Score
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
