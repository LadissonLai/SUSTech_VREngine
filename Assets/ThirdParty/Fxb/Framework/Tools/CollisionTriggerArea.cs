using System;
using System.Collections.Generic;
using UnityEngine;


namespace Framework.Tools
{
    /// <summary>
    /// trigger 碰撞触发区域
    /// </summary>
    public class CollisionTriggerArea : MonoBehaviour
    {
        private HashSet<GameObject> triggerStaysChecker;

        private List<GameObject> gameobjsInArea;

        public bool ignoreTriggerCollider;

        public event Action<GameObject> OnGOTrigger;

        public event Action<GameObject> OnGOExit;

        public IReadOnlyList<GameObject> GameobjsInArea => gameobjsInArea;

        private void Awake()
        {
            triggerStaysChecker = new HashSet<GameObject>();

            gameobjsInArea = new List<GameObject>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ignoreTriggerCollider && other.isTrigger)
                return;

            var rigidBody = other.attachedRigidbody;

            if (rigidBody == null)
                return;

            if (!gameobjsInArea.Contains(rigidBody.gameObject))
            {
                gameobjsInArea.Add(rigidBody.gameObject);

                OnGOTrigger?.Invoke(rigidBody.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (ignoreTriggerCollider && other.isTrigger)
                return;

            var rigidBody = other.attachedRigidbody;

            //非动态物体
            if (rigidBody == null)
                return;

            if (!triggerStaysChecker.Contains(rigidBody.gameObject))
                triggerStaysChecker.Add(rigidBody.gameObject);
        }

        private void FixedUpdate()
        {
            for (int i = gameobjsInArea.Count - 1; i >= 0; i--)
            {
                var go = gameobjsInArea[i];

                if (go == null || !triggerStaysChecker.Contains(go))
                {
                    gameobjsInArea.RemoveAt(i);

                    OnGOExit?.Invoke(go);
                }
            }
 
            triggerStaysChecker.Clear();
        }
    }
}