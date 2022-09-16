using Framework.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class DACarCtr : MonoBehaviour
    {
        public GraphicsCache testGC;

        public Material testMat1;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return null;

            //testGC.SwapGraphicsSharedMats(testMat1);
        }
    }
}