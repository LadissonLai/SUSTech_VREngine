using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fxb.CMSVR
{
    public interface IExternalDevicesConnectAble
    {
        Transform DeviceConnect { get; }
    }
}
