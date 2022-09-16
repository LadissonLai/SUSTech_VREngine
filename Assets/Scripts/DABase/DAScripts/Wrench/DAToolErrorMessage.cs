using Doozy.Engine;
using static Fxb.DA.AbstractDAScript;

namespace Fxb.DA
{
    /// <summary>
    /// 拆装工具使用错误消息。 待从Wrench文件夹中移出
    /// </summary>
    public class DAToolErrorMessage : Message
    {
        public string tipInfo;

        public string daObjID;

        public DAAnimType daAnimType;

        public DAToolErrorMessage(string tipInfo, string daObjID, DAAnimType daAnimType)
        {
            this.tipInfo = tipInfo;

            this.daObjID = daObjID;

            this.daAnimType = daAnimType;
        }
    }
}
