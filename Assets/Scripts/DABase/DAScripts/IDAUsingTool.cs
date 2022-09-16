using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDAUsingTool
{

    /// <summary>
    /// 工具可否用来紧固  
    /// </summary>
    bool FixUsingAble { get; }

    bool IsUsing { get;}
}
