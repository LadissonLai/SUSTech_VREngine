using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Fxb.CsvConfig
{
    public class RowDataGenerator<T> : IConfigRowDataGenerat<T> where T : new()
    {
        protected PropertyInfo[] properties;

        protected Dictionary<PropertyInfo, Action<string, T>> propSetActions;

        public RowDataGenerator()
        {
            properties = typeof(T).GetProperties();
 
            propSetActions = new Dictionary<PropertyInfo, Action<string, T>>();
        }

        private Action<string, T> CreatePropSetAction(PropertyInfo pInfo)
        {
            Debug.Assert(!pInfo.PropertyType.IsArray, "暂不支持数组");

            #region 不支持数组
            //if(pInfo.PropertyType.IsArray)
            //{
            //    if (pInfo.PropertyType == typeof(int[]))
            //    {
            //        var action = CreatePropSetAction<int[]>(pInfo);

            //        return (string strVal, T inst) =>
            //        {
            //            if (!string.IsNullOrWhiteSpace(strVal) && ConvertToIntArray(strVal, out var intArrVal))
            //                action(inst, intArrVal);
            //        };
            //    }
            //}
            #endregion

            //int自动转换为Enum
            if (pInfo.PropertyType.IsEnum)
            {
                var action = CreatePropSetAction<int>(pInfo);

                return (string strVal, T inst) =>
                {
                    if (!string.IsNullOrWhiteSpace(strVal) && ConvertToEnum(strVal, pInfo.PropertyType, out var enumVal))
                        action(inst, enumVal);
                };
            }

            if(pInfo.PropertyType == typeof(bool))
            {
                var action = CreatePropSetAction<bool>(pInfo);

                return (string strVal, T inst) =>
                {
                    if (!string.IsNullOrWhiteSpace(strVal) && ConvertToBoolean(strVal, out var booleanVal))
                        action(inst, booleanVal);
                };
            }

            if (pInfo.PropertyType == typeof(int))
            {
                var action = CreatePropSetAction<int>(pInfo);

                return (string strVal, T inst) => {
                    if (!string.IsNullOrWhiteSpace(strVal) && ConvertToInt(strVal, out var intVal))
                        action(inst, intVal);
                };
            }

            if (pInfo.PropertyType == typeof(string))
            {
                var action = CreatePropSetAction<string>(pInfo);

                return (string strVal, T inst) => { action.Invoke(inst, strVal); };
            }

            if (pInfo.PropertyType == typeof(float))
            {
                var action = CreatePropSetAction<float>(pInfo);

                return (string strVal, T inst) => {
                    if (!string.IsNullOrWhiteSpace(strVal) && ConvertToFloat(strVal, out var floatVal))
                        action(inst, floatVal);
                };
            }

            return null;
        }

        public Action<T, U> CreatePropSetAction<U>(PropertyInfo pInfo)
        {
            return Delegate.CreateDelegate(typeof(Action<T, U>), pInfo.GetSetMethod(true)) as Action<T, U>;
        }

        public T FromListDatas(IReadOnlyList<string> listData, Dictionary<string, int> titleMap)
        {
            var rowData = new T();

            foreach (var propInfo in properties)
            {
                if (!titleMap.TryGetValue(propInfo.Name, out var titleIndex))
                    continue;

                SetPropVal(rowData, propInfo, listData[titleIndex]);
            }

            return rowData;
        }

        public void SetPropVal(T inst, PropertyInfo pInfo, string strVal)
        {
            if (!propSetActions.TryGetValue(pInfo, out var PropSetAction))
            {
                PropSetAction = CreatePropSetAction(pInfo);

                propSetActions.Add(pInfo, PropSetAction);
            }

            PropSetAction?.Invoke(strVal, inst);
        }

        protected bool ConvertToInt(string strVal, out int res)
        {
            res = -1;

            return int.TryParse(strVal, out res);
        }

        protected bool ConvertToFloat(string strVal, out float res)
        {
            res = 0.0f;

            return float.TryParse(strVal, out res);
        }

        protected bool ConvertToBoolean(string strVal, out bool booleanVal)
        {
            booleanVal = false;

            if(ConvertToInt(strVal, out var intVal))
            {
                booleanVal = intVal > 0;

                return true;
            }

            return false;
        }

        protected bool ConvertToEnum(string strVal, Type enumType, out int enumVal)
        {
            enumVal = -1;

            if (ConvertToInt(strVal, out var intVal) && Enum.IsDefined(enumType, Enum.ToObject(enumType, intVal)))
            {
                enumVal = intVal;

                return true;
            }

            return false;
        }

        #region 未使用
        //protected bool ConvertToIntArray(string strVal, out int[] res)
        //{
        //    res = null;

        //    var strArray = strVal.Split(',');

        //    if (strArray.Length == 0)
        //        return false;

        //    res = new int[strArray.Length];

        //    for (int i = 0, len = res.Length; i < len; i++)
        //    {
        //        ConvertToInt(strArray[i], out int intRes);

        //        res[i] = intRes;
        //    }

        //    return true;
        //}
        #endregion
    }
}
