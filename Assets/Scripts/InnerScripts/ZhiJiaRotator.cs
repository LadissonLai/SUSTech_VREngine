using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhiJiaRotator : MonoBehaviour
{
    public Transform rotationRef;

    public bool isEngine;
    
    private Vector3 origin;

    private bool isNeedUpdate = false;

    private float lastAngles = 0f;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.localEulerAngles;
    }

    // Update is called once per frame
    void LateUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.M))
        {
            rotationRef.localEulerAngles = new Vector3(rotationRef.localEulerAngles.x, rotationRef.localEulerAngles.y, rotationRef.localEulerAngles.z + 3f);
        }
       else if (Input.GetKey(KeyCode.N))
        {
            rotationRef.localEulerAngles = new Vector3(rotationRef.localEulerAngles.x, rotationRef.localEulerAngles.y, rotationRef.localEulerAngles.z -3f);
        }
#endif

        ChangeAngles();
    }

    private void ChangeAngles()
    {
        if (lastAngles != rotationRef.localEulerAngles.z)
        {
            lastAngles = rotationRef.localEulerAngles.z;
            transform.localEulerAngles = new Vector3(isEngine ? -lastAngles : lastAngles, origin.y, origin.z);
        }
    }
}
