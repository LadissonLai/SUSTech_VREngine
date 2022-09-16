using System.Collections.Generic;

namespace Fxb.CsvConfig
{
    public interface IConfigRowDataGenerat<T> where T : new()
    {
        T FromListDatas(IReadOnlyList<string> listData, Dictionary<string, int> titleMap);
    }
}
