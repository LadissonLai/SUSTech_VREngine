using Fxb.CsvConfig;

namespace Fxb.CMSVR
{
    public class PropCsvConfig : DynamicCsvConfig<PropCsvConfig.Item>
    {
        public class Item
        {
            public string Id { get; private set; }

            public string Name { get; private set; }

            public string CustomClonePath { get; private set; }
        }

        public override bool CheckRowDataIngore(Item rowData)
        {
            //id项为空表示当前项暂不应用
            return string.IsNullOrEmpty(rowData.Id);
        }

        protected override int GetIndexVal(Item rData)
        {
            return rData.Id.GetHashCode();
        }
    }
}
