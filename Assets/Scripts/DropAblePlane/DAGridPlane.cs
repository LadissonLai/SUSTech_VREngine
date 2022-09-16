using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 网格平面
    /// </summary>
    public class DAGridPlane : MonoBehaviour
    {
        /// <summary>
        /// 网格尺寸
        /// </summary>
        [Tooltip("网格尺寸 单位米")]
        public float gridSize = 0.02f;

        /// <summary>
        /// 长宽格子数
        /// </summary>
        [Tooltip("平面自身尺寸 基于grid size")]
        public Vector2Int size = new Vector2Int(1,1);

        public float offset = -0.1f;
        
        public Vector2 GridCenter => new Vector2(gridSize * 0.5f, gridSize * 0.5f);

        public Vector2 PlaneSize => new Vector2(size.x * gridSize, size.y * gridSize);
         
        public Vector2 PlaneCenter => PlaneSize * 0.5f;

        public Vector3 PlaneWDir => transform.up;

        public Vector3 PlaneWOffset => PlaneWDir * offset;

        /// <summary>
        /// 格子坐标转平面坐标
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public Vector2 GridToPlanePos(Vector2Int gridPos)
        {
            return (Vector2)gridPos * gridSize + GridCenter;
        }
        
        /// <summary>
        /// 平面坐标转最近的格子坐标
        /// </summary>
        /// <param name="planePos"></param>
        /// <returns></returns>
        public Vector2Int PlanePosToGrid(Vector2 planePos)
        {
            var gridOffset = planePos / gridSize;
             
            return Vector2Int.FloorToInt(gridOffset);
        }

        /// <summary>
        /// 平面坐标转世界坐标
        /// </summary>
        /// <param name="planePos"></param>
        /// <returns></returns>
        public Vector3 PlanePosToWPos(Vector2 planePos)
        {
            var blPlanePos = planePos - PlaneCenter;

            var blPlanePosVec3 = new Vector3(blPlanePos.x, 0, blPlanePos.y);

            return transform.position + PlaneWOffset + transform.rotation * blPlanePosVec3;
        }
         
        /// <summary>
        /// 世界坐标投影到PlanePos
        /// </summary>
        /// <param name="wPos"></param>
        /// <returns></returns>
        public Vector2 WPosToPlanePos(Vector3 wPos)
        {
            var helpPlane = new Plane(transform.up, transform.position + PlaneWOffset);
             
            //投影到平面的世界坐标
            var projectWPos = helpPlane.ClosestPointOnPlane(wPos);

            Debug.DrawLine(wPos, projectWPos);

            //投影世界坐标转平面坐标
            var blPlanePosVec3 = Quaternion.Inverse(transform.rotation) * (projectWPos - transform.position - PlaneWOffset);

            var planePos = new Vector2(blPlanePosVec3.x, blPlanePosVec3.z) + PlaneCenter;

            return planePos;
        }

        public Vector3 GridToWPos(Vector2Int gridPos)
        {
            return PlanePosToWPos(GridToPlanePos(gridPos));
        }

        public Vector2Int WPosToGrid(Vector3 wPos)
        {
            return PlanePosToGrid(WPosToPlanePos(wPos));
        }

        /// <summary>
        /// 平面投影  不是特别智能，有优化空间。
        /// </summary>
        /// <param name="src">被投影的平面</param>
        /// <param name="dest">投影到平面</param>
        /// <returns>投影到dest身上的网格范围</returns>
        public static (Vector2Int startGrid, Vector2Int size) Project(DAGridPlane src, DAGridPlane dest)
        {
            var destPlanePos = dest.WPosToPlanePos(src.transform.position);

            var destCenterGrid = dest.PlanePosToGrid(destPlanePos);

            var startGrid = destCenterGrid - src.size / 2;

            var resSize = src.size;

            //也可以考虑此方法只管计算常规投影，其它的需求放到上层处理
            {
                var maxX = dest.size.x - 1;

                var minX = 1 - resSize.x;

                var maxY = dest.size.y - 1;

                var minY = 1 - resSize.y;

                if (startGrid.x > maxX || startGrid.x < minX || startGrid.y > maxY || startGrid.y < minY)
                {
                    //界外
                    resSize = Vector2Int.zero;
                }
                else
                {
                    startGrid.x = Mathf.Max(startGrid.x, 0);

                    startGrid.x = Mathf.Min(startGrid.x, dest.size.x - resSize.x);

                    startGrid.y = Mathf.Max(startGrid.y, 0);

                    startGrid.y = Mathf.Min(startGrid.y, dest.size.y - resSize.y);
                }
            }

            return (startGrid, resSize);
        }
         
#if UNITY_EDITOR
        public Color gizmosGridColor = Color.red;

        public bool gizmosDrawAways = false;

        protected Mesh gizmosMesh;
#endif

        protected virtual void OnDestroy()
        {
#if UNITY_EDITOR
            if (gizmosMesh != null)
            {
                if (Application.isPlaying)
                    Destroy(gizmosMesh);
                else
                    DestroyImmediate(gizmosMesh);
            }
#endif
        }

#if UNITY_EDITOR
        protected virtual void GenGizmosMesh()
        {
            if (gizmosMesh != null)
            {
                if (Application.isPlaying)
                    Destroy(gizmosMesh);
                else
                    DestroyImmediate(gizmosMesh);
            }

            gizmosMesh = new Mesh();

            var hVertexCount = size.x + 1;

            var vVertexCount = size.y + 1;

            var vertices = new Vector3[hVertexCount * vVertexCount];

            var triangles = new int[size.x * size.y * 6];

            var vertexIndex = 0;

            var triangleIndex = 0;

            for (var i = 0; i < vVertexCount; i++)
            {
                for (int j = 0; j < hVertexCount; j++ ,vertexIndex ++)
                {
                    vertices[vertexIndex] = new Vector3(j * gridSize, 0.0f, i * gridSize);

                    if(i + 1 < vVertexCount && j + 1 < hVertexCount)
                    {
                        var br = vertexIndex + 1;
                        var bl = vertexIndex;
                        var tl = vertexIndex + hVertexCount;
                        var tr = br + hVertexCount;

                        triangles[triangleIndex++] = br;
                        triangles[triangleIndex++] = bl;
                        triangles[triangleIndex++] = tl;

                        triangles[triangleIndex++] = tl;
                        triangles[triangleIndex++] = tr;
                        triangles[triangleIndex++] = br;
                    }
                }
            }

            gizmosMesh.SetVertices(vertices);

            gizmosMesh.SetTriangles(triangles, 0);

            gizmosMesh.RecalculateNormals();
        }

        protected virtual void DoGizmosDraw()
        {
            if (gizmosMesh != null)
                DestroyImmediate(gizmosMesh);

            if (gizmosMesh == null)
            {
                GenGizmosMesh();
            }

            var meshPos = PlanePosToWPos(Vector2.zero);
             
            var colorPre = Gizmos.color;

            Gizmos.color = gizmosGridColor;

            Gizmos.DrawWireMesh(gizmosMesh, meshPos, transform.rotation, Vector3.one);

            Gizmos.color = Color.green;

            Gizmos.DrawRay(transform.position, PlaneWOffset);

            Gizmos.color = colorPre;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (gizmosDrawAways)
                return;

            DoGizmosDraw();
        }

        protected virtual void OnDrawGizmos()
        {
            if (!gizmosDrawAways)
                return;

            DoGizmosDraw();
        }

#endif
    }
}


///// <summary>
///// 网格平面  TODO 尝试提炼出通用的数据结构
///// </summary>
//public struct GridPlane
//{
//    /// <summary>
//    /// 网格尺寸
//    /// </summary>
//    public float gridSize;

//    /// <summary>
//    /// 网格数量
//    /// </summary>
//    public Vector2Int grids;

//    public Vector2 GridCenter => new Vector2(gridSize * 0.5f, gridSize * 0.5f);

//    public Vector2 PlaneSize => new Vector2(grids.x * gridSize, grids.y * gridSize);

//    public Vector2 PlaneCenter => PlaneSize * 0.5f;

//    /// <summary>
//    /// 格子坐标转平面坐标
//    /// </summary>
//    /// <param name="gridPos"></param>
//    /// <returns></returns>
//    public Vector2 GridToPlanePos(Vector2Int gridPos)
//    {
//        return (Vector2)gridPos * gridSize + GridCenter;
//    }

//    /// <summary>
//    /// 平面坐标转最近的格子坐标
//    /// </summary>
//    /// <param name="planePos"></param>
//    /// <returns></returns>
//    public Vector2Int PlanePosToGrid(Vector2 planePos)
//    {
//        var gridOffset = planePos / gridSize;

//        return new Vector2Int(Mathf.FloorToInt(gridOffset.x), Mathf.FloorToInt(gridOffset.y));
//    }
//}
