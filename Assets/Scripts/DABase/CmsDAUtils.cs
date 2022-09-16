using System;
using System.Collections.Generic;
using Framework;
using Fxb.DA;

namespace Fxb.DA
{
    public static class CmsDAUtils
    {
        public static bool ObjsDismantledPredicate(AbstractDAObjCtr obj) => obj.State == CmsObjState.Dismantled;

        public static bool ObjsUnDismantledPredicate(AbstractDAObjCtr obj) => obj.State != CmsObjState.Dismantled;
 
        public static bool ObjsUnDefaultPredicate(AbstractDAObjCtr obj) => obj.State != CmsObjState.Default;

        public static bool ObjsDefaultPredicate(AbstractDAObjCtr obj) => obj.State == CmsObjState.Default;

        /// <summary>
        /// 检查是否存在匹配状态的依赖物体（不会递归查找）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="filterType"></param>
        /// <param name="matchState"></param>
        /// <param name="outObjCtr"></param>
        /// <returns></returns>
        public static bool CheckDepandObjExist(AbstractDAObjCtr target, CmsObjType filterType, CmsObjState matchState, out AbstractDAObjCtr outObjCtr)
        {
            if ((filterType & CmsObjType.Parts) != 0 && DoCheckDepandObjExist(target, CmsObjType.Parts, matchState,out outObjCtr))
            {
                return true;
            }

            if ((filterType & CmsObjType.SnapFit) != 0 && DoCheckDepandObjExist(target, CmsObjType.SnapFit, matchState,out outObjCtr))
            {
                return true;
            }

            outObjCtr = null;

            return false;
        }

        private static bool DoCheckDepandObjExist(AbstractDAObjCtr target, CmsObjType filterType, CmsObjState matchState, out AbstractDAObjCtr outObjCtr)
        {
            var dependObjs = filterType == CmsObjType.Parts ? target.DependParts : target.DependSnapFits;

            if (dependObjs?.Count > 0)
            {
                foreach (var obj in dependObjs)
                {
                    if ((obj.State & matchState) != 0)
                    {
                        outObjCtr = obj;

                        return true;
                    }
 
                    if (DoCheckDepandObjExist(obj, filterType, matchState, out outObjCtr))
                    {
                        return true;
                    }
                }
            }

            outObjCtr = null;

            return false;
        }

        /// <summary>
        /// 找到所有依赖的物体
        /// </summary>
        /// <param name="target"></param>
        /// <param name="filterType"></param>
        /// <param name="givenRes"></param>
        /// <param name="conditionMatch">如果不匹配 </param>
        public static void FindAllDepandMatchObjs(AbstractDAObjCtr target, CmsObjType filterType, CmsObjState matchState, List<AbstractDAObjCtr> givenRes, Predicate<AbstractDAObjCtr> conditionMatch = null)
        {
            if ((filterType & CmsObjType.Parts) != 0)
                DoFindAllDepandMatchObjs(target, CmsObjType.Parts, matchState, givenRes, conditionMatch);

            if ((filterType & CmsObjType.SnapFit) != 0)
                DoFindAllDepandMatchObjs(target, CmsObjType.SnapFit, matchState, givenRes, conditionMatch);
        }

        private static void DoFindAllDepandMatchObjs(AbstractDAObjCtr target, CmsObjType filterType, CmsObjState matchState, List<AbstractDAObjCtr> givenRes, Predicate<AbstractDAObjCtr> conditionMatch = null)
        {
            var dependObjs = filterType == CmsObjType.Parts ? target.DependParts : target.DependSnapFits;

            if (dependObjs?.Count > 0)
            {
                foreach (var obj in dependObjs)
                {
                    DebugEx.AssertNotNull(obj, $"依赖物体为空 {target.name}");

                    if (obj == null || (obj.State & matchState) == 0 || (conditionMatch != null && !conditionMatch(obj)))
                        continue;

                    givenRes.AddUnique(obj);

                    DoFindAllDepandMatchObjs(obj, filterType, matchState, givenRes, conditionMatch);
                }
            }
        }

        /// <summary>
        /// 找到依赖的部件（递归查找）
        /// </summary>
        /// <param name="filterType"></param>
        /// <param name="matchState"></param>
        /// <param name="givenRes">传入的结果列表数据，默认不会清空！</param>
        public static void FindDepandMatchObjs(AbstractDAObjCtr target, CmsObjType filterType, CmsObjState matchState, List<AbstractDAObjCtr> givenRes, bool findDependLeaf = false, Predicate<AbstractDAObjCtr> conditionMatch = null)
        {
            if ((filterType & CmsObjType.Parts) != 0)
                DoFindDepandMatchObjs(target, CmsObjType.Parts, matchState, givenRes, findDependLeaf, conditionMatch);

            if ((filterType & CmsObjType.SnapFit) != 0)
                DoFindDepandMatchObjs(target, CmsObjType.SnapFit, matchState, givenRes, findDependLeaf, conditionMatch);
        }

        private static bool DoFindDepandMatchObjs(AbstractDAObjCtr target, CmsObjType filterType, CmsObjState matchState, List<AbstractDAObjCtr> givenRes, bool findDependLeaf, Predicate<AbstractDAObjCtr> conditionMatch = null)
        {
            var hasFindValidObj = false;

            var dependObjs = filterType == CmsObjType.Parts ? target.DependParts : target.DependSnapFits;
 
            if (dependObjs?.Count > 0)
            {
                foreach (var obj in dependObjs)
                {
                    DebugEx.AssertNotNull(obj, $"依赖物体为空 {target.name}");

                    if (findDependLeaf)
                    {
                        if (obj == null || (obj.State & matchState) == 0 || ( conditionMatch != null && !conditionMatch(obj)))
                            continue;

                        hasFindValidObj = true;
 
                        if(!DoFindDepandMatchObjs(obj, filterType, matchState, givenRes, findDependLeaf, conditionMatch))
                        {
                            givenRes.AddUnique(obj);
                        }
                    }
                    else
                    {
                        if(obj == null)
                            continue;

                        if ((obj.State & matchState) != 0)
                        {
                            if((conditionMatch == null || conditionMatch(obj)))
                            {
                                hasFindValidObj = true;

                                givenRes.AddUnique(obj);
                            }

                            continue;
                        }

                        hasFindValidObj = DoFindDepandMatchObjs(obj, filterType, matchState, givenRes, findDependLeaf, conditionMatch);
                    }
                }
            }

            return hasFindValidObj;
        }

        /// <summary>
        /// 安装时根据依赖去重。 依赖的数据结构改成双向查找后可以不需要此方法
        /// 如: a依赖c并且b依赖c，当a被安装上但是b未安装时，不能将c激活。
        /// </summary>
        /// <param name="parts"></param>
        public static void RemovePartsByDepend(IList<AbstractDAObjCtr> parts)
        {
            if (parts.Count > 1)
            {
                //去重
                for (int i = parts.Count - 1; i >= 0; i--)
                {
                    var checkObj = parts[i];

                    for (int j = 0; j < parts.Count; j++)
                    {
                        var tmpObj = parts[j];

                        if (tmpObj == checkObj)
                            continue;

                        if (tmpObj.DependParts != null && tmpObj.DependParts.Contains(checkObj))
                        {
                            parts.Remove(checkObj);

                            break;
                        }
                    }
                }
            }
        }
    }
}