using UnityEngine;

namespace Framework
{
    public static partial class ExtendFunc
    {
        public static void Clear(this RenderTexture rt , bool clearDepth, bool clearColor, Color backgroundColor)
        {
            var activeRT = RenderTexture.active;

            RenderTexture.active = rt;

            GL.Clear(clearDepth, clearColor, backgroundColor);

            RenderTexture.active = rt;
        }

        /// <summary>
        /// Clip by 4 points.  
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="RectPoints">bl,tl,tr,br</param>
        /// <returns></returns>
        public static Texture2D Clip(this Texture2D texture , Vector2[] rectPoints)
        {
            var rectSize = rectPoints[2] - rectPoints[0];

            var offset = new Vector2(rectPoints[0].x / texture.width , rectPoints[0].y / texture.height);

            var scale = new Vector2(rectSize.x / texture.width , rectSize.y / texture.height);

            return Clip(texture, offset, scale);
        }

        /// <summary>
        /// Clip by offset and scale.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="offset"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Texture2D Clip(this Texture2D texture , Vector2 offset , Vector2 scale)
        {
            offset.x = offset.x < 0 ? 0 : offset.x;

            offset.y = offset.y < 0 ? 0 : offset.y;

            scale.x = offset.x + scale.x > 1 ? 1 - offset.x : scale.x;

            scale.y = offset.y + scale.y > 1 ? 1 - offset.y : scale.y; 
            
            var tmpRT = RenderTexture.GetTemporary((int)(texture.width * scale.x), (int)(texture.height * scale.y), 0);

            var activeRT = RenderTexture.active;

            var resTex = new Texture2D(tmpRT.width, tmpRT.height, texture.format, false);

            RenderTexture.active = tmpRT;

            Graphics.Blit(texture, tmpRT , scale , offset);

            resTex.ReadPixels(new Rect(0, 0, resTex.width, resTex.height), 0, 0);

            resTex.Apply();

            RenderTexture.active = activeRT;

            RenderTexture.ReleaseTemporary(tmpRT);

            return resTex;
        }

        public static Texture2D Downsampling(this Texture2D texture , float downscale)
        {
            if (downscale == 1)
            {
                return texture;
            }

            Vector2Int destSize = new Vector2Int((int)(texture.width * downscale), (int)(texture.height * downscale));

            return Downsampling(texture, destSize);
        }

        /// <summary>
        /// 降采样
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="destSize"></param>
        /// <returns></returns>
        public static Texture2D Downsampling(this Texture2D texture , Vector2Int destSize)
        {
            var tmpRT = RenderTexture.GetTemporary(destSize.x, destSize.y , 0);

            var activeRT = RenderTexture.active;

            var resTex = new Texture2D(destSize.x, destSize.y, texture.format, false);

            RenderTexture.active = tmpRT;

            Graphics.Blit(texture, tmpRT);

            resTex.ReadPixels(new Rect(0, 0, destSize.x, destSize.y), 0, 0);

            resTex.Apply();

            RenderTexture.active = activeRT;

            RenderTexture.ReleaseTemporary(tmpRT);

            return resTex;
        }

        public static byte[] ReadPixels(this Texture2D texture, RenderTexture rt , Rect rect , int destX , int destY)
        {
            var activeRT = RenderTexture.active;

            RenderTexture.active = rt;

            texture.ReadPixels(rect , destX , destY);

            RenderTexture.active = activeRT;

            return texture.GetRawTextureData();
        }
    }
}