using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fxb.CMSVR
{
    public interface IRecordModel
    {
        /// <summary>
        /// 记录某项内容
        /// </summary>
        /// <param name="type">记录项类型</param>
        /// <param name="args">相关参数 有可能有多条</param>
        /// <returns>是否成功添加一条记录</returns>
        bool Record(RecordStepType type, params string[] args);

        bool Record(RecordStepType type, string arg);

        /// <summary>
        /// 打印当前的所有记录项内容，可以用来测试数据
        /// </summary>
        void PrintRecordList();

        bool CheckRecordCompleted(string recordID);

        bool RecordError(RecordStepType type, string arg, ErrorRecordType errorType);

        void SetRecordCompleted(string recordID);

        RecordCsvConfig.Item FindRecord(string id);

        float GetRecordErrorScoreDeducting(string recordID);

        string GetRecordAllErrors(string recordID);

        void ClearRecord();
    }
}
