// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using DG.DOTweenEditor;
using DG.Tweening;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Animation;
using UnityEngine;
using DelayedCall = Doozy.Editor.Utils.DelayedCall;

namespace Doozy.Editor.UI.Animation
{
    public static class UIAnimatorUtils
    {
        public static bool PreviewIsPlaying { get { return s_previewIsPlaying; } }

        private static DelayedCall s_delayedCall;
        private static bool s_previewIsPlaying;

        private static Vector3 s_startPosition;
        private static Vector3 s_startRotation;
        private static Vector3 s_startScale;
        private static float s_startAlpha;

        public static void StopAllAnimations(RectTransform target)
        {
            foreach (AnimationType value in Enum.GetValues(typeof(AnimationType)))
                UIAnimator.StopAnimations(target, value);
        }

        public static void PreviewViewAnimation(UIView view, UIAnimation animation)
        {
            if (s_previewIsPlaying) return;
            if (s_delayedCall != null) s_delayedCall.Cancel();
            view.UpdateStartValues();
            StopViewPreview(view);
            StopAllAnimations(view.RectTransform);

            Vector3 moveFrom = UIAnimator.GetAnimationMoveFrom(view.RectTransform, animation, view.CurrentStartPosition);
            Vector3 moveTo = UIAnimator.GetAnimationMoveTo(view.RectTransform, animation, view.CurrentStartPosition);
            if (!animation.Move.Enabled) view.ResetPosition();
            else PreviewMove(view.RectTransform, moveFrom, moveTo, animation, view.CurrentStartPosition);

            Vector3 rotateFrom = UIAnimator.GetAnimationRotateFrom(animation, view.StartRotation);
            Vector3 rotateTo = UIAnimator.GetAnimationRotateTo(animation, view.StartRotation);
            if (!animation.Rotate.Enabled) view.ResetRotation();
            else PreviewRotate(view.RectTransform, rotateFrom, rotateTo, animation, view.StartRotation);

            Vector3 scaleFrom = UIAnimator.GetAnimationScaleFrom(animation, view.StartScale);
            Vector3 scaleTo = UIAnimator.GetAnimationScaleTo(animation, view.StartScale);
            if (!animation.Scale.Enabled) view.ResetScale();
            else PreviewScale(view.RectTransform, scaleFrom, scaleTo, animation, view.StartScale);

            float fadeFrom = UIAnimator.GetAnimationFadeFrom(animation, view.StartAlpha);
            float fadeTo = UIAnimator.GetAnimationFadeTo(animation, view.StartAlpha);
            if (!animation.Fade.Enabled) view.ResetAlpha();
            else PreviewFade(view.RectTransform, fadeFrom, fadeTo, animation, view.StartAlpha);

            DOTweenEditorPreview.Start();
            s_previewIsPlaying = true;

            s_delayedCall = new DelayedCall((animation.AnimationType == AnimationType.Loop ? 5f : animation.TotalDuration + (animation.AnimationType == AnimationType.Hide ? 0.5f : 0f)), () =>
            {
                StopViewPreview(view);
                s_delayedCall = null;
            });
        }

        public static void PreviewPopupAnimation(UIPopup popup, UIAnimation animation)
        {
            if (s_previewIsPlaying) return;
            if (s_delayedCall != null) s_delayedCall.Cancel();
            popup.UpdateStartValues();
            StopPopupPreview(popup);
            StopAllAnimations(popup.RectTransform);
            if (popup.HasContainer)
            {
                popup.Container.UpdateStartValues();
                StopAllAnimations(popup.Container.RectTransform);
            }

            if (popup.HasOverlay)
            {
                popup.Overlay.UpdateStartValues();
                StopAllAnimations(popup.RectTransform);
            }

            Vector3 moveFrom = UIAnimator.GetAnimationMoveFrom(popup.Container.RectTransform, animation, popup.Container.StartPosition);
            Vector3 moveTo = UIAnimator.GetAnimationMoveTo(popup.Container.RectTransform, animation, popup.Container.StartPosition);
            if (!animation.Move.Enabled) popup.Container.ResetPosition();
            else PreviewMove(popup.Container.RectTransform, moveFrom, moveTo, animation, popup.Container.StartPosition);

            Vector3 rotateFrom = UIAnimator.GetAnimationRotateFrom(animation, popup.Container.StartRotation);
            Vector3 rotateTo = UIAnimator.GetAnimationRotateTo(animation, popup.Container.StartRotation);
            if (!animation.Rotate.Enabled) popup.Container.ResetRotation();
            else PreviewRotate(popup.Container.RectTransform, rotateFrom, rotateTo, animation, popup.Container.StartRotation);

            Vector3 scaleFrom = UIAnimator.GetAnimationScaleFrom(animation, popup.Container.StartScale);
            Vector3 scaleTo = UIAnimator.GetAnimationScaleTo(animation, popup.Container.StartScale);
            if (!animation.Scale.Enabled) popup.Container.ResetScale();
            else PreviewScale(popup.Container.RectTransform, scaleFrom, scaleTo, animation, popup.Container.StartScale);

            float fadeFrom = UIAnimator.GetAnimationFadeFrom(animation, popup.Container.StartAlpha);
            float fadeTo = UIAnimator.GetAnimationFadeTo(animation, popup.Container.StartAlpha);
            if (!animation.Fade.Enabled) popup.Container.ResetAlpha();
            else PreviewFade(popup.Container.RectTransform, fadeFrom, fadeTo, animation, popup.Container.StartAlpha);

            if (animation.Enabled && popup.HasOverlay && popup.Overlay.Enabled)
            {
                float overlayFadeFrom = animation.AnimationType == AnimationType.Show ? 0 : 1;
                float overlayFadeTo = animation.AnimationType == AnimationType.Show ? 1 : 0;
                popup.Overlay.CanvasGroup.alpha = overlayFadeFrom;
                DOTweenEditorPreview.PrepareTweenForPreview(popup.Overlay.CanvasGroup.DOFade(overlayFadeTo, animation.TotalDuration), true, true, false);
            }

            DOTweenEditorPreview.Start();
            s_previewIsPlaying = true;

            s_delayedCall = new DelayedCall(animation.TotalDuration + (animation.AnimationType == AnimationType.Hide ? 0.5f : 0f), () =>
            {
                StopPopupPreview(popup);
                s_delayedCall = null;
            });
        }

        public static void PreviewButtonAnimation(UIAnimation animation, RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            if (s_previewIsPlaying) return;
            if (s_delayedCall != null) s_delayedCall.Cancel();
            StopButtonPreview(rectTransform, canvasGroup);

            RectTransform target = rectTransform;
            s_startPosition = target.anchoredPosition3D;
            s_startRotation = target.localRotation.eulerAngles;
            s_startScale = target.localScale;
            s_startAlpha = canvasGroup.alpha;

            StopAllAnimations(target);

            Vector3 moveFrom = UIAnimator.GetAnimationMoveFrom(rectTransform, animation, s_startPosition);
            Vector3 moveTo = UIAnimator.GetAnimationMoveTo(rectTransform, animation, s_startPosition);
            if (!animation.Move.Enabled) target.anchoredPosition3D = s_startPosition;
            else PreviewMove(rectTransform, moveFrom, moveTo, animation, s_startPosition);

            Vector3 rotateFrom = UIAnimator.GetAnimationRotateFrom(animation, s_startRotation);
            Vector3 rotateTo = UIAnimator.GetAnimationRotateTo(animation, s_startRotation);
            if (!animation.Rotate.Enabled) target.localRotation = Quaternion.Euler(s_startRotation);
            else PreviewRotate(rectTransform, rotateFrom, rotateTo, animation, s_startRotation);

            Vector3 scaleFrom = UIAnimator.GetAnimationScaleFrom(animation, s_startScale);
            Vector3 scaleTo = UIAnimator.GetAnimationScaleTo(animation, s_startScale);
            if (!animation.Scale.Enabled) target.localScale = s_startScale;
            else PreviewScale(rectTransform, scaleFrom, scaleTo, animation, s_startScale);

            float fadeFrom = UIAnimator.GetAnimationFadeFrom(animation, s_startAlpha);
            float fadeTo = UIAnimator.GetAnimationFadeTo(animation, s_startAlpha);
            if (!animation.Fade.Enabled) canvasGroup.alpha = s_startAlpha;
            else PreviewFade(rectTransform, fadeFrom, fadeTo, animation, s_startAlpha);

            DOTweenEditorPreview.Start();
            s_previewIsPlaying = true;

            s_delayedCall = new DelayedCall((animation.AnimationType == AnimationType.Loop ? 5f : animation.TotalDuration + (animation.AnimationType == AnimationType.Hide || animation.AnimationType == AnimationType.State ? 0.5f : 0f)), () =>
            {
                StopButtonPreview(rectTransform, canvasGroup);
                s_delayedCall = null;
            });
        }

        private static void ResetButtonToStartValues(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            rectTransform.anchoredPosition3D = s_startPosition;
            rectTransform.localRotation = Quaternion.Euler(s_startRotation);
            rectTransform.localScale = s_startScale;
            canvasGroup.alpha = s_startAlpha;
        }

        public static void PreviewMove(RectTransform target, Vector3 from, Vector3 to, UIAnimation animation, Vector3 startPosition)
        {
            switch (animation.AnimationType)
            {
                case AnimationType.Show:
                case AnimationType.Hide:
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.MoveTween(target, animation, @from, to), true, true, false);
                    break;
                case AnimationType.Loop:
                    target.anchoredPosition3D = UIAnimator.MoveLoopPositionA(animation, startPosition);
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.MoveLoopTween(target, animation, startPosition), true, true, false);
                    break;
                case AnimationType.Punch:
                    target.anchoredPosition3D = startPosition;
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.MovePunchTween(target, animation), true, true, false);
                    break;
                case AnimationType.State:
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.MoveStateTween(target, animation, startPosition), true, true, false);
                    break;
            }
        }

        public static void PreviewRotate(RectTransform target, Vector3 from, Vector3 to, UIAnimation animation, Vector3 startRotation)
        {
            switch (animation.AnimationType)
            {
                case AnimationType.Show:
                case AnimationType.Hide:
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.RotateTween(target, animation, @from, to), true, true, false);
                    break;
                case AnimationType.Loop:
                    target.localRotation = Quaternion.Euler(UIAnimator.RotateLoopRotationA(animation, startRotation));
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.RotateLoopTween(target, animation, startRotation), true, true, false);
                    break;
                case AnimationType.Punch:
                    target.localRotation = Quaternion.Euler(startRotation);
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.RotatePunchTween(target, animation), true, true, false);
                    break;
                case AnimationType.State:
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.RotateStateTween(target, animation, startRotation), true, true, false);
                    break;
            }
        }

        public static void PreviewScale(RectTransform target, Vector3 from, Vector3 to, UIAnimation animation, Vector3 startScale)
        {
            switch (animation.AnimationType)
            {
                case AnimationType.Show:
                case AnimationType.Hide:
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.ScaleTween(target, animation, @from, to), true, true, false);
                    break;
                case AnimationType.Loop:
                    target.localScale = animation.Scale.From;
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.ScaleLoopTween(target, animation), true, true, false);
                    break;
                case AnimationType.Punch:
                    target.localScale = startScale;
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.ScalePunchTween(target, animation), true, true, false);
                    break;
                case AnimationType.State:
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.ScaleStateTween(target, animation, startScale), true, true, false);
                    break;
            }
        }

        public static void PreviewFade(RectTransform target, float from, float to, UIAnimation animation, float startAlpha)
        {
            switch (animation.AnimationType)
            {
                case AnimationType.Show:
                case AnimationType.Hide:
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.FadeTween(target, animation, @from, to), true, true, false);
                    break;
                case AnimationType.Loop:
                    CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>() != null ? target.GetComponent<CanvasGroup>() : target.gameObject.AddComponent<CanvasGroup>();
                    animation.Fade.From = Mathf.Clamp01(animation.Fade.From);
                    canvasGroup.alpha = animation.Fade.From;
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.FadeLoopTween(target, animation), true, true, false);
                    break;
                case AnimationType.State:
                    DOTweenEditorPreview.PrepareTweenForPreview(UIAnimator.FadeStateTween(target, animation, startAlpha), true, true, false);
                    break;
            }
        }

        public static void StopViewPreview(UIView view)
        {
            DOTweenEditorPreview.Stop(true);
            view.ResetToStartValues();
            s_previewIsPlaying = false;
        }

        public static void StopPopupPreview(UIPopup popup)
        {
            DOTweenEditorPreview.Stop(true);
            popup.ResetToStartValues();
            popup.Container.ResetToStartValues();
            if (popup.HasOverlay) popup.Overlay.ResetToStartValues();
            s_previewIsPlaying = false;
        }

        public static void StopButtonPreview(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            DOTweenEditorPreview.Stop(true);
            if (s_previewIsPlaying) ResetButtonToStartValues(rectTransform, canvasGroup);
            s_previewIsPlaying = false;
        }
    }
}