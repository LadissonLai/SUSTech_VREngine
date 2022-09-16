using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Fxb.CMSVR
{
    public interface IGradStateRectDrawer
    {
        void SetupGrids(Vector2Int size);
 
        void DrawRect((Vector2Int startGrid, Vector2Int size) rect, bool enable);

        void DrawGridByState(int x, int y, bool enable);

        void EndDraw();
    }

    /// <summary>
    /// 检测grad plane投影状态
    /// </summary>
    public class DADorpAblePlane : DAGridPlane
    {
        [System.Serializable]
        public class ProjectChecker
        {
            public DAGridPlane target;
             
            public (Vector2Int startGrid, Vector2Int size) projectResults;

            public bool canDrop;
        }

        public IGradStateRectDrawer drawer;

        private Dictionary<DAGridPlane, ProjectChecker> projectPlaneMap;

        private Dictionary<string, (Vector2Int startGrid, Vector2Int size)> gridUsedMap;

        /// <summary>
        /// false:未占用(可使用)  true:被占用
        /// </summary>
        private bool[,] gridStates;
         
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            gridSize = 0.02f;
 
            transform.localScale = new Vector3(PlaneSize.x, 1.0f, PlaneSize.y);
        }

        private void Awake()
        {
            gridStates = new bool[size.x, size.y];

            if (drawer == null)
                drawer = GetComponent<IGradStateRectDrawer>();

            if (drawer != null)
                drawer.SetupGrids(size);
        }

        private void Update()
        {
            //绘制当前检查的格子
            if (projectPlaneMap != null && projectPlaneMap.Count > 0)
            {
                foreach (var kv in projectPlaneMap)
                {
                    var checker = kv.Value;

                    if (checker.target == null)
                        continue;

                    var projectRect = Project(checker.target, this);

                    var hasInvalidGrid = false;
                    
                    //投影会保证范围，如果投影失败 size为0
                    if (projectRect.size == Vector2Int.zero)
                    {
                        hasInvalidGrid = true;
                    }
                    else
                    {
                        //TODO 需要避免网格被多次绘制，如 多个projectRect范围重叠的情况。
                        //Debug.Log($"startX:{projectRect.startGrid.x}  startY:{projectRect.startGrid.y}");
                        for (int i = projectRect.startGrid.x, maxX = projectRect.startGrid.x + projectRect.size.x; i < maxX; i++)
                        {
                            for (int j = projectRect.startGrid.y, maxY = projectRect.startGrid.y + projectRect.size.y; j < maxY; j++)
                            {
                                var gridHasUsed = gridStates[i, j];

                                if (gridHasUsed)
                                    hasInvalidGrid = true;

                                if (drawer != null)
                                    drawer.DrawGridByState(i, j, !gridHasUsed);
                            }
                        }
                    }

                    checker.projectResults = projectRect;

                    checker.canDrop = !hasInvalidGrid;
                }
            }

        }

        private void LateUpdate()
        {
            if (drawer != null)
                drawer.EndDraw();
        }
 
        private void GizmosDrawGrids((Vector2Int startGrid, Vector2Int size) result)
        {
            for (int i = result.startGrid.x; i < result.startGrid.x + result.size.x; i++)
            {
                for (int j = result.startGrid.y; j < result.startGrid.y + result.size.y; j++)
                {
                    var grid = new Vector2Int(i,j);

                    Gizmos.color = gridStates[i, j] ? Color.red : Color.cyan;
                     
                    Gizmos.DrawSphere(GridToWPos(grid), 0.002f);
                }
            }
        }

#if UNITY_EDITOR
        protected override void DoGizmosDraw()
        {
            base.DoGizmosDraw();

            if (projectPlaneMap == null || projectPlaneMap.Count == 0)
                return;

            foreach (var kv in projectPlaneMap)
            {
                var checker = kv.Value;

                if (checker.target != null && checker.projectResults.size.magnitude != 0)
                    GizmosDrawGrids(checker.projectResults);
            }
        }
#endif        

        private void SetGridUsedState((Vector2Int startGrid, Vector2Int size) rect , bool isUsed)
        {
            for (int i = rect.startGrid.x, maxX = Mathf.Min(i + rect.size.x, size.x); i < maxX; i++)
            {
                for (int j = rect.startGrid.y, maxY = Mathf.Min(j + rect.size.y, size.y); j < maxY; j++)
                {
                    gridStates[i, j] = isUsed;
                }
            }
        }
          
        public bool GetUsedGridById(string id, out (Vector2Int startGrid, Vector2Int size) rect)
        {
            rect = (Vector2Int.zero, Vector2Int.zero);
             
            if (gridUsedMap == null || !gridUsedMap.ContainsKey(id))
                return false;

            rect = gridUsedMap[id];

            return true;
        }

        /// <summary>
        /// 设置网格为使用状态
        /// </summary>
        /// <param name="gridPlane"></param>
        /// <param name="rect"></param>
        public void SetGridStateUsed(string id, (Vector2Int startGrid, Vector2Int size) rect)
        {
            gridUsedMap = gridUsedMap ?? new Dictionary<string, (Vector2Int startGrid, Vector2Int size)>();

            Debug.Assert(!gridUsedMap.ContainsKey(id));

            gridUsedMap.Add(id, rect);

            SetGridUsedState(rect, true);
        }

        /// <summary>
        /// 设置网格为未使用状态
        /// </summary>
        /// <param name="rect"></param>
        public void SetGridStateUnUsed(string id)
        {
            gridUsedMap = gridUsedMap ?? new Dictionary<string, (Vector2Int startGrid, Vector2Int size)>();

            if (gridUsedMap.TryGetValue(id, out var rect))
            {
                SetGridUsedState(rect, false);

                gridUsedMap.Remove(id);
            }
        }

        public void AddProjectPlane(DAGridPlane target)
        {
            projectPlaneMap = projectPlaneMap ?? new Dictionary<DAGridPlane, ProjectChecker>();

            if (projectPlaneMap.ContainsKey(target))
                return;

            var checker = new ProjectChecker() { target = target };

            projectPlaneMap.Add(target, checker);
        }

        public void RemoveProjectPlane(DAGridPlane target)
        {
            if (projectPlaneMap == null || !projectPlaneMap.ContainsKey(target))
                return;

            var checker = projectPlaneMap[target];

            projectPlaneMap.Remove(target);
        }
        
        public ProjectChecker GetProjectChecker(DAGridPlane target)
        {
            if(projectPlaneMap != null && projectPlaneMap.ContainsKey(target))
            {
                return projectPlaneMap[target];
            }

            return null;
        }

        /// <summary>
        /// 直接将物体放置到投影到的位置
        /// </summary>
        /// <param name="go"></param>
        /// <param name="target"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public void PlaceToProjectPose(GameObject go, DAGridPlane target, (Vector2Int startGrid, Vector2Int size) rect)
        {
            var srcPose = new Pose(go.transform.position, go.transform.rotation);
 
            var dropPostion = PlanePosToWPos((GridToPlanePos(rect.startGrid) - GridCenter) + ((Vector2)(rect.size) * gridSize) * 0.5f);

            go.transform.rotation = transform.rotation * Quaternion.Inverse(target.transform.rotation) * go.transform.rotation;

            //PlaneWDir等用的都是世界坐标，先设置旋转再调整位置
            go.transform.position = dropPostion - target.PlaneWDir * target.offset + (go.transform.position - target.transform.position);

            var targetPose = new Pose(go.transform.position, go.transform.rotation);

            go.transform.position = srcPose.position;

            go.transform.rotation = srcPose.rotation;

            go.transform.DOMove(targetPose.position, 0.08f, false);

            go.transform.DORotateQuaternion(targetPose.rotation, 0.04f);
        }
    }
}
