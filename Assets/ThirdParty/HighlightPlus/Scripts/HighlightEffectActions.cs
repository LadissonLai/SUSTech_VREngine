using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighlightPlus {


	public partial class HighlightEffect : MonoBehaviour {

		bool overlayOneShotRunning = false;
		bool currentHighlighted;
		Color overlayOneShotCurrentColor;
		float overlayOneShotCurrentAnimationSpeed, overlayOneShotCurrentOverlay;
		float overlayOneShotCurrentOuterGlow, overlayOneShotCurrentInnerGlow, overlayOneShotCurrentOutline, overlayOneShotCurrentSeeThroughIntensity;
		Coroutine overlayOneShotCo;

		public void OverlayOneShot (Color color, float duration) {
			if (duration == 0)
				return;

			if (overlayOneShotRunning) {
				OverlayOneShotEnd ();
				StopCoroutine (overlayOneShotCo);
			}
			overlayOneShotRunning = true;
			overlayOneShotCurrentOverlay = overlay;
			overlayOneShotCurrentColor = overlayColor;
			overlayOneShotCurrentAnimationSpeed = overlayAnimationSpeed;
			overlayOneShotCurrentOuterGlow = glow;
			overlayOneShotCurrentInnerGlow = innerGlow;
			overlayOneShotCurrentOutline = outline;
			overlayOneShotCurrentSeeThroughIntensity = seeThroughIntensity;
			currentHighlighted = _highlighted;
			if (!currentHighlighted) {
				glow = innerGlow = outline = seeThroughIntensity = 0;
			}

			overlayOneShotCo = StartCoroutine (OverlayOneShotAnimator (color, duration));
		}

		IEnumerator OverlayOneShotAnimator (Color color, float duration) {
			overlayAnimationSpeed = 0;

			float startTime = Time.time;
			float t = 1f;
			highlighted = true;
			overlayColor = color;
			overlayAnimationSpeed = 0;
			WaitForEndOfFrame ef = new WaitForEndOfFrame ();
			while (t > 0f) {
				t = 1f - (Time.time - startTime) / duration;
				if (t < 0) {
					t = 0f;
				}
				overlay = t;
				yield return ef;
			}

			OverlayOneShotEnd ();
		}

		void OverlayOneShotEnd() {
			overlay = overlayOneShotCurrentOverlay;
			overlayColor = overlayOneShotCurrentColor;
			overlayAnimationSpeed = overlayOneShotCurrentAnimationSpeed;
			if (!currentHighlighted) {
				glow = overlayOneShotCurrentOuterGlow;
				innerGlow = overlayOneShotCurrentInnerGlow;
				outline = overlayOneShotCurrentOutline;
				seeThroughIntensity = overlayOneShotCurrentSeeThroughIntensity;
				highlighted = false;
			}
			overlayOneShotRunning = false;
		}
	}

}