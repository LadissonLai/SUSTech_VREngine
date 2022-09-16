using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fxb.CMSVR
{
    public class RTGridStateDrawer : MonoBehaviour, IGradStateRectDrawer
    {
        private static readonly int MAT_PROP_COLOR = Shader.PropertyToID("_Color");

        [System.Serializable]
        public struct Pen
        {
            public Material mat;

            //用quad mesh
            public Mesh mesh;

            public Color unusedGridColor;

            public Color usedGridColor;
        }

        private RenderTexture canvasRT;

        private int canvasGridSize;

        private Vector2Int canvasSize;

        private CommandBuffer cb;

        private Matrix4x4[] unusedMatrixArr;

        private Matrix4x4[] usedMatrixArr;

        private int unusedGridAmount;

        private int usedGridAmount;

        private MaterialPropertyBlock unusedMPB;

        private MaterialPropertyBlock usedMPB;

        public Material canvasMat;

        public Pen pen;

        Matrix4x4[] cacheMatrix;

        private void OnDestroy()
        {
            if (canvasRT != null)
                Destroy(canvasRT);
        }
         
        public void EndDraw()
        {
            if (unusedMPB == null)
            {
                unusedMPB = new MaterialPropertyBlock();

                unusedMPB.SetColor(MAT_PROP_COLOR, pen.unusedGridColor);
            }

            if (usedMPB == null)
            {
                usedMPB = new MaterialPropertyBlock();

                usedMPB.SetColor(MAT_PROP_COLOR, pen.usedGridColor);
            }

            cb.Clear();
            cb.SetRenderTarget(canvasRT);
            cb.ClearRenderTarget(false, true, Color.clear);

            if (unusedGridAmount > 0)
                DoDrawGrid(unusedGridAmount, unusedMatrixArr, unusedMPB);

            if (usedGridAmount > 0)
            {
                pen.mat.SetColor(MAT_PROP_COLOR, pen.usedGridColor);

                DoDrawGrid(usedGridAmount, usedMatrixArr, usedMPB);
            }

            Graphics.ExecuteCommandBuffer(cb);

            usedGridAmount = unusedGridAmount = 0;
        }

        void DoDrawGrid(int drawAmount, Matrix4x4[] drawMtx, MaterialPropertyBlock propertyBlock)
        {
            int Amount = drawAmount;

            int drawCounter = 0;

            while (Amount > 1023)
            {
                Array.Copy(drawMtx, drawCounter * 1023, cacheMatrix, 0, 1023);

                cb.DrawMeshInstanced(pen.mesh, 0, pen.mat, 0, cacheMatrix, 1023, propertyBlock);

                Amount -= 1023;

                drawCounter++;
            }

            if (drawAmount > 1023)
            {
                Matrix4x4[] tMatrix = new Matrix4x4[Amount];

                Array.Copy(drawMtx, drawCounter * 1023, tMatrix, 0, Amount);

                cb.DrawMeshInstanced(pen.mesh, 0, pen.mat, 0, tMatrix, Amount, propertyBlock);

                return;
            }

            cb.DrawMeshInstanced(pen.mesh, 0, pen.mat, 0, drawMtx, drawAmount, propertyBlock);
        }

        private Matrix4x4 CalcObjToWorldMatrix(Vector2 canvasStart, Vector2 canvasRect)
        {
            return Matrix4x4.TRS(
                canvasStart / canvasSize,
                Quaternion.identity,
                canvasRect / canvasSize
            );
        }

        public void DrawRect((Vector2Int startGrid, Vector2Int size) rect, bool enable)
        {
            if (rect.size.x == 0)
                return;

            for (int i = rect.startGrid.x, maxX = rect.startGrid.x + rect.size.x; i < maxX; i++)
            {
                for (int j = rect.startGrid.y, maxY = rect.startGrid.y + rect.size.y; j < maxY; j++)
                {
                    DrawGridByState(i, j, enable);
                }
            }
        }

        public void DrawGridByState(int x, int y, bool enable)
        {
            //Debug.Log("")
            var canvasStart = new Vector2(x, y) * canvasGridSize;

            var canvasRect = Vector2.one * canvasGridSize;

            var matrix = CalcObjToWorldMatrix(canvasStart, canvasRect);

            if (enable)
                unusedMatrixArr[unusedGridAmount++] = matrix;
            else
                usedMatrixArr[usedGridAmount++] = matrix;
        }

        public void SetupGrids(Vector2Int size)
        {
            if (canvasRT != null)
                Destroy(canvasRT);

            canvasGridSize = 20;

            canvasSize = new Vector2Int(size.x * canvasGridSize, size.y * canvasGridSize);

            canvasRT = new RenderTexture(canvasSize.x, canvasSize.y, 0, RenderTextureFormat.ARGB32);

            canvasMat.mainTexture = canvasRT;

            unusedMatrixArr = new Matrix4x4[size.x * size.y];

            usedMatrixArr = new Matrix4x4[unusedMatrixArr.Length];

            if (cb == null)
            {
                cb = new CommandBuffer
                {
                    name = "TestCB"
                };
            }

            cacheMatrix = new Matrix4x4[1023];
        }
    }
}
