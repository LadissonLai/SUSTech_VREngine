using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR;

namespace HighlightPlus {
    public static class VRCheck {

#if UNITY_2019_3_OR_NEWER

    static List<XRDisplaySubsystemDescriptor> displaysDescs = new List<XRDisplaySubsystemDescriptor>();
    static List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();

    public static bool IsActive() {
        displaysDescs.Clear();
        SubsystemManager.GetSubsystemDescriptors(displaysDescs);

        // If there are registered display descriptors that is a good indication that VR is most likely "enabled"
        return displaysDescs.Count > 0;
    }

    public static bool IsVrRunning() {
        bool vrIsRunning = false;
        displays.Clear();
        SubsystemManager.GetInstances(displays);
        foreach (var displaySubsystem in displays) {
            if (displaySubsystem.running) {
                vrIsRunning = true;
                break;
            }
        }

        return vrIsRunning;
    }
#else
        public static bool IsActive() {
            return PlayerSettings.virtualRealitySupported;
        }

        public static bool IsVrRunning() {
            return Application.isPlaying && PlayerSettings.virtualRealitySupported;
        }
#endif

    }

}