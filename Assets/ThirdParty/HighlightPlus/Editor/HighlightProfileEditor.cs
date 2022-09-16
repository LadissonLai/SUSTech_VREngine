using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HighlightPlus {

    [CustomEditor(typeof(HighlightProfile))]
    [CanEditMultipleObjects]
    public class HighlightProfileEditor : Editor {

        SerializedProperty overlay, overlayColor, overlayAnimationSpeed, overlayMinIntensity, overlayBlending;
        SerializedProperty effectGroup, effectGroupLayer, alphaCutOff, cullBackFaces, depthClip;
        SerializedProperty fadeInDuration, fadeOutDuration;
        SerializedProperty outline, outlineColor, outlineWidth, outlineQuality, outlineDownsampling, outlineOptimalBlit, outlineVisibility, outlineIndependent;
        SerializedProperty glow, glowWidth, glowQuality, glowOptimalBlit, glowDownsampling, glowHQColor, glowBlendMode, glowDithering, glowMagicNumber1, glowMagicNumber2, glowAnimationSpeed, glowVisibility, glowPasses;
        SerializedProperty innerGlow, innerGlowWidth, innerGlowColor, innerGlowVisibility;
        SerializedProperty targetFX, targetFXTexture, targetFXColor, targetFXRotationSpeed, targetFXInitialScale, targetFXEndScale, targetFXTransitionDuration, targetFXStayDuration;
        SerializedProperty seeThrough, seeThroughOccluderMask, seeThroughOccluderThreshold, seeThroughOccluderCheckInterval;
        SerializedProperty seeThroughIntensity, seeThroughTintAlpha, seeThroughTintColor, seeThroughNoise, seeThroughBorder, seeThroughBorderWidth, seeThroughBorderColor;

        void OnEnable() {
            effectGroup = serializedObject.FindProperty("effectGroup");
            effectGroupLayer = serializedObject.FindProperty("effectGroupLayer");
            alphaCutOff = serializedObject.FindProperty("alphaCutOff");
            cullBackFaces = serializedObject.FindProperty("cullBackFaces");
            depthClip = serializedObject.FindProperty("depthClip");
            fadeInDuration = serializedObject.FindProperty("fadeInDuration");
            fadeOutDuration = serializedObject.FindProperty("fadeOutDuration");
            overlay = serializedObject.FindProperty("overlay");
            overlayColor = serializedObject.FindProperty("overlayColor");
            overlayAnimationSpeed = serializedObject.FindProperty("overlayAnimationSpeed");
            overlayMinIntensity = serializedObject.FindProperty("overlayMinIntensity");
            overlayBlending = serializedObject.FindProperty("overlayBlending");
            outline = serializedObject.FindProperty("outline");
            outlineColor = serializedObject.FindProperty("outlineColor");
            outlineWidth = serializedObject.FindProperty("outlineWidth");
            outlineQuality = serializedObject.FindProperty("outlineQuality");
            outlineOptimalBlit = serializedObject.FindProperty("outlineOptimalBlit");
            outlineDownsampling = serializedObject.FindProperty("outlineDownsampling");
            outlineVisibility = serializedObject.FindProperty("outlineVisibility");
            outlineIndependent = serializedObject.FindProperty("outlineIndependent");
            glow = serializedObject.FindProperty("glow");
            glowWidth = serializedObject.FindProperty("glowWidth");
            glowQuality = serializedObject.FindProperty("glowQuality");
            glowOptimalBlit = serializedObject.FindProperty("glowOptimalBlit");
            glowDownsampling = serializedObject.FindProperty("glowDownsampling");
            glowHQColor = serializedObject.FindProperty("glowHQColor");
            glowBlendMode = serializedObject.FindProperty("glowBlendMode");
            glowAnimationSpeed = serializedObject.FindProperty("glowAnimationSpeed");
            glowDithering = serializedObject.FindProperty("glowDithering");
            glowMagicNumber1 = serializedObject.FindProperty("glowMagicNumber1");
            glowMagicNumber2 = serializedObject.FindProperty("glowMagicNumber2");
            glowAnimationSpeed = serializedObject.FindProperty("glowAnimationSpeed");
            glowVisibility = serializedObject.FindProperty("glowVisibility");
            glowPasses = serializedObject.FindProperty("glowPasses");
            innerGlow = serializedObject.FindProperty("innerGlow");
            innerGlowColor = serializedObject.FindProperty("innerGlowColor");
            innerGlowWidth = serializedObject.FindProperty("innerGlowWidth");
            innerGlowVisibility = serializedObject.FindProperty("innerGlowVisibility");
            targetFX = serializedObject.FindProperty("targetFX");
            targetFXTexture = serializedObject.FindProperty("targetFXTexture");
            targetFXRotationSpeed = serializedObject.FindProperty("targetFXRotationSpeed");
            targetFXInitialScale = serializedObject.FindProperty("targetFXInitialScale");
            targetFXEndScale = serializedObject.FindProperty("targetFXEndScale");
            targetFXColor = serializedObject.FindProperty("targetFXColor");
            targetFXTransitionDuration = serializedObject.FindProperty("targetFXTransitionDuration");
            targetFXStayDuration = serializedObject.FindProperty("targetFXStayDuration");
            seeThrough = serializedObject.FindProperty("seeThrough");
            seeThroughOccluderMask = serializedObject.FindProperty("seeThroughOccluderMask");
            seeThroughOccluderThreshold = serializedObject.FindProperty("seeThroughOccluderThreshold");
            seeThroughOccluderCheckInterval = serializedObject.FindProperty("seeThroughOccluderCheckInterval");
            seeThroughIntensity = serializedObject.FindProperty("seeThroughIntensity");
            seeThroughTintAlpha = serializedObject.FindProperty("seeThroughTintAlpha");
            seeThroughTintColor = serializedObject.FindProperty("seeThroughTintColor");
            seeThroughNoise = serializedObject.FindProperty("seeThroughNoise");
            seeThroughBorder = serializedObject.FindProperty("seeThroughBorder");
            seeThroughBorderWidth = serializedObject.FindProperty("seeThroughBorderWidth");
            seeThroughBorderColor = serializedObject.FindProperty("seeThroughBorderColor");
        }

        public override void OnInspectorGUI() {

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Highlight Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(effectGroup, new GUIContent("Include"));
            if (effectGroup.intValue == (int)TargetOptions.LayerInScene || effectGroup.intValue == (int)TargetOptions.LayerInChildren) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(effectGroupLayer, new GUIContent("Layer"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(alphaCutOff);
            EditorGUILayout.PropertyField(cullBackFaces);
            EditorGUILayout.PropertyField(depthClip);
            EditorGUILayout.PropertyField(fadeInDuration);
            EditorGUILayout.PropertyField(fadeOutDuration);
            EditorGUILayout.PropertyField(outline);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(outlineWidth, new GUIContent("Width"));
            EditorGUILayout.PropertyField(outlineColor, new GUIContent("Color"));
            EditorGUILayout.PropertyField(outlineQuality, new GUIContent("Quality", "Default and High use a mesh displacement technique. Highest quality can provide best look and also performance depending on the complexity of mesh."));
            if (outlineQuality.intValue == (int)QualityLevel.Highest) {
                EditorGUILayout.PropertyField(outlineDownsampling, new GUIContent("Downsampling"));
                EditorGUILayout.PropertyField(outlineOptimalBlit, new GUIContent("Optimal Blit", "Blits result over a section of the screen instead of rendering to the full screen buffer."));
            }
            if (outlineQuality.intValue == (int)QualityLevel.Highest && glowQuality.intValue == (int)QualityLevel.Highest) {
                EditorGUILayout.PropertyField(glowVisibility, new GUIContent("Visibility"));
            } else {
                EditorGUILayout.PropertyField(outlineVisibility, new GUIContent("Visibility"));
            }
            if (outlineQuality.intValue != (int)QualityLevel.Highest) {
                GUI.enabled = outlineVisibility.intValue != (int)Visibility.AlwaysOnTop;
                EditorGUILayout.PropertyField(outlineIndependent, new GUIContent("Independent", "Shows full outline regardless of other highlighted objects."));
                GUI.enabled = true;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(glow, new GUIContent("Outer Glow"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(glowWidth, new GUIContent("Width"));
            EditorGUILayout.PropertyField(glowQuality, new GUIContent("Quality", "Default and High use a mesh displacement technique. Highest quality can provide best look and also performance depending on the complexity of mesh."));
            if (glowQuality.intValue == (int)QualityLevel.Highest) {
                EditorGUILayout.PropertyField(glowDownsampling, new GUIContent("Downsampling"));
                EditorGUILayout.PropertyField(glowHQColor, new GUIContent("Color"));
                EditorGUILayout.PropertyField(glowBlendMode, new GUIContent("Blend Mode"));
                EditorGUILayout.PropertyField(glowOptimalBlit, new GUIContent("Optimal Blit", "Blits result over a section of the screen instead of rendering to the full screen buffer."));
            }
            EditorGUILayout.PropertyField(glowVisibility, new GUIContent("Visibility"));
            if (glowQuality.intValue != (int)QualityLevel.Highest) {
                EditorGUILayout.PropertyField(glowDithering, new GUIContent("Dithering"));
                if (glowDithering.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(glowMagicNumber1, new GUIContent("Magic Number 1"));
                    EditorGUILayout.PropertyField(glowMagicNumber2, new GUIContent("Magic Number 2"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(glowPasses, true);
            }
            EditorGUILayout.PropertyField(glowAnimationSpeed, new GUIContent("Animation Speed"));
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(innerGlow, new GUIContent("Inner Glow"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(innerGlowColor, new GUIContent("Color"));
            EditorGUILayout.PropertyField(innerGlowWidth, new GUIContent("Width"));
            EditorGUILayout.PropertyField(innerGlowVisibility, new GUIContent("Visibility"));
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(overlay);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(overlayColor, new GUIContent("Color"));
            EditorGUILayout.PropertyField(overlayBlending, new GUIContent("Blending"));
            EditorGUILayout.PropertyField(overlayMinIntensity, new GUIContent("Min Intensity"));
            EditorGUILayout.PropertyField(overlayAnimationSpeed, new GUIContent("Animation Speed"));
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(targetFX, new GUIContent("Target"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(targetFXTexture, new GUIContent("Texture"));
            EditorGUILayout.PropertyField(targetFXColor, new GUIContent("Color"));
            EditorGUILayout.PropertyField(targetFXRotationSpeed, new GUIContent("Rotation Speed"));
            EditorGUILayout.PropertyField(targetFXInitialScale, new GUIContent("Initial Scale"));
            EditorGUILayout.PropertyField(targetFXEndScale, new GUIContent("End Scale"));
            EditorGUILayout.PropertyField(targetFXTransitionDuration, new GUIContent("Transition Duration"));
            EditorGUILayout.PropertyField(targetFXStayDuration, new GUIContent("Stay Duration"));
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("See-Through Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(seeThrough);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(seeThroughOccluderMask, new GUIContent("Occluder Layer"));
            if (seeThroughOccluderMask.intValue > 0) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(seeThroughOccluderThreshold, new GUIContent("Radius Threshold", "Multiplier to the object bounds. Making the bounds smaller prevents false occlusion tests."));
                EditorGUILayout.PropertyField(seeThroughOccluderCheckInterval, new GUIContent("Check Interval", "Interval in seconds between occlusion tests."));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(seeThroughIntensity, new GUIContent("Intensity"));
            EditorGUILayout.PropertyField(seeThroughTintColor, new GUIContent("Color"));
            EditorGUILayout.PropertyField(seeThroughTintAlpha, new GUIContent("Color Blend"));
            EditorGUILayout.PropertyField(seeThroughNoise, new GUIContent("Noise"));
            EditorGUILayout.PropertyField(seeThroughBorder, new GUIContent("Border When Hidden"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(seeThroughBorderWidth, new GUIContent("Width"));
            EditorGUILayout.PropertyField(seeThroughBorderColor, new GUIContent("Color"));
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

            if (serializedObject.ApplyModifiedProperties() || (Event.current.type == EventType.ExecuteCommand &&
                Event.current.commandName == "UndoRedoPerformed")) {

                // Triggers profile reload on all Highlight Effect scripts
                HighlightEffect[] effects = FindObjectsOfType<HighlightEffect>();
                for (int t = 0; t < targets.Length; t++) {
                    HighlightProfile profile = (HighlightProfile)targets[t];
                    for (int k = 0; k < effects.Length; k++) {
                        if (effects[k] != null && effects[k].profile == profile && effects[k].profileSync) {
                            profile.Load(effects[k]);
                            effects[k].Refresh();
                        }
                    }
                }
                EditorUtility.SetDirty(target);
            }

        }


    }

}