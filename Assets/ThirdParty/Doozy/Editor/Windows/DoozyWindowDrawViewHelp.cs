// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private const float LINK_BOX_HEIGHT = 32;
        private const float LINK_BOX_ICON_SIZE = 26;
        private const float LINK_BOX_PADDING = 8;
        private const float LINK_BOX_WIDTH = 256;

        private void DrawViewHelp()
        {
            if (CurrentView != View.Help) return;

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                //column 1 (LEFT)
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MaxWidth(GeneralColumnWidth));
                {
                    DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconDoozy, "DoozyUI", UILabels.HelpResources, DGUI.Colors.LightOrDarkColorName);
                    DrawDynamicViewVerticalSpace(0.5f);
                    DrawHelpLinkBox("Website", "doozyui.com", Styles.GetStyle(Styles.StyleName.IconDoozyUI), DGUI.Colors.LightOrDarkColorName, UILabels.Open, DoozySettings.LINK_WEBSITE_DOOZYUI);
                    DrawDynamicViewVerticalSpace(0.25f);
                    DrawHelpLinkBox("Documentation", "Manual and Script References", Styles.GetStyle(Styles.StyleName.IconFaBook), DGUI.Colors.LightOrDarkColorName, UILabels.Open, DoozySettings.LINK_WEBSITE_DOOZYUI_DOCUMENTATION);
                    DrawDynamicViewVerticalSpace(0.25f);
                    DrawHelpLinkBox("FAQ", "Frequently Asked Questions", Styles.GetStyle(Styles.StyleName.IconFaQuestion), DGUI.Colors.LightOrDarkColorName, UILabels.Open, DoozySettings.LINK_WEBSITE_DOOZYUI_DOCUMENTATION);
                    DrawDynamicViewVerticalSpace(0.25f);
                    DrawHelpLinkBox("Support Request", "Open a Support Ticket", Styles.GetStyle(Styles.StyleName.IconFaPaperPlane), DGUI.Colors.LightOrDarkColorName, UILabels.Open, DoozySettings.LINK_WEBSITE_DOOZYUI_SUPPORT_REQUEST);
                    DrawDynamicViewVerticalSpace(0.25f);
                    DrawHelpLinkBoxSupportEmail();
                }
                GUILayout.EndVertical();
                DrawDynamicViewVerticalSpace(2);
                //column 2 (RIGHT)
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MaxWidth(GeneralColumnWidth));
                {
                    DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconFaExternalLink, UILabels.OtherReferences, UILabels.UsefulLinks, DGUI.Colors.LightOrDarkColorName);
                    DrawDynamicViewVerticalSpace(0.5f);
                    DrawHelpLinkBox("Unity Manual", "Learn how to use the Unity Editor and its associated services", Styles.GetStyle(Styles.StyleName.IconFaBookOpen), ColorName.Gray, UILabels.Open, DoozySettings.LINK_WEBSITE_UNITY_MANUAL);
                    DrawDynamicViewVerticalSpace(0.25f);
                    DrawHelpLinkBox("Unity Scripting API", "Details of the scripting API that Unity provides", Styles.GetStyle(Styles.StyleName.IconFaBracketsCurly), ColorName.Gray, UILabels.Open, DoozySettings.LINK_WEBSITE_UNITY_SCRIPTING_API);
                    DrawDynamicViewVerticalSpace(0.25f);
                    DrawHelpLinkBox(".NET API Browser", "One-stop shop for all .NET-based APIs from Microsoft", Styles.GetStyle(Styles.StyleName.IconFaWindows), ColorName.Gray, UILabels.Open, DoozySettings.LINK_WEBSITE_MICROSOFT_DOT_NET_API);
                    DrawDynamicViewVerticalSpace(0.25f);
                    DrawHelpLinkBox("DOTween", "Fast, efficient, fully type-safe object-oriented animation engine", Styles.GetStyle(Styles.StyleName.IconFaChevronRight), ColorName.Gray, UILabels.Open, DoozySettings.LINK_WEBSITE_DOTWEEN);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            DrawDynamicViewVerticalSpace(2);

            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconFaHashtag, UILabels.SocialLinks, UILabels.GetInTouch, DGUI.Colors.LightOrDarkColorName);
            DrawDynamicViewVerticalSpace(0.5f);
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                //column 1 (LEFT)
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MaxWidth(GeneralColumnWidth));
                {
                    DrawHelpLinkBox("YouTube", "Doozy Entertainment Channel", Styles.GetStyle(Styles.StyleName.IconFaYoutube), ColorName.Red, UILabels.Open, DoozySettings.LINK_YOUTUBE_CHANNEL);
                    DrawDynamicViewVerticalSpace(0.25f);
                    DrawHelpLinkBox("Facebook", "Doozy Entertainment Page", Styles.GetStyle(Styles.StyleName.IconFaFacebook), ColorName.Blue, UILabels.Open, DoozySettings.LINK_FACEBOOK);
                }
                GUILayout.EndVertical();
                DrawDynamicViewVerticalSpace(2);
                //column 2 (RIGHT)
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MaxWidth(GeneralColumnWidth));
                {
                    DrawHelpLinkBox("Twitter", "@doozyplay", Styles.GetStyle(Styles.StyleName.IconFaTwitter), ColorName.LightBlue, UILabels.Open, DoozySettings.LINK_TWITTER);
                    DrawDynamicViewVerticalSpace(0.25f);
                    DrawHelpLinkBox("Discord", "https://discord.gg/y9Axq7b", Styles.GetStyle(Styles.StyleName.IconFaDiscord), ColorName.Gray, UILabels.Open, DoozySettings.LINK_DISCORD_INVITE);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            DrawDynamicViewVerticalSpace(2);
        }

        private static void DrawHelpLinkBoxSupportEmail()
        {
            string emailLink = DoozySettings.MAILTO +
                               DoozySettings.SUPPORT_EMAIL_ADDRESS +
                               MyEscapeURL("?subject=DoozyUI - Support Request" +
                                           "&body=" +
                                           "What Happened" +
                                           "%0D%0A" +
                                           "------------------" +
                                           "%0D%0A" +
                                           "%0D%0A" +
                                           "%0D%0A" +
                                           "How did it happen? " +
                                           "%0D%0A" +
                                           "------------------" +
                                           "%0D%0A" +
                                           "%0D%0A" +
                                           "%0D%0A" +
                                           "%0D%0A" +
                                           "%0D%0A" +
                                           "------------------" +
                                           "%0D%0A" +
                                           "Please attach links to any relevant files, screenshots or videos that can help describe the issue better." +
                                           "%0D%0A" +
                                           "------------------" +
                                           "%0D%0A" +
                                           "Unity Version: " + Application.unityVersion +
                                           "%0D%0A" +
                                           "Operating System: " + SystemInfo.operatingSystem +
                                           "%0D%0A" +
                                           "Device Model: " + SystemInfo.deviceModel +
                                           "%0D%0A" +
                                           "------------------" +
                                           "%0D%0A");


            DrawHelpLinkBox(UILabels.SupportEmail, DoozySettings.SUPPORT_EMAIL_ADDRESS, Styles.GetStyle(Styles.StyleName.IconFaEnvelope), DGUI.Colors.LightOrDarkColorName, UILabels.Email, emailLink);
        }

        private static void DrawHelpLinkBox(string linkName, string linkDescription, GUIStyle linkIcon, ColorName colorName, string linkButtonLabel, string linkUrl)
        {
            GUILayout.BeginVertical(DGUI.Background.Style(CornerType.Round), GUILayout.MinHeight(LINK_BOX_HEIGHT));
            {
                GUILayout.Space(LINK_BOX_PADDING);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(LINK_BOX_PADDING);
                    DGUI.Icon.Draw(linkIcon, LINK_BOX_ICON_SIZE, LINK_BOX_ICON_SIZE, colorName);
                    GUILayout.Space(LINK_BOX_PADDING);
                    GUILayout.BeginVertical();
                    {
                        DGUI.Label.Draw(linkName, Size.M, colorName);
                        GUILayout.Space(2);
                        GUI.enabled = !EditorApplication.isCompiling;
                        DGUI.WindowUtils.DrawSettingDescription(linkDescription);
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(LINK_BOX_PADDING);
                    GUILayout.FlexibleSpace();
                    if (DGUI.Button.Draw(linkButtonLabel, Size.S, DGUI.Colors.DarkOrLightColorName, DGUI.Colors.LightOrDarkColorName, true, LINK_BOX_ICON_SIZE, 80))
                    {
//                        DDebug.Log(linkUrl);
                        Application.OpenURL(linkUrl);
                    }

                    GUILayout.Space(LINK_BOX_PADDING);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUI.enabled = true;
        }

        private static string MyEscapeURL(string url) { return UnityWebRequest.EscapeURL(url).Replace("+", "%20"); }
    }
}