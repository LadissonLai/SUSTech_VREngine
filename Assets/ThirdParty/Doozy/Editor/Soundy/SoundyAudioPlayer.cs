// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor;
using Doozy.Engine.Extensions;
using Doozy.Engine.Soundy;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Doozy.Editor.Soundy
{
    public static class SoundyAudioPlayer
    {
        private static readonly Dictionary<SoundGroupData, Player> PlayerAudioDataDatabase = new Dictionary<SoundGroupData, Player>();
        private static readonly Dictionary<AudioClip, Player> PlayerAudioClipDatabase = new Dictionary<AudioClip, Player>();

        public static Player GetPlayer(SoundGroupData soundGroupData, AudioMixerGroup audioMixerGroup = null)
        {
            if (soundGroupData == null) return null;
            if (PlayerAudioDataDatabase.ContainsKey(soundGroupData))
            {
                if (audioMixerGroup != null) PlayerAudioDataDatabase[soundGroupData].UpdateAudioMixerGroup(audioMixerGroup);
                return PlayerAudioDataDatabase[soundGroupData];
            }

            var player = new Player(soundGroupData, audioMixerGroup);
            PlayerAudioDataDatabase.Add(soundGroupData, player);
            return player;
        }

        public static Player GetPlayer(AudioClip audioClip, AudioMixerGroup audioMixerGroup = null)
        {
            if (audioClip == null) return null;
            if (PlayerAudioClipDatabase.ContainsKey(audioClip))
            {
                if (audioMixerGroup != null) PlayerAudioClipDatabase[audioClip].UpdateAudioMixerGroup(audioMixerGroup);
                return PlayerAudioClipDatabase[audioClip];
            }

            var player = new Player(audioClip, audioMixerGroup);
            PlayerAudioClipDatabase.Add(audioClip, player);
            return player;
        }

        public static void StopAllPlayers()
        {
            foreach (Player player in PlayerAudioDataDatabase.Values)
                player.Stop();
            PlayerAudioDataDatabase.Clear();

            foreach (Player player in PlayerAudioClipDatabase.Values)
                player.Stop();
            PlayerAudioClipDatabase.Clear();
        }

        public class Player
        {
            public Player(SoundGroupData soundGroupData, AudioMixerGroup outputAudioMixerGroup)
            {
                m_soundGroupData = soundGroupData;
                m_outputAudioMixerGroup = outputAudioMixerGroup;
                m_soundSource = SoundSource.Soundy;
            }

            public Player(AudioClip audioClip, AudioMixerGroup outputAudioMixerGroup)
            {
                m_audioClip = audioClip;
                m_outputAudioMixerGroup = outputAudioMixerGroup;
                m_soundSource = SoundSource.AudioClip;
            }

            private SoundSource m_soundSource;

            private SoundGroupData m_soundGroupData;
            private AudioClip m_audioClip;
            private AudioMixerGroup m_outputAudioMixerGroup;


            public SoundGroupData SoundGroupData { get { return m_soundGroupData; } }
            public AudioClip AudioClip { get { return m_audioClip; } }
            public AudioMixerGroup OutputAudioMixerGroup { get { return m_outputAudioMixerGroup; } }


            private AudioSource m_audioSource;

            public AudioSource AudioSource
            {
                get
                {
                    if (m_audioSource != null) return m_audioSource;
                    m_audioSource = EditorUtility.CreateGameObjectWithHideFlags("Soundy Player", HideFlags.DontSave, typeof(AudioSource)).GetComponent<AudioSource>();
                    return m_audioSource;
                }
            }

            public string OutputAudioMixerGroupName { get { return OutputAudioMixerGroup == null ? "No Output AudioMixerGroup" : OutputAudioMixerGroup.name + " (" + OutputAudioMixerGroup.audioMixer.name + ")"; } }

            public bool IsPlaying { get { return m_audioSource != null && m_audioSource.isPlaying; } }
            public bool IsPaused { get; set; }
            
            
            public float Progress { get { return m_audioSource != null && (IsPlaying || IsPaused) ? (float) Math.Round(m_audioSource.time / m_audioSource.clip.length, 3) : 0; } }


            public float PlaybackTimeMinutes { get { return m_audioSource != null && (IsPlaying || IsPaused) ? GetMinutes(m_audioSource.time) : 0; } }
            public float PlaybackTimeSeconds { get { return m_audioSource != null && (IsPlaying || IsPaused) ? GetSeconds(m_audioSource.time) : 0; } }
            public float ClipLengthMinutes { get { return m_audioSource != null && m_audioSource.clip != null && (IsPlaying || IsPaused) ? GetMinutes(m_audioSource.clip.length) : 0; } }
            public float ClipLengthSeconds { get { return m_audioSource != null && m_audioSource.clip != null && (IsPlaying || IsPaused) ? GetSeconds(m_audioSource.clip.length) : 0; } }

            public string PlaybackTimeLabel { get { return GetTimePretty(PlaybackTimeMinutes, PlaybackTimeSeconds); } }
            public string ClipLengthLabel { get { return GetTimePretty(ClipLengthMinutes, ClipLengthSeconds); } }
            public string ClipName { get { return m_audioSource.clip.name; } }
            public string DurationLabel { get { return "(" + PlaybackTimeLabel + " / " + ClipLengthLabel + ")"; } }
            public string ProgressLabel { get { return ClipName + " - " + DurationLabel; } }


            private static float GetMinutes(float seconds) { return Mathf.Floor(seconds / 60); }
            private static float GetSeconds(float seconds) { return Mathf.RoundToInt(seconds % 60); }

            private static string GetTimePretty(float seconds) { return GetTimePretty(GetMinutes(seconds), GetSeconds(seconds)); }
            private static string GetTimePretty(float minutes, float seconds) { return (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds; }

            private float ProgressBarAlpha { get { return m_audioSource != null && m_audioSource.isPlaying ? 1f : 0; } }

            private float ProgressBarWidth(float rowWidth) { return Progress > 0 ? Mathf.Clamp(rowWidth * Progress, DGUI.Properties.Space(4), rowWidth) : 0; }

            private bool CanPlay
            {
                get
                {
                    switch (m_soundSource)
                    {
                        case SoundSource.Soundy:
                            return m_soundGroupData != null && m_soundGroupData.HasSound;
                        case SoundSource.AudioClip:
                            return m_audioClip != null;
                        case SoundSource.MasterAudio:
                            return false;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public void Play(AudioClip audioClip = null)
            {
                switch (m_soundSource)
                {
                    case SoundSource.Soundy:
                        if (!CanPlay && audioClip == null) return;
                        if (IsPlaying || IsPaused) Stop();
                        m_soundGroupData.PlaySoundPreview(AudioSource, m_outputAudioMixerGroup, audioClip);
                        IsPaused = false;
                        break;
                    case SoundSource.AudioClip:
                        if (!CanPlay) return;
                        if (IsPlaying || IsPaused) Stop();
                        AudioSource.clip = m_audioClip;
                        AudioSource.Play();
                        IsPaused = false;
                        break;
                    case SoundSource.MasterAudio: break;
                    default:                 throw new ArgumentOutOfRangeException();
                }
            }

            public void Stop()
            {
                switch (m_soundSource)
                {
                    case SoundSource.Soundy:
                    case SoundSource.AudioClip:
                        if (m_audioSource == null) return;
                        m_audioSource.Stop();
                        Object.DestroyImmediate(m_audioSource.gameObject);
                        m_audioSource = null;
                        IsPaused = false;
                        break;
                    case SoundSource.MasterAudio: break;
                    default:                 throw new ArgumentOutOfRangeException();
                }
            }

            public void DrawPlayer(Rect rect, ColorName colorName)
            {
                if (m_audioSource != null && !IsPlaying && !IsPaused)
                {
                    Object.DestroyImmediate(m_audioSource.gameObject);
                    m_audioSource = null;
                    return;
                }

                var progressMouseDetectionArea = new Rect(rect.x + DGUI.Properties.Space(),
                                                          rect.y,
                                                          rect.width - DGUI.Properties.Space(7) - DGUI.Button.IconButton.Width * 2,
                                                          rect.height);

                bool mouseInsideScrubber = progressMouseDetectionArea.Contains(Event.current.mousePosition);
                
                string audioName = "";
                switch (m_soundSource)
                {
                    case SoundSource.Soundy:
                        if (m_soundGroupData == null || m_soundGroupData.SoundName.Equals(SoundyManager.NO_SOUND)) audioName = SoundyManager.NO_SOUND;
                        else if (m_audioSource != null && m_audioSource.clip != null && (IsPlaying || IsPaused)) audioName = m_audioSource.clip.name;
                        else audioName = "---";
                        break;
                    case SoundSource.AudioClip:
                        if (m_audioSource != null && m_audioSource.clip != null && (IsPlaying || IsPaused)) audioName = m_audioSource.clip.name;
                        else audioName = "---";
                        break;
                    case SoundSource.MasterAudio:
                        audioName = "[Master Audio]";
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                var audioNameContent = new GUIContent(audioName);
                Vector2 audioNameContentSize = DGUI.Label.Style(Size.S).CalcSize(audioNameContent);

                var audioNameLabelRect = new Rect(rect.x + DGUI.Properties.Space(),
                                                  rect.y + (rect.height - audioNameContentSize.y) / 2 - DGUI.Properties.Space(),
                                                  audioNameContentSize.x,
                                                  audioNameContentSize.y);

                var durationContent = new GUIContent(DurationLabel);
                Vector2 durationContentSize = DGUI.Label.Style(Size.S).CalcSize(durationContent);

                var durationLabelRect = new Rect(rect.x + rect.width - durationContentSize.x - DGUI.Button.IconButton.Width * 2 - DGUI.Properties.Space(6),
                                                 rect.y + (rect.height - durationContentSize.y) / 2 - DGUI.Properties.Space(),
                                                 durationContentSize.x,
                                                 durationContentSize.y);

//                float barHeight = mouseInsideScrubber ? DGUI.Properties.Space(2) : DGUI.Properties.Space();
                float barHeight = DGUI.Properties.Space();
                
                var barBackgroundRect = new Rect(rect.x + DGUI.Properties.Space(),
                                                 durationLabelRect.y + durationLabelRect.height + DGUI.Properties.Space(),
                                                 rect.width - DGUI.Properties.Space(7) - DGUI.Button.IconButton.Width * 2,
                                                 barHeight);

                var barProgressRect = new Rect(barBackgroundRect.x, barBackgroundRect.y, ProgressBarWidth(barBackgroundRect.width), barBackgroundRect.height);


                var stopButtonRect = new Rect(rect.x + rect.width - DGUI.Button.IconButton.Width - DGUI.Properties.Space(),
                                              rect.y + (rect.height - DGUI.Button.IconButton.Height) / 2,
                                              DGUI.Button.IconButton.Width,
                                              DGUI.Button.IconButton.Height);

                var playButtonRect = new Rect(stopButtonRect.x - DGUI.Button.IconButton.Width - DGUI.Properties.Space(),
                                              rect.y + (rect.height - DGUI.Button.IconButton.Height) / 2,
                                              DGUI.Button.IconButton.Width,
                                              DGUI.Button.IconButton.Height);


                Color initialColor = GUI.color;
                Color contentColor = DGUI.Utility.IsProSkin ? Color.white : Color.black;
                Color backgroundColor = DGUI.Utility.IsProSkin ? Color.black : Color.white;

                GUI.color = backgroundColor.WithAlpha(0.2f);
                GUI.Label(barBackgroundRect, GUIContent.none, DGUI.Properties.White); //draw progress bar background
                GUI.color = initialColor;

                GUI.color = DGUI.Colors.IconColor(colorName);
                GUI.Label(barProgressRect, GUIContent.none, DGUI.Properties.White); //draw progress bar (dynamic width)
                GUI.color = initialColor;

                Color textColor = IsPlaying || IsPaused ? DGUI.Colors.TextColor(colorName) : contentColor.WithAlpha(0.4f);
                GUI.Label(audioNameLabelRect, audioNameContent, new GUIStyle(DGUI.Label.Style(Size.S)) {normal = {textColor = textColor}}); //draw audio clip name label -- to the left
                GUI.Label(durationLabelRect, durationContent, new GUIStyle(DGUI.Label.Style(Size.S)) {normal = {textColor = textColor}}); //draw duration (current time/total time) -- to the right

                GUI.color = DGUI.Colors.TextColor(colorName);
                DrawPlayButton(playButtonRect); //draw play button
                DrawStopButton(stopButtonRect); //draw stop button
                GUI.color = initialColor;

                //keep this here -- quick visualize the mouse detection area for the scrubber (it's bigger than needed to make it more user friendly)
                {
//                GUI.color = Color.yellow.WithAlpha(0.5f);
//                GUI.Label(progressMouseDetectionArea, GUIContent.none, DGUI.Properties.White);
//                GUI.color = initialColor;
                }

                if (!mouseInsideScrubber || !GUI.enabled) return;
                Vector2 mousePosition = Event.current.mousePosition; //get mouse position (for easy core reading)
                float virtualScrubberLength = Mathf.Abs(progressMouseDetectionArea.x - mousePosition.x); //calculate scrubber length (the dark overlay bar)
                float scrubberHeight = barHeight + DGUI.Properties.Space(4); //calculate scrubber height (the pointer)
                float progressMousePosition = virtualScrubberLength / progressMouseDetectionArea.width; //calculate the progress at the mouse position into a linear percentage value from 0 to 1

                //draw virtual timeline
                GUI.color = backgroundColor.WithAlpha(0.4f);
                GUI.Label(new Rect(barBackgroundRect.x, barBackgroundRect.y, virtualScrubberLength, barHeight), GUIContent.none, DGUI.Properties.White); //draws the dark overlay bar (from the start to the mouse position)
                GUI.color = initialColor;

                //draw scrubber
                GUI.color = DGUI.Colors.IconColor(colorName).WithAlpha(0.8f);
                GUI.Label(new Rect(mousePosition.x, barBackgroundRect.y + (barHeight - scrubberHeight) / 2, DGUI.Properties.Space(2), scrubberHeight), GUIContent.none,
                          DGUI.Properties.White); //draw the scrubber at the mouse position (only on x)
                GUI.color = initialColor;

                //draw scrubber time
                if (IsPlaying || IsPaused)
                {
                    var scrubberTimeContent = new GUIContent(GetTimePretty(AudioSource.clip.length * progressMousePosition)); //get the scrubber position time value in a pretty format 12:34
                    Vector2 scrubberTimeContentSize = DGUI.Label.Style(Size.S).CalcSize(scrubberTimeContent); //calculate the time label size
                    var scrubberTimeLabelRect = new Rect(mousePosition.x - scrubberTimeContentSize.x / 2 + DGUI.Properties.Space(), //calculate the time label rect
                                                         barBackgroundRect.y - scrubberTimeContentSize.y - DGUI.Properties.Space(2),
                                                         scrubberTimeContentSize.x,
                                                         scrubberTimeContentSize.y);
                    var scrubberTimeBackgroundRect = new Rect(scrubberTimeLabelRect.x - DGUI.Properties.Space(), //calculate a background for the time label to make it easy to read when hovering over other text 
                                                              scrubberTimeLabelRect.y - DGUI.Properties.Space(),
                                                              scrubberTimeLabelRect.width + DGUI.Properties.Space(2),
                                                              scrubberTimeLabelRect.height + DGUI.Properties.Space(2));

                    GUI.color = backgroundColor.WithAlpha(0.8f);
                    GUI.Label(scrubberTimeBackgroundRect, GUIContent.none, DGUI.Properties.White); //draw the time label background
                    GUI.color = initialColor;

                    GUI.Label(scrubberTimeLabelRect, scrubberTimeContent, new GUIStyle(DGUI.Label.Style(Size.S)) {normal = {textColor = DGUI.Colors.TextColor(colorName).WithAlpha(0.8f)}}); //draw the time label text
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0) //user left clicked on the progress bar
                {
                    if (!CanPlay) return; //this is a sanity check -> the player should not be visible if there are issues -> but we check this just in case
                    if (IsPaused)
                    {
                        AudioSource.UnPause();
                        IsPaused = false;
                    }
                    else if (!IsPlaying) //if the player is not playing -> start playing the sound
                        Play(); 
                    else //else scrub to the pointer position on the progress bar
                        AudioSource.time = AudioSource.clip.length * progressMousePosition; //set the AudioSource play time at the scrubber position (it's magic!!!) :))
                }
                else if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && (IsPlaying || IsPaused)) //if is playing and user performed right click on the bar -> stop playing the sound
                {
                    Stop();
                }
                else if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
                {
                    if (IsPaused)
                    {
                        AudioSource.UnPause();
                        IsPaused = false;
                    }
                    else
                    {
                        if (!IsPlaying) Play();
                        AudioSource.Pause();
                        IsPaused = true;
                    }
                }
            }

            public bool DrawPlayButton()
            {
                if (!DGUI.Button.IconButton.Play(-1, DGUI.Colors.DisabledTextColorName)) return false;
                Play();
                return true;
            }

            public bool DrawPlayButton(Rect rect)
            {
                if (!DGUI.Button.IconButton.Play(rect, DGUI.Colors.DisabledTextColorName)) return false;
                Play();
                return true;
            }

            public bool DrawStopButton()
            {
                if (!DGUI.Button.IconButton.Stop(IsPlaying ? Color.red :  DGUI.Colors.IconColor(DGUI.Colors.DisabledTextColorName))) return false;
                Stop();
                return true;
            }

            public bool DrawStopButton(Rect rect)
            {
                if (!DGUI.Button.IconButton.Stop(rect, IsPlaying ? Color.red :  DGUI.Colors.IconColor(DGUI.Colors.DisabledTextColorName))) return false;
                Stop();
                return true;
            }

            public void DrawProgressBar(float width, float height, UnityAction repaintCallback)
            {
                if (m_audioSource == null) return;

                if (m_audioSource != null && !m_audioSource.isPlaying)
                {
                    Object.DestroyImmediate(m_audioSource.gameObject);
                    m_audioSource = null;
                    return;
                }

                float currentProgressBarWidth = ProgressBarWidth(width);

                Color initialColor = GUI.color;
                GUILayout.BeginVertical();
                {
                    if (AudioSource.isPlaying) repaintCallback.Invoke();
//                    GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(0));
//                    if (Event.current.type == EventType.Repaint) m_progressBarWidth = GUILayoutUtility.GetLastRect().width;
                    GUI.color = initialColor.WithAlpha(ProgressBarAlpha);
                    GUILayout.Label(GUIContent.none, DGUI.Button.ButtonStyle(), GUILayout.Height(height), GUILayout.Width(currentProgressBarWidth));
                    GUILayout.Space(-height);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUI.color = Color.white.WithAlpha(0.4f);
                        DGUI.Label.Draw(ProgressLabel, Size.S, height);
                        GUILayout.Space(DGUI.Properties.Space(2));
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUI.color = initialColor;
            }

            public void UpdateAudioMixerGroup(AudioMixerGroup audioMixerGroup)
            {
                m_outputAudioMixerGroup = audioMixerGroup;
                if (m_audioSource != null) m_audioSource.outputAudioMixerGroup = audioMixerGroup;
            }
        }
    }
}