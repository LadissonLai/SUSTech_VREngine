using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Framework;
using System;

namespace VRTKExtensions
{

    public class InteractObjCachePool
    {
        protected Dictionary<string, VRTK_InteractableObject> keyMap = new Dictionary<string, VRTK_InteractableObject>();

        public List<string> container = new List<string>();

        public List<int> torsionContainer = new List<int>();

        public int ShortcutPointer
        {
            get
            {
                if (CurrentShortcutID == null)
                    return -1;

                return container.IndexOf(CurrentShortcutID);
            }
        }

        public string CurrentShortcutID { get; private set; }

        public int MaxAmout { get; private set; }

        public event Action OnShortcutAmountChanged;

        public event Action OnShortcutPointerChanged;

        public InteractObjCachePool(int maxAmout = 999)
        {
            MaxAmout = maxAmout;
        }

        private void Remove(string key)
        {
            if (!keyMap.TryGetValue(key, out var target))
                return;

            keyMap.Remove(key);

            torsionContainer.RemoveAt(container.IndexOf(key));
            container.Remove(key);

            if (target == null)
                return;

            target.InteractableObjectUngrabbed -= OnObjUnGrabed;

            GameObject.Destroy(target.gameObject);
        }

        public void Add(VRTK_InteractableObject target, string key, int torsion, bool autoGet = true)
        {
            target.InteractableObjectUngrabbed -= OnObjUnGrabed;
            target.InteractableObjectUngrabbed += OnObjUnGrabed;

            Remove(key);

            keyMap.Add(key, target);

            torsionContainer.Add(torsion);
            container.Add(key);

            if (container.Count > MaxAmout)
            {
                //超限
                Remove(container[0]);
            }

            OnShortcutAmountChanged?.Invoke();

            if (autoGet)
            {
                Get(container.Count - 1);
            }
            else
            {
                target.gameObject.SetActive(false);
            }
        }

        private void OnObjUnGrabed(object sender, InteractableObjectEventArgs e)
        {
            var targetObj = sender as VRTK_InteractableObject;

            if (targetObj == null)
                return;

            targetObj.gameObject.SetActive(false);

            if (CurrentShortcutID == null)
                return;

            if (!keyMap.TryGetValue(CurrentShortcutID, out var target))
                return;

            if (target == targetObj)
            {
                //当前选择的工具被放下

                CurrentShortcutID = null;

                OnShortcutPointerChanged?.Invoke();
            }
        }

        public void Get(string key)
        {
            if (!keyMap.TryGetValue(key, out var target))
            {
                Debug.LogError("无此快捷工具:" + key);
            }

            if (target.IsGrabbed())
                return;

            CurrentShortcutID = key;

            target.gameObject.SetActive(true);

            VRTKHelper.ForceGrab(target);

            OnShortcutPointerChanged?.Invoke();
        }

        public void Get(int pointer)
        {
            if (container.Count == 0)
                return;

            if (pointer < 0)
                pointer = container.Count - 1;
            else if (pointer >= container.Count)
                pointer = 0;

            var key = container[pointer];

            Get(key);
        }

        public void GetPre()
        {
            Get(ShortcutPointer - 1);
        }

        public void Dispose()
        {
            foreach (var kv in keyMap)
            {
                if (kv.Value == null)
                    continue;

                kv.Value.InteractableObjectUngrabbed -= OnObjUnGrabed;

                GameObject.Destroy(kv.Value.gameObject);
            }

            torsionContainer.Clear();
            container.Clear();

            keyMap.Clear();

            OnShortcutAmountChanged = null;
            OnShortcutPointerChanged = null;
        }
    }
}

