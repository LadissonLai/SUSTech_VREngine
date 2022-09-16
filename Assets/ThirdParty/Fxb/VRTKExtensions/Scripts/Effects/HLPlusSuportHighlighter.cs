using HighlightPlus;
using System;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Highlighters;

namespace VRTKExtensions
{
    public class HLPlusSuportHighlighter : VRTK_BaseHighlighter
    {
        public HighlightEffect highlightPlusEffect;

        public Color? defaultHLColor;

        [Tooltip("自定义的目标父物体")]
        public Transform[] customRoots;

        //[Tooltip("")]
        //public Renderer[] customRenderers;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (highlightPlusEffect == null)
                highlightPlusEffect = GetComponent<HighlightEffect>();

            if (customRoots == null || customRoots.Length == 0)
            {
                if(highlightPlusEffect != null)
                    highlightPlusEffect.SetTargetRenderers(null);

                return;
            }

            var subRenderers = new List<Renderer>();

            foreach (var root in customRoots)
            {
                if (root == null)
                    continue;

                subRenderers.AddRange(root.GetComponentsInChildren<Renderer>());
            }

            //Debug.Log(subRenderers.Count);
 
            if(highlightPlusEffect != null)
            {
                if (subRenderers.Count > 0)
                    highlightPlusEffect.SetTargetRenderers(subRenderers.ToArray());
                else
                    highlightPlusEffect.SetTargetRenderers(null);
            }

            //Debug.Log("HLPlusSuportHighlighter awake.  " + name);
        }
 
        public override void Highlight(Color? color = null, float duration = 0)
        {
            if (color == null)
                color = defaultHLColor;

            if (color != null)
            {
                highlightPlusEffect.outlineColor = color.Value;

                highlightPlusEffect.SetHighlighted(true);

                active = true;
            }
        }

        public override void Initialise(Color? color = null, GameObject affectObject = null, Dictionary<string, object> options = null)
        {
            defaultHLColor = color;

            highlightPlusEffect = highlightPlusEffect ?? GetComponent<HighlightPlus.HighlightEffect>();

            usesClonedObject = false;

            ResetHighlighter();
        }

        public override void ResetHighlighter()
        {
            highlightPlusEffect.enabled = true;

            active = false;
        }

        public override void Unhighlight(Color? color = null, float duration = 0)
        {
            if(highlightPlusEffect != null)
                highlightPlusEffect.SetHighlighted(false);

            active = false;
        }

        private void OnDestroy()
        {
            if (highlightPlusEffect != null)
                Destroy(highlightPlusEffect);
        }
    }
}
