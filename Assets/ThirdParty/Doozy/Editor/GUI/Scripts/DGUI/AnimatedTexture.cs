// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.IO;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class AnimatedTexture
        {
            private const int FRAME_COUNT = 30;

            public static Texture[] GetAnimatedTextureArray(string textureName, string basePath, bool skinDependent = true, int frameCount = FRAME_COUNT)
            {
                var array = new Texture[frameCount];

                string folderPath = Path.Combine(basePath, textureName);
                string skinPath = Path.Combine(folderPath, (Utility.IsProSkin ? Skin.Dark : Skin.Light).ToString());

                for (int i = 0; i < array.Length; i++)
                    array[i] = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(skinDependent ? skinPath : folderPath, i + 1 + ".png"));
                return array;
            }

            public static void Draw(AnimBool expanded, Texture[] textures, float size, ColorName colorName) { Draw(expanded, textures, size, size, colorName); }
            public static void Draw(AnimBool expanded, Texture[] textures, float size, DColor dColor) { Draw(expanded, textures, size, size, dColor); }
            public static void Draw(AnimBool expanded, Texture[] textures, float size, Color color) { Draw(expanded, textures, size, size, color); }
            public static void Draw(AnimBool expanded, Texture[] textures, float size) { Draw(expanded, textures, size, size); }

            public static void Draw(AnimBool expanded, Texture[] textures, float width, float height, ColorName colorName) { Draw(expanded, textures, width, height, Colors.IconColor(colorName)); }
            public static void Draw(AnimBool expanded, Texture[] textures, float width, float height, DColor dColor) { Draw(expanded, textures, width, height, Colors.IconColor(dColor)); }

            public static void Draw(AnimBool expanded, Texture[] textures, float width, float height, Color color)
            {
                Color initialColor = GUI.color;
                GUI.color = color;
                Draw(expanded, textures, width, height);
                GUI.color = initialColor;
            }

            public static void Draw(AnimBool expanded, Texture[] textures, float width, float height)
            {
                int index = Mathf.RoundToInt(expanded.faded * (textures.Length - 1));
                GUILayout.Label(textures[index], GUILayout.Width(width), GUILayout.Height(height));
            }
        }
    }
}