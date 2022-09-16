// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.Soundy;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Soundy
{
    [CustomEditor(typeof(SoundGroupData))]
    public class SoundGroupDataEditor : BaseEditor
    {
        private SoundyAudioPlayer.Player Player { get { return SoundyAudioPlayer.GetPlayer(Target); } }
        private SoundGroupData m_target;

        private SoundGroupData Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (SoundGroupData) target;
                return m_target;
            }
        }

        protected override ColorName ComponentColorName { get { return DGUI.Colors.SoundGroupDataColorName; } }

        private SoundGroupData Data { get { return (SoundGroupData) target; } }
        private AudioSource m_previewAudioSource;
        private bool m_soundNameHasBeenChanged;
        private bool m_needsSave;

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_previewAudioSource = EditorUtility.CreateGameObjectWithHideFlags("Soundy Audio Data Preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();

            if (Data.name.Equals(GetProperty(PropertyName.SoundName).stringValue)) return;
            Data.name = GetProperty(PropertyName.SoundName).stringValue;
            Data.SetDirty(false);
            m_needsSave = true;
        }

        protected override void OnDisable()
        {
            DestroyImmediate(m_previewAudioSource.gameObject);
            if (Data == null) return;

            if (!Data.name.Equals(GetProperty(PropertyName.SoundName).stringValue))
            {
                Data.name = GetProperty(PropertyName.SoundName).stringValue;
                m_needsSave = true;
            }

            if (m_soundNameHasBeenChanged)
            {
                SoundDatabase soundDatabase = SoundySettings.Database.GetSoundDatabase(GetProperty(PropertyName.DatabaseName).stringValue);
                if (soundDatabase != null && m_soundNameHasBeenChanged)
                    soundDatabase.UpdateSoundNames(false);
            }

            if (m_needsSave) Data.SetDirty(false);

            if (!m_needsSave && !m_soundNameHasBeenChanged) return;
            AssetDatabase.SaveAssets();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderSoundGroupData));
            GUI.enabled = !GetProperty(PropertyName.SoundName).stringValue.Equals(SoundyManager.NO_SOUND);
            {
                DrawSoundName(); //SOUND NAME
                GUILayout.Space(DGUI.Properties.Space(2));
                DrawLoopAndIgnoreListenerPause();
                GUILayout.Space(DGUI.Properties.Space(2));
                DrawVolume(); //VOLUME
                GUILayout.Space(DGUI.Properties.Space());
                DrawPitch(); //PITCH
                GUILayout.Space(DGUI.Properties.Space());
                DrawSpatialBlend(); //SPATIAL BLEND
                GUILayout.Space(DGUI.Properties.Space(2));
                DrawPlayMode(); //PLAY MODE & LOOP
                GUILayout.Space(DGUI.Properties.Space(4));
                DrawPlayer(); //PLAYER
                GUILayout.Space(DGUI.Properties.Space(6));
                DrawSounds(); //SOUNDS
                GUILayout.Space(DGUI.Properties.Space());
            }
            GUI.enabled = true;
            GUILayout.Space(DGUI.Properties.Space());
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSoundName()
        {
            SerializedProperty soundName = GetProperty(PropertyName.SoundName);
            ColorName colorName = string.IsNullOrEmpty(soundName.stringValue.Trim()) ? ColorName.Red : ComponentColorName;
            DGUI.Line.Draw(false, colorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.SoundName, Size.S, colorName, DGUI.Properties.SingleLineHeight); //sound name label
                               EditorGUI.BeginChangeCheck();
                               DGUI.Property.Draw(soundName, DGUI.Properties.SingleLineHeight); //sound name property
                               if (EditorGUI.EndChangeCheck())
                               {
                                   soundName.stringValue = soundName.stringValue.Trim();
                                   m_soundNameHasBeenChanged = true;
                               }

                               GUILayout.Space(DGUI.Properties.Space(2));
                           });
        }

        private void DrawLoopAndIgnoreListenerPause()
        {
            SerializedProperty loop = GetProperty(PropertyName.Loop);
            SerializedProperty ignoreListenerPause = GetProperty(PropertyName.IgnoreListenerPause);
            DGUI.Line.Draw(true, ComponentColorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space());
                               DGUI.Label.Draw(UILabels.Settings, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight); //settings label
                               GUILayout.FlexibleSpace();
                               DGUI.Toggle.Switch.Draw(loop, UILabels.LoopSound, ComponentColorName, false, false);                          //loop switch property
                               DGUI.Toggle.Switch.Draw(ignoreListenerPause, UILabels.IgnoreListenerPause, ComponentColorName, false, false); //ignore listener switch property
                           });

            GUILayout.Space(-DGUI.Properties.Space());
        }

        private void DrawVolume()
        {
            SerializedProperty volume = GetProperty(PropertyName.Volume);
            SerializedProperty min = GetProperty(PropertyName.MinValue, volume);
            SerializedProperty max = GetProperty(PropertyName.MaxValue, volume);

            DGUI.Line.Draw(false, ComponentColorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.VolumeDb, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight); //volume label
                               EditorGUI.BeginChangeCheck();
                               DGUI.Property.Draw(volume, DGUI.Properties.SingleLineHeight); //volume property
                               if (EditorGUI.EndChangeCheck())
                               {
                                   min.floatValue = (float) Math.Round(min.floatValue, 1);
                                   max.floatValue = (float) Math.Round(max.floatValue, 1);
                               }

                               GUILayout.Space(DGUI.Properties.Space());
                               if (DGUI.Button.IconButton.Reset(DGUI.Properties.SingleLineHeight, ComponentColorName))
                               {
                                   Undo.RecordObject(this, "Reset Volume");
                                   Target.Volume.MinValue = SoundGroupData.DEFAULT_VOLUME;
                                   Target.Volume.MaxValue = SoundGroupData.DEFAULT_VOLUME;
                                   m_needsSave = true;
                               }

                               GUILayout.Space(DGUI.Properties.Space(2));
                           });
        }

        private void DrawPitch()
        {
            SerializedProperty pitch = GetProperty(PropertyName.Pitch);
            SerializedProperty min = GetProperty(PropertyName.MinValue, pitch);
            SerializedProperty max = GetProperty(PropertyName.MaxValue, pitch);

            DGUI.Line.Draw(false, ComponentColorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.PitchSemitones, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight); //pitch label
                               EditorGUI.BeginChangeCheck();
                               DGUI.Property.Draw(pitch, DGUI.Properties.SingleLineHeight); //pitch property
                               if (EditorGUI.EndChangeCheck())
                               {
                                   min.floatValue = (float) Math.Round(min.floatValue, 1);
                                   max.floatValue = (float) Math.Round(max.floatValue, 1);
                               }

                               GUILayout.Space(DGUI.Properties.Space());
                               if (DGUI.Button.IconButton.Reset(DGUI.Properties.SingleLineHeight, ComponentColorName))
                               {
                                   Undo.RecordObject(this, "Reset Pitch");
                                   Target.Pitch.MinValue = SoundGroupData.DEFAULT_PITCH;
                                   Target.Pitch.MaxValue = SoundGroupData.DEFAULT_PITCH;
                                   m_needsSave = true;
                               }

                               GUILayout.Space(DGUI.Properties.Space(2));
                           });
        }

        private void DrawSpatialBlend()
        {
            DGUI.Line.Draw(false, ComponentColorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.SpatialBlend, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight); //spatial blend label
                               DGUI.Property.Draw(GetProperty(PropertyName.SpatialBlend), DGUI.Properties.SingleLineHeight);         //spatial blend property
                               GUILayout.Space(DGUI.Properties.Space());
                               if (DGUI.Button.IconButton.Reset(DGUI.Properties.SingleLineHeight, ComponentColorName))
                               {
                                   Undo.RecordObject(this, "Reset SpatialBlend");
                                   Target.SpatialBlend = SoundGroupData.DEFAULT_SPATIAL_BLEND;
                                   m_needsSave = true;
                               }

                               GUILayout.Space(DGUI.Properties.Space(2));
                           });
        }

        private void DrawPlayMode()
        {
            SerializedProperty mode = GetProperty(PropertyName.Mode);

            DGUI.Line.Draw(false, ComponentColorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.PlayMode, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight); //play mode label
                               EditorGUI.BeginChangeCheck();
                               DGUI.Property.Draw(mode, ComponentColorName, DGUI.Properties.SingleLineHeight); //play mode property
                               if (EditorGUI.EndChangeCheck()) m_needsSave = true;

                               GUILayout.Space(DGUI.Properties.Space(2));
                           });

            //PlayMode.Sequence Options
            AnimBool showPlayModeSequenceOptions = GetAnimBool(mode.propertyPath);
            showPlayModeSequenceOptions.target = mode.enumValueIndex == (int) SoundGroupData.PlayMode.Sequence;
            SerializedProperty resetSequence = GetProperty(PropertyName.ResetSequenceAfterInactiveTime);
            AnimBool resetSequenceExpanded = GetAnimBool(resetSequence.propertyPath, resetSequence.boolValue);
            resetSequenceExpanded.target = resetSequence.boolValue;

            if (DGUI.FadeOut.Begin(showPlayModeSequenceOptions, false))
            {
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Line.Draw(false, resetSequence.boolValue ? ComponentColorName : ColorName.White,
                               () =>
                               {
                                   EditorGUI.BeginChangeCheck();
                                   DGUI.Toggle.Switch.Draw(resetSequence, UILabels.AutoResetSequence, ComponentColorName, false, false); //draw auto reset sequence switch property
                                   if (EditorGUI.EndChangeCheck()) m_needsSave = true;

                                   if (DGUI.FadeOut.Begin(resetSequenceExpanded, false))
                                       DGUI.Line.Draw(false, ComponentColorName, false,
                                                      () =>
                                                      {
                                                          GUILayout.Space(-DGUI.Properties.Space() * resetSequenceExpanded.faded);
                                                          DGUI.Label.Draw(UILabels.After, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight); //draw 'after' label
                                                          EditorGUI.BeginChangeCheck();
                                                          DGUI.Property.Draw(GetProperty(PropertyName.SequenceResetTime), ColorName.White, //draw reset time property
                                                                             DGUI.Properties.DefaultFieldWidth * resetSequenceExpanded.faded, DGUI.Properties.SingleLineHeight);
                                                          if (EditorGUI.EndChangeCheck()) m_needsSave = true;
                                                          DGUI.Label.Draw(UILabels.Seconds, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight); //draw 'seconds' label
                                                          GUILayout.FlexibleSpace();
                                                      });

                                   DGUI.FadeOut.End(resetSequenceExpanded, false);
                                   GUILayout.FlexibleSpace();
                               });
            }

            DGUI.FadeOut.End(showPlayModeSequenceOptions, false);
        }

        private void DrawPlayer()
        {
            GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(0));
            Rect lastRect = GUILayoutUtility.GetLastRect();
            var playerRect = new Rect(lastRect.x + DGUI.Properties.Space(), lastRect.y, lastRect.width - DGUI.Properties.Space(2), DGUI.Properties.SingleLineHeight); //calculate player rect
            Player.DrawPlayer(playerRect, ComponentColorName);                                                                                                        //draw player
            GUILayout.Space(playerRect.height);
        }

        private void DrawSounds()
        {
            bool hasIssues = Target.HasMissingAudioClips;
            DGUI.Line.Draw(true, hasIssues ? ColorName.Red : ComponentColorName, true, DGUI.Properties.SingleLineHeight,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space());
                               if (hasIssues)
                               {
                                   DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconError), DGUI.Properties.SingleLineHeight * 0.9f, DGUI.Properties.SingleLineHeight, Color.red); //draw error icon (if needed)
                                   GUILayout.Space(DGUI.Properties.Space(3));
                               }

                               DGUI.Label.Draw(UILabels.Sounds, Size.XL, hasIssues ? ColorName.Red : ComponentColorName, DGUI.Properties.SingleLineHeight); //draw 'SOUNDS' label
                           });

            GUILayout.Space(DGUI.Properties.Space());

            SerializedProperty sounds = GetProperty(PropertyName.Sounds);
            bool enabledState = GUI.enabled;
            if (sounds.arraySize == 0)
            {
                DGUI.Line.Draw(false, ColorName.Red,
                               () =>
                               {
                                   GUILayout.Space(DGUI.Properties.Space(2));
                                   GUI.enabled = false;
                                   DGUI.Label.Draw(UILabels.NoSoundsHaveBeenAdded + "...", Size.S, DGUI.Colors.BaseDColor(), DGUI.Properties.SingleLineHeight); //draw 'No sounds have been added...' label
                                   GUI.enabled = enabledState;
                                   GUILayout.FlexibleSpace();
                                   DrawDropZone();
                                   if (DGUI.Button.IconButton.Plus(DGUI.Properties.SingleLineHeight)) //draw PLUS button
                                   {
                                       Undo.RecordObject(this, "Add AudioData");
                                       Target.Sounds.Add(new AudioData());
                                       Data.SetDirty(false);
                                       m_needsSave = true;
                                   }
                               });
                return;
            }

//            float backgroundHeight = DGUI.Properties.Space() + DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(); //plus button height
            float backgroundHeight = -DGUI.Properties.SingleLineHeight + DGUI.Properties.Space();

            for (int i = 0; i < sounds.arraySize; i++)
            {
                backgroundHeight += EditorGUI.GetPropertyHeight(sounds.GetArrayElementAtIndex(i));
                backgroundHeight += DGUI.Properties.Space(2);
            }

            DGUI.Background.Draw(ComponentColorName, GUILayout.Height(backgroundHeight));
            GUILayout.Space(-backgroundHeight);
            GUILayout.Space(DGUI.Properties.Space());

            for (int i = 0; i < sounds.arraySize; i++)
            {
                SerializedProperty audioDataProperty = sounds.GetArrayElementAtIndex(i);
                SerializedProperty audioClipProperty = GetProperty(PropertyName.AudioClip, audioDataProperty);
                float propertyHeight = EditorGUI.GetPropertyHeight(audioClipProperty);

                bool isPlaying = Player.IsPlaying && Player.AudioSource.clip == audioClipProperty.objectReferenceValue;
                bool hasAudioClip = audioClipProperty.objectReferenceValue != null;

                DGUI.Line.Draw(false, ComponentColorName, false, propertyHeight,
                               () =>
                               {
                                   GUILayout.Space(DGUI.Properties.Space());
                                   if (hasAudioClip)
                                   {
                                       GUILayout.Space(DGUI.Properties.Space());
                                       if (isPlaying)
                                       {
                                           if (DGUI.Button.IconButton.Stop(DGUI.Properties.SingleLineHeight, ColorName.Red)) //stop button
                                               Player.Stop();
                                       }
                                       else
                                       {
                                           if (DGUI.Button.IconButton.Play(DGUI.Properties.SingleLineHeight, DGUI.Colors.DisabledTextColorName)) //play button
                                               Player.Play((AudioClip) audioClipProperty.objectReferenceValue);
                                       }

                                       GUILayout.Space(-DGUI.Properties.Space());
                                   }
                                   else
                                   {
                                       GUILayout.Space(DGUI.Properties.Space(2));
                                       DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconMuteSound), DGUI.Properties.SingleLineHeight, DGUI.Properties.SingleLineHeight, ColorName.Red); //mute icon
                                       GUILayout.Space(DGUI.Properties.Space());
                                   }


                                   DGUI.Property.Draw(audioClipProperty, !hasAudioClip ? ColorName.Red : isPlaying ? ComponentColorName : ColorName.White, DGUI.Properties.SingleLineHeight); //audio clip property
                                   if (hasAudioClip)
                                   {
                                       if (DGUI.Button.Draw(DGUI.Properties.Labels.SetAsSoundName, Size.S, ComponentColorName, false, DGUI.Properties.SingleLineHeight)) //'Set as Sound Name' button
                                       {
                                           Undo.RecordObject(this, "Update Sound Name");
                                           GetProperty(PropertyName.SoundName).stringValue = audioClipProperty.objectReferenceValue.name;
                                           m_soundNameHasBeenChanged = true;
                                           Data.SetDirty(true);
                                       }
                                       
                                       GUILayout.Space(DGUI.Properties.Space());
                                   }

                                   if (DGUI.Button.IconButton.Minus(propertyHeight)) //minus button
                                   {
                                       sounds.DeleteArrayElementAtIndex(i);
                                       Data.SetDirty(false);
                                       m_needsSave = true;
                                   }
                               });
            }

            DGUI.Line.Draw(false,
                           () =>
                           {
                               GUILayout.FlexibleSpace();
                               DrawDropZone();
                               if (DGUI.Button.IconButton.Plus(DGUI.Properties.SingleLineHeight)) //plus button
                               {
//                                   sounds.InsertArrayElementAtIndex(sounds.arraySize);
                                   Undo.RecordObject(this, "Add AudioData");
                                   Target.Sounds.Add(new AudioData());
                                   Data.SetDirty(false);
                                   m_needsSave = true;
                               }
                           });


        }

        private void DrawDropZone()
        {
            GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(0));
            Rect lastRect = GUILayoutUtility.GetLastRect();
            var dropRect = new Rect(lastRect.x + DGUI.Properties.Space(), lastRect.y - 2, DGUI.Properties.DefaultFieldWidth * 5, DGUI.Properties.SingleLineHeight); //calculate rect
           
            bool containsMouse = dropRect.Contains(Event.current.mousePosition);
            if (containsMouse)
            {
                DGUI.Colors.SetNormalGUIColorAlpha();

                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        bool containsAudioClip = DragAndDrop.objectReferences.OfType<AudioClip>().Any();
                        DragAndDrop.visualMode = containsAudioClip ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                        Event.current.Use();
                        Repaint();
                        break;
                    case EventType.DragPerform:
                        IEnumerable<AudioClip> audioClips = DragAndDrop.objectReferences.OfType<AudioClip>();
                        bool undoRecorded = false;
                        foreach (AudioClip audioClip in audioClips)
                        {
                            if (Target.Contains(audioClip)) continue;
                            if (!undoRecorded)
                            {
                                DoozyUtils.UndoRecordObject(Target, UILabels.AddSounds);
                                undoRecorded = true;
                            }

                            Target.Sounds.Add(new AudioData(audioClip));
                            Target.SetDirty(false);
                        }

                        Event.current.Use();
                        Repaint();
                        break;
                }
            }

            DGUI.Doozy.DrawDropZone(dropRect, containsMouse);
            GUILayout.Space(dropRect.width - DGUI.Properties.Space(2));
        }      
    }
}