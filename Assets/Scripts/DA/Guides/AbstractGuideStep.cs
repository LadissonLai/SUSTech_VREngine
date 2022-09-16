using Doozy.Engine;
using Fxb.Localization;

namespace Fxb.CMSVR
{
    public abstract class GuideTipDataBase
    {
        public string param;
    }


    /// <summary>
    /// 单个指引步骤
    /// </summary>
    public abstract class AbstractGuideStep
    {
        private GuideTipMessage daTipMessageCache;

        public string TipInfo { get; private set; }

        public RecordStepType RecordType { get; private set; }

        public abstract bool IsCompleted { get; }

        public virtual void Setup(string tipInfo, RecordStepType type, string singleParam = null, string[] mutiPrams = null)
        {
            TipInfo = LocalizeMgr.Inst.GetLocalizedStr(tipInfo);

            RecordType = type;

            SendDAObjTipMessage(TipInfo);
        }

        public virtual void Clear()
        {
            SendDAObjTipMessage(null);
        }

        protected abstract void UpdateTipObjs();

        public virtual void Tick()
        {
            UpdateTipObjs();
        }

        protected void SendDAObjTipMessage(string info = null)
        {
            daTipMessageCache = daTipMessageCache ?? new GuideTipMessage();

            daTipMessageCache.tip = info;

            Message.Send(daTipMessageCache);
        }

        public virtual void Destroy() { }
    }

}