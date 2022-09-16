using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HighlightPlus {

    [CustomEditor(typeof(HighlightEffect))]
    [CanEditMultipleObjects]
    public class HighlightEffectEditor : Editor {

#pragma warning disable 0618

        SerializedProperty profile, profileSync, ignoreObjectVisibility, reflectionProbes;
        SerializedProperty ignore, previewInEditor, effectGroup, effectGroupLayer, alphaCutOff, cullBackFaces, depthClip;
        SerializedProperty highlighted, fadeInDuration, fadeOutDuration, flipY, constantWidth;
        SerializedProperty overlay, overlayColor, overlayAnimationSpeed, overlayMinIntensity, overlayBlending;
        SerializedProperty outline, outlineColor, outlineWidth, outlineQuality, outlineDownsampling, outlineVisibility, outlineOptimalBlit, outlineBlitDebug, outlineIndependent;
        SerializedProperty glow, glowWidth, glowQuality, glowBlendMode, glowDownsampling, glowHQColor, glowDithering, glowMagicNumber1, glowMagicNumber2, glowAnimationSpeed, glowPasses, glowVisibility, glowOptimalBlit, glowBlitDebug;
        SerializedProperty innerGlow, innerGlowWidth, innerGlowColor, innerGlowVisibility;
        SerializedProperty seeThrough, seeThroughOccluderMask, seeThroughOccluderThreshold, seeThroughOccluderCheckInterval;
        SerializedProperty seeThroughIntensity, seeThroughTintAlpha, seeThroughTintColor, seeThroughNoise, seeThroughBorder, seeThroughBorderWidth, seeThroughBorderColor;
        SerializedProperty targetFX, targetFXTexture, targetFXColor, targetFXCenter, targetFXRotationSpeed, targetFXInitialScale, targetFXEndScale, targetFXTransitionDuration, targetFXStayDuration;
        HighlightEffect thisEffect;
        bool profileChanged, enableProfileApply;

        void OnEnable() {
            profile = serializedObject.FindProperty("profile");
            profileSync = serializedObject.FindProperty("profileSync");
            ignoreObjectVisibility = serializedObject.FindProperty("ignoreObjectVisibility");
            reflectionProbes = serializedObject.FindProperty("reflectionProbes");
            ignore = serializedObject.FindProperty("ignore");
            previewInEditor = serializedObject.FindProperty("previewInEditor");
            effectGroup = serializedObject.FindProperty("effectGroup");
            effectGroupLayer = serializedObject.FindProperty("effectGroupLayer");
            alphaCutOff = serializedObject.FindProperty("alphaCutOff");
            cullBackFaces = serializedObject.FindProperty("cullBackFaces");
            depthClip = serializedObject.FindProperty("depthClip");
            highlighted = serializedObject.FindProperty("_highlighted");
            fadeInDuration = serializedObject.FindProperty("fadeInDuration");
            fadeOutDuration = serializedObject.FindProperty("fadeOutDuration");
            flipY = serializedObject.FindProperty("flipY");
            constantWidth = serializedObject.FindProperty("constantWidth");
            //customTarget = serializedObject.FindProperty("customTarget");
            overlay = serializedObject.FindProperty("overlay");
            overlayColor = serializedObject.FindProperty("overlayColor");
            overlayAnimationSpeed = serializedObject.FindProperty("overlayAnimationSpeed");
            overlayMinIntensity = serializedObject.FindProperty("overlayMinIntensity");
            overlayBlending = serializedObject.FindProperty("overlayBlending");
            outline = serializedObject.FindProperty("outline");
            outlineColor = serializedObject.FindProperty("outlineColor");
            outlineWidth = serializedObject.FindProperty("outlineWidth");
            outlineQuality = serializedObject.FindProperty("outlineQuality");
            outlineDownsampling = serializedObject.FindProperty("outlineDownsampling");
            outlineVisibility = serializedObject.FindProperty("outlineVisibility");
            outlineOptimalBlit = serializedObject.FindProperty("outlineOptimalBlit");
            outlineBlitDebug = serializedObject.FindProperty("outlineBlitDebug");
            outlineIndependent = serializedObject.FindProperty("outlineIndependent");
            glow = serializedObject.FindProperty("glow");
            glowWidth = serializedObject.FindProperty("glowWidth");
            glowQuality = serializedObject.FindProperty("glowQuality");
            glowDownsampling = serializedObject.FindProperty("glowDownsampling");
            glowHQColor = serializedObject.FindProperty("glowHQColor");
            glowAnimationSpeed = serializedObject.FindProperty("glowAnimationSpeed");
            glowDithering = serializedObject.FindProperty("glowDithering");
            glowBlendMode = serializedObject.FindProperty("glowBlendMode");
            glowMagicNumber1 = serializedObject.FindProperty("glowMagicNumber1");
            glowMagicNumber2 = serializedObject.FindProperty("glowMagicNumber2");
            glowAnimationSpeed = serializedObject.FindProperty("glowAnimationSpeed");
            glowPasses = serializedObject.FindProperty("glowPasses");
            glowVisibility = serializedObject.FindProperty("glowVisibility");
            glowOptimalBlit = serializedObject.FindProperty("glowOptimalBlit");
            glowBlitDebug = serializedObject.FindProperty("glowBlitDebug");
            innerGlow = serializedObject.FindProperty("innerGlow");
            innerGlowColor = serializedObject.FindProperty("innerGlowColor");
            innerGlowWidth = serializedObject.FindProperty("innerGlowWidth");
            innerGlowVisibility = serializedObject.FindProperty("innerGlowVisibility");
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
            targetFX = serializedObject.FindProperty("targetFX");
            targetFXTexture = serializedObject.FindProperty("targetFXTexture");
            targetFXRotationSpeed = serializedObject.FindProperty("targetFXRotationSpeed");
            targetFXInitialScale = serializedObject.FindProperty("targetFXInitialScale");
            targetFXEndScale = serializedObject.FindProperty("targetFXEndScale");
            targetFXColor = serializedObject.FindProperty("targetFXColor");
            targetFXCenter = serializedObject.FindProperty("targetFXCenter");
            targetFXTransitionDuration = serializedObject.FindProperty("targetFXTransitionDuration");
            targetFXStayDuration = serializedObject.FindProperty("targetFXStayDuration");
            thisEffect = (HighlightEffect)target;
            thisEffect.Refresh();
        }

        public override void OnInspectorGUI() {
            bool isManager = false;
            EditorGUILayout.Separator();
            serializedObject.Update();


            EditorGUILayout.BeginHorizontal();
            HighlightProfile prevProfile = (HighlightProfile)profile.objectReferenceValue;
            EditorGUILayout.PropertyField(profile, new GUIContent("Profile", "Create or load stored presets."));
            if (profile.objectReferenceValue != null) {

                if (prevProfile != profile.objectReferenceValue) {
                    profileChanged = true;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(EditorGUIUtility.labelWidth));
                if (GUILayout.Button(new GUIContent("Create", "Creates a new profile which is a copy of the current settings."), GUILayout.Width(60))) {
                    CreateProfile();
                    profileChanged = false;
                    enableProfileApply = false;
                    GUIUtility.ExitGUI();
                    return;
                }
                if (GUILayout.Button(new GUIContent("Load", "Updates settings with the profile configuration."), GUILayout.Width(60))) {
                    profileChanged = true;
                }
                if (!enableProfileApply)
                    GUI.enabled = false;
                if (GUILayout.Button(new GUIContent("Save", "Updates profile configuration with changes in this inspector."), GUILayout.Width(60))) {
                    enableProfileApply = false;
                    profileChanged = false;
                    thisEffect.profile.Save(thisEffect);
                    EditorUtility.SetDirty(thisEffect.profile);
                    GUIUtility.ExitGUI();
                    return;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(profileSync, new GUIContent("Sync With Profile", "If disabled, profile settings will only be loaded when clicking 'Load' which allows you to customize settings after loading a profile and keep those changes."));
                EditorGUILayout.BeginHorizontal();
            } else {
                if (GUILayout.Button(new GUIContent("Create", "Creates a new profile which is a copy of the current settings."), GUILayout.Width(60))) {
                    CreateProfile();
                    GUIUtility.ExitGUI();
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();


            if (isManager) {
                EditorGUILayout.HelpBox("These are default settings for highlighted objects. If the highlighted object already has a Highlight Effect component, those properties will be used.", MessageType.Info);
            } else {
                EditorGUILayout.PropertyField(previewInEditor);
            }

            EditorGUILayout.PropertyField(ignoreObjectVisibility);
            if (thisEffect.staticChildren) {
                EditorGUILayout.HelpBox("This GameObject or one of its children is marked as static. If highlight is not visible, add a MeshCollider to them.", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(reflectionProbes);

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Highlight Options", EditorStyles.boldLabel);
            if (GUILayout.Button("Help", GUILayout.Width(50))) {
                EditorUtility.DisplayDialog("Quick Help", "Move the mouse over a setting for a short description.\n\nVisit kronnect.com's forum for support, questions and more cool assets.\n\nIf you like Highlight Plus please rate it or leave a review on the Asset Store! Thanks.", "Ok");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            if (!isManager) {
                EditorGUILayout.PropertyField(ignore, new GUIContent("Ignore", "This object won't be highlighted."));
                if (!ignore.boolValue) {
                    EditorGUILayout.PropertyField(highlighted);
                }
            }
            if (!ignore.boolValue) {
                EditorGUILayout.PropertyField(effectGroup, new GUIContent("Include", "Additional objects to highlight. Pro tip: when highlighting multiple objects at the same time include them in the same layer or under the same parent."));
                if (effectGroup.intValue == (int)TargetOptions.LayerInScene || effectGroup.intValue == (int)TargetOptions.LayerInChildren) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(effectGroupLayer, new GUIContent("Layer"));
                    EditorGUI.indentLevel--;
                }

                //EditorGUILayout.PropertyField(customTarget, new GUIContent("CustomTarget", "自定义目标"));

                bool usesHQEffects = (outlineQuality.intValue == (int)QualityLevel.Highest && outline.floatValue > 0) || (glowQuality.intValue == (int)QualityLevel.Highest && glow.floatValue > 0);
                EditorGUILayout.PropertyField(alphaCutOff, new GUIContent("Alpha Cut Off", "Only for semi-transparent objects. Leave this to zero for normal opaque objects."));
                EditorGUILayout.PropertyField(cullBackFaces);
                EditorGUILayout.PropertyField(fadeInDuration);
                EditorGUILayout.PropertyField(fadeOutDuration);
                if (usesHQEffects) {
                    EditorGUILayout.PropertyField(depthClip);
                    if (usesHQEffects && VRCheck.IsActive()) {
                        EditorGUILayout.PropertyField(flipY, new GUIContent("Flip Y Fix", "Flips outline/glow effect to fix bug introduced in Unity 2019.1.0 when VR is enabled."));
                    }
                }
                if (glowQuality.intValue != (int)QualityLevel.Highest || outlineQuality.intValue != (int)QualityLevel.Highest) {
                    EditorGUILayout.PropertyField(constantWidth, new GUIContent("Constant Width", "Compensates outline/glow width with depth increase."));
                }
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(outline, "Outline", outline.floatValue > 0);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(outlineWidth, new GUIContent("Width"));
                EditorGUILayout.PropertyField(outlineColor, new GUIContent("Color"));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(outlineQuality, new GUIContent("Quality", "Default and High use a mesh displacement technique. Highest quality can provide best look and also performance depending on the complexity of mesh."));
                if (outlineQuality.intValue == (int)QualityLevel.Highest) {
                    GUILayout.Label("(Screen-Space Effect)");
                } else {
                    GUILayout.Label("(Mesh-based Effect)");
                }
                EditorGUILayout.EndHorizontal();
                CheckVRSupport(outlineQuality.intValue);
                if (outlineQuality.intValue == (int)QualityLevel.Highest) {
                    EditorGUILayout.PropertyField(outlineDownsampling, new GUIContent("Downsampling"));
                    EditorGUILayout.PropertyField(outlineOptimalBlit, new GUIContent("Optimal Blit", "Blits result over a section of the screen instead of rendering to the full screen buffer."));
                    if (outlineOptimalBlit.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(outlineBlitDebug, new GUIContent("Debug View", "Shows the blitting rectangle on the screen."));
                        if (outlineBlitDebug.boolValue && (!previewInEditor.boolValue || !highlighted.boolValue)) {
                            EditorGUILayout.HelpBox("Enable \"Preview In Editor\" and \"Highlighted\" to display the outline Debug View.", MessageType.Warning);
                        }
                        EditorGUI.indentLevel--;
                    }
                }

                GUI.enabled = outlineQuality.intValue != (int)QualityLevel.Highest || CheckForwardMSAA();
                if (outlineQuality.intValue == (int)QualityLevel.Highest && glowQuality.intValue == (int)QualityLevel.Highest) {
                    EditorGUILayout.PropertyField(glowVisibility, new GUIContent("Visibility"));
                } else {
                    EditorGUILayout.PropertyField(outlineVisibility, new GUIContent("Visibility"));
                }
                GUI.enabled = true;
                EditorGUILayout.PropertyField(outlineIndependent, new GUIContent("Independent", "Shows full outline regardless of other highlighted objects."));

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(glow, "Outer Glow", glow.floatValue > 0);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(glowWidth, new GUIContent("Width"));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(glowQuality, new GUIContent("Quality", "Default and High use a mesh displacement technique. Highest quality can provide best look and also performance depending on the complexity of mesh."));
                if (glowQuality.intValue == (int)QualityLevel.Highest) {
                    GUILayout.Label("(Screen-Space Effect)");
                } else {
                    GUILayout.Label("(Mesh-based Effect)");
                }
                EditorGUILayout.EndHorizontal();
                CheckVRSupport(glowQuality.intValue);
                if (glowQuality.intValue == (int)QualityLevel.Highest) {
                    EditorGUILayout.PropertyField(glowDownsampling, new GUIContent("Downsampling"));
                    EditorGUILayout.PropertyField(glowHQColor, new GUIContent("Color"));
                    EditorGUILayout.PropertyField(glowBlendMode, new GUIContent("Blend Mode"));
                    EditorGUILayout.PropertyField(glowOptimalBlit, new GUIContent("Optimal Blit", "Blits result over a section of the screen instead of rendering to the full screen buffer."));
                    if (glowOptimalBlit.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(glowBlitDebug, new GUIContent("Debug View", "Shows the blitting rectangle on the screen."));
                        if (glowBlitDebug.boolValue && (!previewInEditor.boolValue || !highlighted.boolValue)) {
                            EditorGUILayout.HelpBox("Enable \"Preview In Editor\" and \"Highlighted\" to display the glow Debug View.", MessageType.Warning);
                        }
                        EditorGUI.indentLevel--;
                    }
                    GUI.enabled = glowQuality.intValue != (int)QualityLevel.Highest || CheckForwardMSAA();
                    EditorGUILayout.PropertyField(glowVisibility, new GUIContent("Visibility"));
                    GUI.enabled = true;
                } else {
                    GUI.enabled = glowQuality.intValue != (int)QualityLevel.Highest || CheckForwardMSAA();
                    EditorGUILayout.PropertyField(glowVisibility, new GUIContent("Visibility"));
                    GUI.enabled = true;
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
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(innerGlow, "Inner Glow", innerGlow.floatValue > 0);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(innerGlowColor, new GUIContent("Color"));
                EditorGUILayout.PropertyField(innerGlowWidth, new GUIContent("Width"));
                EditorGUILayout.PropertyField(innerGlowVisibility, new GUIContent("Visibility"));
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(overlay, "Overlay", overlay.floatValue > 0);
                if (overlay.floatValue < overlayMinIntensity.floatValue) {
                    overlayMinIntensity.floatValue = overlay.floatValue;
                }
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(overlayColor, new GUIContent("Color"));
                EditorGUILayout.PropertyField(overlayBlending, new GUIContent("Blending"));
                EditorGUILayout.PropertyField(overlayMinIntensity, new GUIContent("Min Intensity"));
                if (overlayMinIntensity.floatValue > overlay.floatValue) {
                    overlay.floatValue = overlayMinIntensity.floatValue;
                }
                EditorGUILayout.PropertyField(overlayAnimationSpeed, new GUIContent("Animation Speed"));
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(targetFX, "Target", targetFX.boolValue);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(targetFXTexture, new GUIContent("Texture"));
                EditorGUILayout.PropertyField(targetFXColor, new GUIContent("Color"));
                EditorGUILayout.PropertyField(targetFXCenter, new GUIContent("Center", "Optionally assign a transform. Target will follow transform. If the object is skinned, you can also assign a bone to reflect currenct animation position."));
                EditorGUILayout.PropertyField(targetFXRotationSpeed, new GUIContent("Rotation Speed"));
                EditorGUILayout.PropertyField(targetFXInitialScale, new GUIContent("Initial Scale"));
                EditorGUILayout.PropertyField(targetFXEndScale, new GUIContent("End Scale"));
                EditorGUILayout.PropertyField(targetFXTransitionDuration, new GUIContent("Transition Duration"));
                EditorGUILayout.PropertyField(targetFXStayDuration, new GUIContent("Stay Duration"));
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.PropertyField(seeThrough);
            if (isManager && seeThrough.intValue == (int)SeeThroughMode.AlwaysWhenOccluded) {
                EditorGUILayout.HelpBox("This option is not valid in Manager.\nTo make an object always visible add a Highlight Effect component to the gameobject and enable this option on the component.", MessageType.Error);
            }
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
            EditorGUILayout.PropertyField(seeThroughBorder, new GUIContent("Border When Hidden" + ((seeThroughBorder.floatValue > 0) ? " •" : "")));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(seeThroughBorderWidth, new GUIContent("Width"));
            EditorGUILayout.PropertyField(seeThroughBorderColor, new GUIContent("Color"));
            EditorGUI.indentLevel--;

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            if (serializedObject.ApplyModifiedProperties() || profileChanged) {
                if (thisEffect.profile != null) {
                    if (profileChanged) {
                        thisEffect.profile.Load(thisEffect);
                        profileChanged = false;
                        enableProfileApply = false;
                    } else {
                        enableProfileApply = true;
                    }
                }

                foreach (HighlightEffect effect in targets) {
                    effect.Refresh();
                }
            }
        }

        void DrawSectionField(SerializedProperty property, string label, bool active) {
            EditorGUILayout.PropertyField(property, new GUIContent(active ? label + " •" : label));
        }

        void CheckVRSupport(int qualityLevel) {
            if (qualityLevel == (int)QualityLevel.Highest && VRCheck.IsActive()) {
                if (PlayerSettings.stereoRenderingPath != StereoRenderingPath.MultiPass) {
                    EditorGUILayout.HelpBox("Highest Quality only supports VR Multi-Pass as CommandBuffers do not support this VR mode yet. Either switch to 'High Quality' or change VR Stereo mode to Multi-Pass.", MessageType.Error);
                }
            }
        }

        bool CheckForwardMSAA() {
            if (QualitySettings.antiAliasing > 1) {
                if (Camera.main != null && Camera.main.allowMSAA && !depthClip.boolValue) {
                    EditorGUILayout.HelpBox("Effect will be shown always on top due to MSAA. To enable depth clipping enable 'Depth Clip' option above OR disable MSAA in Quality Settings OR choose a different quality level.", MessageType.Info);
                    return false;
                }
            }
            return true;
        }


        #region Profile handling

        void CreateProfile() {

            HighlightProfile newProfile = ScriptableObject.CreateInstance<HighlightProfile>();
            newProfile.Save(thisEffect);

            AssetDatabase.CreateAsset(newProfile, "Assets/Highlight Plus Profile.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newProfile;

            thisEffect.profile = newProfile;
        }


        #endregion

#pragma warning restore 0618

    }

}