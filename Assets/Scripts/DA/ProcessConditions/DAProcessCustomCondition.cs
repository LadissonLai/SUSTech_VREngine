using Fxb.DA;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 自定义的拆装条件
    /// </summary>
    public interface IDAProcessAssertAble
    {
        DAProcessTarget ProcessTarget { get; }
 
        bool Check(DAObjCtr daObjCtr, out string errorMsg);
    }
}

