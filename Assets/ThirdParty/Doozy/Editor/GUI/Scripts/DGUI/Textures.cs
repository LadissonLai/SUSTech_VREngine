// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Textures
        {
            /// <summary> Gets the texture at the given path and with the given fileName and extension, from the AssetDatabase </summary>
            /// <param name="filePath"> The file path </param>
            /// <param name="fileName"> SocketName of the file </param>
            /// <param name="fileExtension"> The file extension </param>
            public static Texture GetTexture(string filePath, string fileName, string fileExtension = ".png") { return AssetDatabase.LoadAssetAtPath<Texture>(filePath + fileName + fileExtension); }

            public static Texture2D CreateColoredTexture(int width, int height, Color color)
            {
                var pixels = new Color[width * height];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
                var result = new Texture2D(width, height);
                result.SetPixels(pixels);
                result.Apply();
                return result;
            }

            /// <summary> Generates a grid texture wit the given line and background colors </summary>
            /// <param name="lineColor">Color of the line.</param>
            /// <param name="backgroundColor">Color of the background.</param>
            public static Texture2D CreateGridTexture(Color lineColor, Color backgroundColor)
            {
                var texture = new Texture2D(64, 64);
                var colors = new Color[64 * 64];
                for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                {
                    Color color = backgroundColor;
                    if (y % 16 == 0 || x % 16 == 0) color = Color.Lerp(lineColor, backgroundColor, 0.65f);
                    if (y == 63 || x == 63) color = Color.Lerp(lineColor, backgroundColor, 0.35f);
                    colors[y * 64 + x] = color;
                }

                texture.SetPixels(colors);
                texture.wrapMode = TextureWrapMode.Repeat;
                texture.filterMode = FilterMode.Bilinear;
                texture.name = "Grid";
                texture.Apply();
                return texture;
            }

            /// <summary> Generates a grid cross texture with the given line color </summary>
            /// <param name="lineColor">Color of the line.</param>
            public static Texture2D CreateGridCrossTexture(Color lineColor)
            {
                var texture = new Texture2D(64, 64);
                var colors = new Color[64 * 64];
                for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                {
                    Color color = lineColor;
                    if (y != 31 && x != 31) color.a = 0;
                    colors[y * 64 + x] = color;
                }

                texture.SetPixels(colors);
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Bilinear;
                texture.name = "GridCross";
                texture.Apply();
                return texture;
            }
        }
    }
}