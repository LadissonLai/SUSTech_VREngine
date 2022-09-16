using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HighlightPlus {


    public partial class HighlightEffect : MonoBehaviour {

        static List<HighlightSeeThroughOccluder> occluders = new List<HighlightSeeThroughOccluder>();
        static Dictionary<Camera, int> occludersFrameCount = new Dictionary<Camera, int>();
        static CommandBuffer cbOccluder;
        static Material fxMatOccluder;

        bool cancelSeeThroughThisFrame;

        public static void RegisterOccluder(HighlightSeeThroughOccluder occluder) {
            if (!occluders.Contains(occluder)) {
                occluders.Add(occluder);
            }
        }

        public static void UnregisterOccluder(HighlightSeeThroughOccluder occluder) {
            if (occluders.Contains(occluder)) {
                occluders.Remove(occluder);
            }
        }

        public void RenderOccluders(Camera cam) {

            int occludersCount = occluders.Count;
            if (occludersCount == 0) return;

            int lastFrameCount;
            occludersFrameCount.TryGetValue(cam, out lastFrameCount);
            int currentFrameCount = Time.frameCount;
            if (currentFrameCount == lastFrameCount) return;
            occludersFrameCount[cam] = currentFrameCount;

            if (cbOccluder == null) {
                cbOccluder = new CommandBuffer();
                cbOccluder.name = "Occluder";
            }

            if (fxMatOccluder == null) {
                InitMaterial(ref fxMatOccluder, "HighlightPlus/Geometry/SeeThroughOccluder");
                if (fxMatOccluder == null) return;
            }

            Vector3 camPos = cam.transform.position;

            cbOccluder.Clear();
            for (int k = 0; k < occludersCount; k++) {
                HighlightSeeThroughOccluder occluder = occluders[k];
                if (occluder == null || !occluder.isActiveAndEnabled) continue;
                if (occluder.detectionMethod == DetectionMethod.Stencil) {
                    if (occluder.meshData == null || occluder.meshData.Length == 0) continue;
                    // Per renderer
                    for (int m = 0; m < occluder.meshData.Length; m++) {
                        // Per submesh
                        Renderer renderer = occluder.meshData[m].renderer;
                        if (renderer.isVisible) {
                            for (int s = 0; s < occluder.meshData[m].subMeshCount; s++) {
                                cbOccluder.DrawRenderer(renderer, fxMatOccluder, s);
                            }
                        }
                    }
                } else {
                    if (rms.Length == 0 || rms[0].renderer == null) continue;
                    // Compute bounds
                    Bounds bounds = rms[0].renderer.bounds;
                    for (int r = 1; r < rms.Length; r++) {
                        if (rms[r].renderer != null) {
                            bounds.Encapsulate(rms[r].renderer.bounds);
                        }
                    }
                    Vector3 pos = bounds.center;
                    Vector3 offset = pos - camPos;
                    float maxDistance = Vector3.Distance(pos, camPos);
                    RaycastHit hit;
                    if (Physics.BoxCast(pos - offset, bounds.extents * 0.9f, offset.normalized, out hit, Quaternion.identity, maxDistance)) {
                        if (hit.collider.transform == occluder.transform) {
                            cancelSeeThroughThisFrame = true;
                        }
                    }
                }
            }
            Graphics.ExecuteCommandBuffer(cbOccluder);
        }

    }
}
