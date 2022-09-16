using Fxb.DA;
using System;
using System.Collections.Generic;
using UnityEngine;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    public class VRDAClaspCtr : DAObjCtr
    {
        public override CmsObjType Type => CmsObjType.SnapFit;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (TryGetComponent<PanelSpawnTooltipTrigger>(out var tooltipTrigger))
            {
                if (string.IsNullOrEmpty(tooltipTrigger.customTooltipSpawnKey))
                    tooltipTrigger.customTooltipSpawnKey = PathConfig.PREFAB_DEFAULT_TOOLTIP_PLANE_S;
            }
        }

        public override List<AbstractDAObjCtr> DependParts
        {
            get
            {
                #region UNITY_EDITOR

                Debug.Assert(base.DependParts == null || base.DependParts.Count == 0, "螺丝卡扣暂不支持依赖 " + name);

                #endregion

                return null;
            }
        }

        public override List<AbstractDAObjCtr> DependSnapFits
        {
            get
            {
                #region UNITY_EDITOR

                Debug.Assert(base.DependParts == null || base.DependParts.Count == 0, "螺丝卡扣暂不支持依赖" + name);

                #endregion

                return null;
            }
        }
    }
}
