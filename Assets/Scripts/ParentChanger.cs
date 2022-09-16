using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMS.Public
{
    [DisallowMultipleComponent]
    public class ParentChanger : MonoBehaviour
    {
        public Transform attachTo;

        // Start is called before the first frame update
        private void Awake()
        {
            DebugEx.AssertIsTrue(!attachTo.IsChildOf(transform));

            transform.SetParent(attachTo);

            Destroy(this);
        }

    }
}
