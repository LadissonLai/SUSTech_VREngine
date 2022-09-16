using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    public interface IWearable
    {
        EquipName EquipName { get; }

        void Wear(bool isOn);
    }
}
