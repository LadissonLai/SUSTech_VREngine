using UnityEngine;
using VRTK;

namespace FxbVRTK
{
    /// <summary>
    /// 让ui canvas 停留在眼前显示。
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class FaceCanvas : MonoBehaviour
    {
        Canvas canvas;

        public float planeDistance;

        protected virtual void OnEnable()
        {
            VRTK_SDKManager.SubscribeLoadedSetupChanged(LoadedSetupChanged);
            InitCanvas();
        }

        protected virtual void OnDisable()
        {
            if (!gameObject.activeSelf)
            {
                VRTK_SDKManager.UnsubscribeLoadedSetupChanged(LoadedSetupChanged);
            }
        }

        protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            if (this != null && VRTK_SDKManager.ValidInstance() && gameObject.activeInHierarchy)
            {
                SetCanvasCamera();
            }
        }

        protected virtual void InitCanvas()
        {
            canvas = transform.GetComponentInParent<Canvas>();

            if (canvas != null)
            {
                canvas.planeDistance = planeDistance < 0.1f ? 0.5f : planeDistance;
            }

            SetCanvasCamera();
        }

        protected virtual void SetCanvasCamera()
        {
            Transform sdkCamera = VRTK_DeviceFinder.HeadsetCamera();
            if (sdkCamera != null)
            {
                canvas.worldCamera = sdkCamera.GetComponent<Camera>();
            }
        }
    }
}

