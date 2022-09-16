using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.Highlighters;

namespace VRTKExtensions
{
    /// <summary>
    /// 循环高亮效果  
    /// </summary>
    public class RepeatPropertyBlockColorSwapHighlighter : VRTK_MaterialColorSwapHighlighter
    {
        /// <summary>
        /// 从暗到亮算一次
        /// </summary>
        [Tooltip("重复次数  从暗到亮算一次。 值为0则一直循环")]
        public int repeatCount;
         
        protected Dictionary<string, MaterialPropertyBlock[]> originalMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock[]>();

        protected Dictionary<string, MaterialPropertyBlock[]> highlightMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock[]>();

        public override void Initialise(Color? color = null, GameObject affectObject = null, Dictionary<string, object> options = null)
        {
            objectToAffect = (affectObject != null ? affectObject : gameObject);
            originalMaterialPropertyBlocks.Clear();
            highlightMaterialPropertyBlocks.Clear();
            // call to parent highlighter
            base.Initialise(color, affectObject, options);
        }

        protected override void StoreOriginalMaterials()
        {
            originalMaterialPropertyBlocks.Clear();

            Renderer[] renderers = objectToAffect.GetComponentsInChildren<Renderer>(false);

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                string objectReference = renderer.gameObject.GetInstanceID().ToString();

                var oPropertyBlocks = new MaterialPropertyBlock[renderer.sharedMaterials.Length];

                var hlPropertyBlocks = new MaterialPropertyBlock[renderer.sharedMaterials.Length];

                for (int j = 0, len = oPropertyBlocks.Length; j < len; j++)
                {
                    var originalPropertyBlock = new MaterialPropertyBlock();

                    renderer.GetPropertyBlock(originalPropertyBlock, j);

                    var hlPropertyBlock = new MaterialPropertyBlock();

                    renderer.GetPropertyBlock(hlPropertyBlock);

                    oPropertyBlocks[j] = originalPropertyBlock;

                    hlPropertyBlocks[j] = hlPropertyBlock;
                }

                VRTK_SharedMethods.AddDictionaryValue(originalMaterialPropertyBlocks, objectReference, oPropertyBlocks, true);

                VRTK_SharedMethods.AddDictionaryValue(highlightMaterialPropertyBlocks, objectReference, hlPropertyBlocks, true);
            }
        }

        public void SetPBSColor(Renderer renderer, MaterialPropertyBlock[] pbs, Color endColor)
        {
            for (int i = 0, len = renderer.sharedMaterials.Length; i < len; i++)
            {
                if (pbs.Length <= i)
                    return;

                pbs[i].SetColor("_Color", endColor);

                renderer.SetPropertyBlock(pbs[i], i);
            }
        }

        public IEnumerator CycleColor(Renderer renderer, MaterialPropertyBlock[] highlightMaterialPropertyBlock, Color endColor, float duration, float delayStart)
        {
            float elapsedTime = 0f;

            bool yoyoFlag = false;

            var startColors = new Color[renderer.sharedMaterials.Length];

            int repeatFlag = repeatCount > 0 ? repeatCount : int.MaxValue;

            for (int i = 0, len = startColors.Length; i < len; i++)
            {
                if (renderer.sharedMaterials[i].HasProperty("_Color"))
                    startColors[i] = renderer.sharedMaterials[i].GetColor("_Color");
            }

            if (delayStart > 0.0)
                yield return new WaitForSeconds(delayStart);

            while (true)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime > duration)
                {
                    if (--repeatFlag <= 0)
                    {
                        //循环完成
                        yield break;
                    }

                    elapsedTime %= duration;

                    yoyoFlag = !yoyoFlag;
                }

                float elapsedRate = elapsedTime / duration;

                elapsedRate = yoyoFlag ? 1 - elapsedRate : elapsedRate;

                {
                    for (int i = 0, len = highlightMaterialPropertyBlock.Length; i < len; i++)
                    {
                        highlightMaterialPropertyBlock[i].SetColor("_Color", Color.Lerp(startColors[i], endColor, elapsedRate));

                        highlightMaterialPropertyBlock[i].SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(endColor, 10));

                        renderer.SetPropertyBlock(highlightMaterialPropertyBlock[i], i);
                    }
                }

                yield return null;
            }
        }

        public override void Highlight(Color? color = null, float duration = 0)
        {
            if (color == null)
            {
                return;
            }

            ChangeToHighlightColor((Color)color, duration);
        }

        public void DelayHightlight(Color? color = null, float duration = 0, float delayStart = 0)
        {
            if (color == null)
            {
                return;
            }

            ChangeToHighlightColor((Color)color, duration, delayStart);
        }

        public override void Unhighlight(Color? color = null, float duration = 0)
        {
            if (objectToAffect == null)
            {
                return;
            }

            if (faderRoutines != null)
            {
                foreach (KeyValuePair<string, Coroutine> fadeRoutine in faderRoutines)
                {
                    if (fadeRoutine.Value != null)
                        StopCoroutine(fadeRoutine.Value);
                }
                faderRoutines.Clear();
            }

            Renderer[] renderers = objectToAffect.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                string objectReference = renderer.gameObject.GetInstanceID().ToString();

                var pbs = VRTK_SharedMethods.GetDictionaryValue(originalMaterialPropertyBlocks, objectReference);

                if (pbs == null)
                {
                    continue;
                }

                for (int j = 0; j < pbs.Length; j++)
                {
                    renderer.SetPropertyBlock(pbs[j], j);
                }
            }
        }

        protected void ChangeToHighlightColor(Color color, float duration = 0f, float delayStart = 0.0f)
        {
            Renderer[] renderers = objectToAffect.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];

                string faderRoutineID = renderer.gameObject.GetInstanceID().ToString();
                if (VRTK_SharedMethods.GetDictionaryValue(originalMaterialPropertyBlocks, faderRoutineID) == null)
                {
                    continue;
                }

                Coroutine existingFaderRoutine = VRTK_SharedMethods.GetDictionaryValue(faderRoutines, faderRoutineID);
                if (existingFaderRoutine != null)
                {
                    StopCoroutine(existingFaderRoutine);
                    faderRoutines.Remove(faderRoutineID);
                }

                var pbs = highlightMaterialPropertyBlocks[faderRoutineID];

                if (duration > 0f && renderer.gameObject.activeInHierarchy)
                {
                    VRTK_SharedMethods.AddDictionaryValue(faderRoutines, faderRoutineID, StartCoroutine(CycleColor(renderer, pbs, color, duration, delayStart)), true);
                }
                else
                {
                    SetPBSColor(renderer, pbs, color);
                }
            }
        }
    }
}
