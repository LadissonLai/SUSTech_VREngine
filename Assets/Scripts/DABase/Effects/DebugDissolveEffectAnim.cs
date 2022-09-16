using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试用的消融效果脚本，有性能上的问题，暂时用来看效果
/// </summary>
public class DebugDissolveEffectAnim : MonoBehaviour
{
    public float progress;
 
    private List<Material> allMats;

    private Shader dissolveShader;

    private int progressID;

    void Start()
    {
        var templeteMat = Resources.Load<Material>("Mats/DefaultPBRDissolve");

        dissolveShader = Shader.Find("Fxb/Dissolve/DefaultPBRDissolve");
 
        progressID = Shader.PropertyToID("_Cutoff");

        allMats = new List<Material>();

        var allRenderers = GetComponentsInChildren<Renderer>();

        foreach (var r in allRenderers)
        {
            allMats.AddRange(r.materials);
        }
 
        foreach (var m in allMats)
        {
            if (m.shader.name != dissolveShader.name)
                m.shader = dissolveShader;
            
            if(m.GetTexture("_NoiseTex") == null)
                m.SetTexture("_NoiseTex", templeteMat.GetTexture("_NoiseTex"));

            m.SetFloat(progressID, progress);
        }
    }

    private void Update()
    {
        if (allMats.Count == 0)
            return;
  
        progress = Mathf.Clamp(progress, 0,1);
 
        foreach (var m in allMats)
        {
            m.SetFloat(progressID, progress);
        }
    }
}
