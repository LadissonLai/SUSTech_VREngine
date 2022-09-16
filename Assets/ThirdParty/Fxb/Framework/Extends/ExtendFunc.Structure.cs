using System;
using System.Collections.Generic;

namespace Framework
{
    public static partial class ExtendFunc
    {
        /// <summary>
        /// 添加不存在的key值，替换已存在的内容
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Tval"></typeparam>
        /// <returns>是否新增</returns>
        public static bool AddUnique<Tkey, Tval>(this IDictionary<Tkey, Tval> dict, Tkey key, Tval val)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, val);

                return true;
            }
            else
            {
                dict[key] = val;

                return false;
            }
        }

        /// <summary>
        /// 通过value返回key值列表
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Tval"></typeparam>
        /// <param name="dict"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<Tkey> GetKeysSafely<Tkey, Tval>(this IDictionary<Tkey, Tval> dict, Tval value)
        {
            List<Tkey> res = null;

            foreach (var kv in dict)
            {
                if (kv.Value.Equals(value))
                {
                    res = res ?? new List<Tkey>();

                    res.Add(kv.Key);
                }
            }

            return res;
        }

        public static Tval GetSafely<Tkey, Tval>(this IDictionary<Tkey, Tval> dict, Tkey key)
        {
            dict.TryGetValue(key, out var val);
             
            return val;
        }

        public static bool AddUnique<T>(this ICollection<T> list, T val)
        {
            if(!list.Contains(val))
            {
                list.Add(val);

                return true;
            }

            return false;
        }

        public static void AddRangeUnique<T>(this ICollection<T> list, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                list.AddUnique(item);
            }
        }
    }
}