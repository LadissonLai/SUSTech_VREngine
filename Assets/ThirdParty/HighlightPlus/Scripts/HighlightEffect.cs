/// <summary>
/// Highlight Plus - (c) 2018-2020 Ramiro Oliva (Kronnect)
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HighlightPlus {

    public delegate bool OnObjectHighlightStartEvent(GameObject obj);
    public delegate void OnObjectHighlightEndEvent(GameObject obj);
    public delegate bool OnRendererHighlightEvent(Renderer renderer);


    public enum SeeThroughMode {
        WhenHighlighted = 0,
        AlwaysWhenOccluded = 1,
        Never = 2
    }

    public enum QualityLevel {
        Fastest = 0,
        High = 1,
        Highest = 2
    }

    public enum TargetOptions {
        Children,
        OnlyThisObject,
        RootToChildren,
        LayerInScene,
        LayerInChildren
    }

    public enum Visibility {
        Normal,
        AlwaysOnTop,
        OnlyWhenOccluded
    }

    public enum GlowBlendMode {
        Additive,
        AlphaBlend
    }

    [Serializable]
    public struct GlowPassData {
        public float offset;
        public float alpha;
        public Color color;
    }

    [ExecuteInEditMode]
    [HelpURL("https://kronnect.freshdesk.com/support/solutions/42000065090")]
    public partial class HighlightEffect : MonoBehaviour {

        /// <summary>
        /// Gets or sets the current profile. To load a profile and apply its settings at runtime, please use ProfileLoad() method.
        /// </summary>
        public HighlightProfile profile;

        /// <summary>
        /// Sets if changes to the original profile should propagate to this effect.
        /// </summary>
        [Tooltip("If enabled, settings will be synced with profile.")]
        public bool profileSync;

        /// <summary>
        /// Makes the effects visible in the SceneView.
        /// </summary>
        public bool previewInEditor = true;

        /// <summary>
        /// Specifies which objects are affected by this effect.
        /// </summary>
        public TargetOptions effectGroup = TargetOptions.Children;

        /// <summary>
        /// The layer that contains the affected objects by this effect when effectGroup is set to LayerMask.
        /// </summary>
        public LayerMask effectGroupLayer = -1;

        /// <summary>
        /// The alpha threshold for transparent cutout objects. Pixels with alpha below this value will be discarded.
        /// </summary>
        [Range(0, 1)]
        public float alphaCutOff = 0;

        /// <summary>
        /// If back facing triangles are ignored. Backfaces triangles are not visible but you may set this property to false to force highlight effects to act on those triangles as well.
        /// </summary>
        public bool cullBackFaces = true;

        /// <summary>
        /// Show highlight effects even if the object is currently not visible. This option is useful if the affected objects are rendered using GPU instancing tools which render directly to the GPU without creating real game object geometry in CPU.
        /// </summary>
        [Tooltip("Show highlight effects even if the object is not visible. If this object or its children use GPU Instancing tools, the MeshRenderer can be disabled although the object is visible. In this case, this option is useful to enable highlighting.")]
        public bool ignoreObjectVisibility;

        /// <summary>
        /// Enable to support reflection probes
        /// </summary>
        [Tooltip("Support reflection probes. Enable only if you want the effects to be visible in reflections.")]
        public bool reflectionProbes;

        /// <summary>
        /// Enabled depth buffer flip in HQ
        /// </summary>
        [Tooltip("Enables depth buffer clipping. Only applies to outline or outer glow in High Quality mode.")]
        public bool depthClip;


        /// <summary>
        /// Ignores highlight effects on this object.
        /// </summary>
        [Tooltip("Ignore highlighting on this object.")]
        public bool ignore;

        [SerializeField]
        bool _highlighted;

        public bool highlighted { get { return _highlighted; } set { SetHighlighted(value); } }

        public float fadeInDuration;
        public float fadeOutDuration;

#if UNITY_2019_OR_NEWER
        public bool flipY = true;

#else
        public bool flipY;
#endif

        public bool constantWidth = true;

        //public Transform customTarget;

        [Range(0, 1)]
        public float overlay = 0.5f;
        public Color overlayColor = Color.yellow;
        public float overlayAnimationSpeed = 1f;
        [Range(0, 1)]
        public float overlayMinIntensity = 0.5f;
        [Range(0, 1)]
        public float overlayBlending = 1.0f;

        [Range(0, 1)]
        public float outline = 1f;
        public Color outlineColor = Color.black;
        public float outlineWidth = 0.45f;
        public QualityLevel outlineQuality = QualityLevel.High;
        [Range(1, 8)]
        public int outlineDownsampling = 2;
        public Visibility outlineVisibility = Visibility.Normal;
        public bool outlineOptimalBlit = true;
        public bool outlineBlitDebug;
        public bool outlineIndependent;

        [Range(0, 5)]
        public float glow = 1f;
        public float glowWidth = 0.4f;
        public QualityLevel glowQuality = QualityLevel.High;
        [Range(1, 8)]
        public int glowDownsampling = 2;
        public Color glowHQColor = new Color(0.64f, 1f, 0f, 1f);
        public bool glowDithering = true;
        public GlowBlendMode glowBlendMode = GlowBlendMode.Additive;
        public float glowMagicNumber1 = 0.75f;
        public float glowMagicNumber2 = 0.5f;
        public float glowAnimationSpeed = 1f;
        public Visibility glowVisibility = Visibility.Normal;
        public bool glowOptimalBlit = true;
        public bool glowBlitDebug;
        public GlowPassData[] glowPasses;

        [Range(0, 5f)]
        public float innerGlow;
        [Range(0, 2)]
        public float innerGlowWidth = 1f;
        public Color innerGlowColor = Color.white;
        public Visibility innerGlowVisibility = Visibility.Normal;

        public bool targetFX;
        public Texture2D targetFXTexture;
        public Color targetFXColor = Color.white;
        public Transform targetFXCenter;
        public float targetFXRotationSpeed = 50f;
        public float targetFXInitialScale = 4f;
        public float targetFXEndScale = 1.5f;
        public float targetFXTransitionDuration = 0.5f;
        public float targetFXStayDuration = 1.5f;

        public event OnObjectHighlightStartEvent OnObjectHighlightStart;
        public event OnObjectHighlightEndEvent OnObjectHighlightEnd;
        public event OnRendererHighlightEvent OnRendererHighlightStart;

        public SeeThroughMode seeThrough;
        public LayerMask seeThroughOccluderMask = -1;
        [Range(0.01f, 0.6f)] public float seeThroughOccluderThreshold = 0.3f;
        public float seeThroughOccluderCheckInterval = 1f;
        [Range(0, 5f)] public float seeThroughIntensity = 0.8f;
        [Range(0, 1)] public float seeThroughTintAlpha = 0.5f;
        public Color seeThroughTintColor = Color.red;
        [Range(0, 1)] public float seeThroughNoise = 1f;
        [Range(0, 1)] public float seeThroughBorder;
        public Color seeThroughBorderColor = Color.black;
        public float seeThroughBorderWidth = 0.45f;

        struct ModelMaterials {
            public bool render; // if this object can render this frame
            public Transform transform;
            public bool bakedTransform;
            public Vector3 currentPosition, currentRotation, currentScale;
            public bool renderWasVisibleDuringSetup;
            public Mesh mesh, originalMesh;
            public Renderer renderer;
            public bool skinnedMesh;
            public Material[] fxMatMask, fxMatSolidColor, fxMatSeeThroughInner, fxMatSeeThroughBorder, fxMatOverlay, fxMatInnerGlow;
            public Matrix4x4 renderingMatrix;
        }

        enum FadingState {
            FadingOut = -1,
            NoFading = 0,
            FadingIn = 1
        }

        [SerializeField, HideInInspector]
        ModelMaterials[] rms;
        [SerializeField, HideInInspector]
        int rmsCount;

#if UNITY_EDITOR
        /// <summary>
        /// True if there's some static children
        /// </summary>
        [NonSerialized]
        public bool staticChildren;
#endif

        [NonSerialized]
        public Transform target;
        
        public Renderer[] customRenderers;

        // Time in which the highlight started
        [NonSerialized]
        public float highlightStartTime;

        const string SKW_ALPHACLIP = "HP_ALPHACLIP";
        const string SKW_DEPTHCLIP = "HP_DEPTHCLIP";
        const string UNIFORM_CUTOFF = "_CutOff";
        const float TAU = 0.70711f;

        // Reference materials. These are instanced per object (rms).
        static Material fxMatMask, fxMatSolidColor, fxMatSeeThroughInner, fxMatSeeThroughBorder, fxMatOverlay, fxMatClearStencil;
        static Material fxMatGlowRef, fxMatInnerGlow, fxMatOutlineRef, fxMatTargetRef;
        static Material fxMatComposeGlowRef, fxMatComposeOutlineRef, fxMatBlurGlowRef, fxMatBlurOutlineRef;

        // Per-object materials
        Material _fxMatOutline, _fxMatGlow, _fxMatTarget;
        Material _fxMatComposeGlow, _fxMatComposeOutline, _fxMatBlurGlow, _fxMatBlurOutline;

        Material fxMatOutline {
            get {
                if (_fxMatOutline == null && fxMatOutlineRef != null) _fxMatOutline = Instantiate(fxMatOutlineRef);
                return _fxMatOutline;
            }
        }

        Material fxMatGlow {
            get {
                if (_fxMatGlow == null && fxMatGlowRef != null) _fxMatGlow = Instantiate(fxMatGlowRef);
                return _fxMatGlow;
            }
        }

        Material fxMatTarget {
            get {
                if (_fxMatTarget == null && fxMatTargetRef != null) _fxMatTarget = Instantiate(fxMatTargetRef);
                return _fxMatTarget;
            }
        }

        Material fxMatComposeGlow {
            get {
                if (_fxMatComposeGlow == null && fxMatComposeGlowRef != null) _fxMatComposeGlow = Instantiate(fxMatComposeGlowRef);
                return _fxMatComposeGlow;
            }
        }

        Material fxMatComposeOutline {
            get {
                if (_fxMatComposeOutline == null && fxMatComposeOutlineRef != null) _fxMatComposeOutline = Instantiate(fxMatComposeOutlineRef);
                return _fxMatComposeOutline;
            }
        }

        Material fxMatBlurGlow {
            get {
                if (_fxMatBlurGlow == null && fxMatBlurGlowRef != null) _fxMatBlurGlow = Instantiate(fxMatBlurGlowRef);
                return _fxMatBlurGlow;
            }
        }

        Material fxMatBlurOutline {
            get {
                if (_fxMatBlurOutline == null && fxMatBlurOutlineRef != null) _fxMatBlurOutline = Instantiate(fxMatBlurOutlineRef);
                return _fxMatBlurOutline;
            }
        }

        static Vector3[] offsets;

        float fadeStartTime;
        FadingState fading = FadingState.NoFading;
        CommandBuffer cbMask, cbSeeThrough, cbGlow, cbOutline, cbOverlay, cbInnerGlow;
        CommandBuffer cbSmoothBlend;
        int[] mipGlowBuffers, mipOutlineBuffers;
        int glowRT, outlineRT;
        static Mesh quadMesh;
        int sourceRT;
        Matrix4x4 quadGlowMatrix, quadOutlineMatrix;
        Vector3[] corners;
        RenderTextureDescriptor sourceDesc;
        Color debugColor, blackColor;
        Visibility lastOutlineVisibility;
        bool requireUpdateMaterial;
        bool usingPipeline = false; // set to false to avoid editor warning due to conditional code
        float occlusionCheckLastTime;
        bool lastOcclusionTestResult;

        void OnEnable() {
            //Debug.Log("Effect on enable:" + name);

            lastOutlineVisibility = outlineVisibility;
            debugColor = new Color(1f, 0f, 0f, 0.5f);
            blackColor = new Color(0, 0, 0, 0);
            if (offsets == null || offsets.Length != 8) {
                offsets = new Vector3[] {
                    Vector3.up,
                    Vector3.right,
                    Vector3.down,
                    Vector3.left,
                    new Vector3 (-TAU, TAU, 0),
                    new Vector3 (TAU, TAU, 0),
                    new Vector3 (TAU, -TAU, 0),
                    new Vector3 (-TAU, -TAU, 0)
                };
            }
            if (corners == null || corners.Length != 8) {
                corners = new Vector3[8];
            }
            if (quadMesh == null) {
                BuildQuad();
            }
            if (target == null) {
                //target = customTarget == null ? transform : customTarget;
                target = transform;
            }
            if (profileSync && profile != null) {
                profile.Load(this);
            }
            if (glowPasses == null || glowPasses.Length == 0) {
                glowPasses = new GlowPassData[4];
                glowPasses[0] = new GlowPassData() { offset = 4, alpha = 0.1f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[1] = new GlowPassData() { offset = 3, alpha = 0.2f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[2] = new GlowPassData() { offset = 2, alpha = 0.3f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[3] = new GlowPassData() { offset = 1, alpha = 0.4f, color = new Color(0.64f, 1f, 0f, 1f) };
            }
            sourceRT = Shader.PropertyToID("_HPSourceRT");

            CheckGeometrySupportDependencies();
            SetupMaterial();
#if UNITY_2019_1_OR_NEWER
            UnityEngine.Rendering.RenderPipelineManager.endCameraRendering -= SRPAfterCameraRenders;
            UnityEngine.Rendering.RenderPipelineManager.endCameraRendering += SRPAfterCameraRenders;
#endif
        }

        void OnDisable() {
#if UNITY_2019_1_OR_NEWER
            UnityEngine.Rendering.RenderPipelineManager.endCameraRendering -= SRPAfterCameraRenders;
#endif
            UpdateMaterialPropertiesNow();
        }


#if UNITY_2019_1_OR_NEWER
        void SRPAfterCameraRenders(ScriptableRenderContext context, Camera cam) {
            if (this == null || !isActiveAndEnabled) return;
            usingPipeline = true;
            DoOnRenderObject(cam);
        }
#endif

        void Reset() {
            SetupMaterial();
        }

        void DestroyMaterial(Material mat) {
            if (mat != null) DestroyImmediate(mat);
        }

        void DestroyMaterialArray(Material[] mm) {
            if (mm == null) return;
            for (int k = 0; k < mm.Length; k++) {
                DestroyMaterial(mm[k]);
            }
        }

        void OnDestroy() {
            if (rms != null) {
                for (int k = 0; k < rms.Length; k++) {
                    DestroyMaterialArray(rms[k].fxMatMask);
                    DestroyMaterialArray(rms[k].fxMatSolidColor);
                    DestroyMaterialArray(rms[k].fxMatSeeThroughInner);
                    DestroyMaterialArray(rms[k].fxMatSeeThroughBorder);
                    DestroyMaterialArray(rms[k].fxMatOverlay);
                    DestroyMaterialArray(rms[k].fxMatInnerGlow);
                }
            }

            DestroyMaterial(fxMatGlow);
            DestroyMaterial(fxMatOutline);
            DestroyMaterial(fxMatTarget);
            DestroyMaterial(fxMatComposeGlow);
            DestroyMaterial(fxMatComposeOutline);
            DestroyMaterial(fxMatBlurGlow);
            DestroyMaterial(fxMatBlurOutline);
        }

        void OnRenderObject() {
            if (usingPipeline) return;
            DoOnRenderObject(Camera.current);
        }

        /// <summary>
        /// Loads a profile into this effect
        /// </summary>
        public void ProfileLoad(HighlightProfile profile) {
            if (profile != null) {
                profile.Load(this);
            }
        }

        /// <summary>
        /// Reloads currently assigned profile
        /// </summary>
        public void ProfileReload() {
            if (profile != null) {
                profile.Load(this);
            }
        }


        /// <summary>
        /// Save current settings into given profile
        /// </summary>
        public void ProfileSaveChanges(HighlightProfile profile) {
            if (profile != null) {
                profile.Save(this);
            }
        }

        /// <summary>
        /// Save current settings into current profile
        /// </summary>
        public void ProfileSaveChanges() {
            if (profile != null) {
                profile.Save(this);
            }
        }


        public void Refresh() {
            if (enabled) {
                SetupMaterial();
            }
        }


        void DoOnRenderObject(Camera cam) {

            if (cam == null) return;

            if (!reflectionProbes && cam.cameraType == CameraType.Reflection)
                return;

            RenderOccluders(cam);

#if UNITY_EDITOR
            if (!previewInEditor && !Application.isPlaying)
                return;
#endif

            if (requireUpdateMaterial) {
                requireUpdateMaterial = false;
                UpdateMaterialPropertiesNow();
            }

            bool seeThroughReal = seeThroughIntensity > 0 && (this.seeThrough == SeeThroughMode.AlwaysWhenOccluded || (this.seeThrough == SeeThroughMode.WhenHighlighted && _highlighted));
            if (cancelSeeThroughThisFrame) {
                cancelSeeThroughThisFrame = false;
                seeThroughReal = false;
            }
            if (seeThroughReal && seeThroughOccluderMask != -1) {
                seeThroughReal = CheckOcclusion(cam);
            }

            if (!_highlighted && !seeThroughReal) {
                return;
            }

            if (rms == null) return;

            // Check camera culling mask
            int cullingMask = cam.cullingMask;

            // Ensure renderers are valid and visible (in case LODgroup has changed active renderer)
            if (!ignoreObjectVisibility) {
                for (int k = 0; k < rmsCount; k++) {
                    if (rms[k].renderer != null && rms[k].renderer.isVisible != rms[k].renderWasVisibleDuringSetup) {
                        SetupMaterial();
                        break;
                    }
                }
            }

            // Apply effect
            float glowReal = _highlighted ? this.glow : 0;
            int layer = gameObject.layer;

            if (fxMatMask == null)
                return;

            // Check smooth blend ztesting capability
            bool useSmoothGlow = glow > 0 && glowQuality == QualityLevel.Highest;
            bool useSmoothOutline = outline > 0 && outlineQuality == QualityLevel.Highest;
            bool useSmoothBlend = useSmoothGlow || useSmoothOutline;
            if (useSmoothBlend) {
                if (useSmoothGlow && useSmoothOutline) {
                    outlineVisibility = glowVisibility;
                }
            }
            Visibility smoothGlowVisibility = glowVisibility;
            Visibility smoothOutlineVisibility = outlineVisibility;
#if UNITY_EDITOR
            if (useSmoothBlend && cam.cameraType == CameraType.SceneView) {
                smoothGlowVisibility = smoothOutlineVisibility = Visibility.AlwaysOnTop;
            }
#endif
            if (useSmoothBlend) {
                if (cam.allowMSAA && QualitySettings.antiAliasing > 1) {
                    if (depthClip) {
                        cam.depthTextureMode |= DepthTextureMode.Depth;
                    }
                    smoothGlowVisibility = smoothOutlineVisibility = Visibility.AlwaysOnTop;
                } else if (UnityEngine.XR.XRSettings.enabled && Application.isPlaying) {
                    if (depthClip) {
                        cam.depthTextureMode |= DepthTextureMode.Depth;
                    }
                    smoothGlowVisibility = smoothOutlineVisibility = Visibility.AlwaysOnTop;
                }
            }

            // First create masks
            bool independentFullScreenNotExecuted = true;
            for (int k = 0; k < rmsCount; k++) {
                rms[k].render = false;
                Transform t = rms[k].transform;
                if (t == null)
                    continue;
                Mesh mesh = rms[k].mesh;
                if (mesh == null)
                    continue;
                if (((1 << t.gameObject.layer) & cullingMask) == 0)
                    continue;
                if (!rms[k].renderer.isVisible)
                    continue;
                rms[k].render = true;

                if (!rms[k].skinnedMesh) {
                    Vector3 lossyScale = t.lossyScale;
                    Vector3 position = t.position;
                    if (rms[k].bakedTransform) {
                        if (rms[k].currentPosition != t.position || rms[k].currentRotation != t.eulerAngles || rms[k].currentScale != t.lossyScale) {
                            BakeTransform(k, true);
                        }
                        rms[k].renderingMatrix = Matrix4x4.identity;
                    } else {
                        rms[k].renderingMatrix = Matrix4x4.TRS(position, t.rotation, lossyScale);
                    }
                }

                // Outline
                if (outlineIndependent) {
                    if (useSmoothBlend && independentFullScreenNotExecuted) {
                        independentFullScreenNotExecuted = false;
                        fxMatClearStencil.SetPass(0);
                        Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
                    } else if (outline > 0) {
                        Material mat = fxMatOutline;
                        for (int l = 0; l < mesh.subMeshCount; l++) {
                            if (outlineQuality == QualityLevel.High) {
                                for (int o = 0; o < offsets.Length; o++) {
                                    Vector3 direction = offsets[o] * (outlineWidth / 100f);
                                    direction.y *= cam.aspect;
                                    mat.SetVector("_OutlineDirection", direction);
                                    if (rms[k].skinnedMesh) {
                                        cbOutline.Clear();
                                        cbOutline.DrawRenderer(rms[k].renderer, mat, l, 1);
                                        Graphics.ExecuteCommandBuffer(cbOutline);
                                    } else {
                                        mat.SetPass(1);
                                        Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                                    }
                                }
                            } else {
                                if (rms[k].skinnedMesh) {
                                    cbOutline.Clear();
                                    cbOutline.DrawRenderer(rms[k].renderer, mat, l, 1);
                                    Graphics.ExecuteCommandBuffer(cbOutline);
                                } else {
                                    mat.SetPass(1);
                                    Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                                }
                            }
                        }
                    }
                }
                if (rms[k].skinnedMesh) {
                    cbMask.Clear();
                    for (int l = 0; l < mesh.subMeshCount; l++) {
                        if (_highlighted && ((outline > 0 && smoothOutlineVisibility != Visibility.Normal) || (glow > 0 && smoothGlowVisibility != Visibility.Normal) || (innerGlow > 0 && innerGlowVisibility != Visibility.Normal))) {
                            rms[k].fxMatMask[l].SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                        } else {
                            rms[k].fxMatMask[l].SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                        }
                        cbMask.DrawRenderer(rms[k].renderer, rms[k].fxMatMask[l], l);
                    }
                    Graphics.ExecuteCommandBuffer(cbMask);
                } else {

                    for (int l = 0; l < mesh.subMeshCount; l++) {
                        if (_highlighted && ((outline > 0 && smoothOutlineVisibility != Visibility.Normal) || (glow > 0 && smoothGlowVisibility != Visibility.Normal) || (innerGlow > 0 && innerGlowVisibility != Visibility.Normal))) {
                            rms[k].fxMatMask[l].SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                        } else {
                            rms[k].fxMatMask[l].SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                        }
                        rms[k].fxMatMask[l].SetPass(0);
                        Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                    }
                }
            }

            // Compute tweening
            float fade = 1f;
            if (fading != FadingState.NoFading) {
                if (fading == FadingState.FadingIn) {
                    if (fadeInDuration > 0) {
                        fade = (Time.time - fadeStartTime) / fadeInDuration;
                        if (fade > 1f) {
                            fade = 1f;
                            fading = FadingState.NoFading;
                        }
                    }
                } else if (fadeOutDuration > 0) {
                    fade = 1f - (Time.time - fadeStartTime) / fadeOutDuration;
                    if (fade < 0f) {
                        fade = 0f;
                        fading = FadingState.NoFading;
                        _highlighted = false;
                        if (OnObjectHighlightEnd != null) {
                            OnObjectHighlightEnd(gameObject);
                        }
                        SendMessage("HighlightEnd", null, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }

            if (glowQuality == QualityLevel.High) {
                glowReal *= 0.25f;
            }

            int smoothRTWidth = 0;
            int smoothRTHeight = 0;
            Bounds smoothBounds = new Bounds();

            if (useSmoothBlend) {
                // Prepare smooth outer glow / outline target
                if (cbSmoothBlend == null) {
                    CheckBlurCommandBuffer();
                }
                cbSmoothBlend.Clear();
                smoothRTWidth = cam.pixelWidth;
                smoothRTHeight = cam.pixelHeight;
                if (smoothRTHeight <= 0) {
                    smoothRTHeight = 1;
                }
                if (UnityEngine.XR.XRSettings.enabled && Application.isPlaying) {
                    sourceDesc = UnityEngine.XR.XRSettings.eyeTextureDesc;
                } else {
                    sourceDesc = new RenderTextureDescriptor(smoothRTWidth, smoothRTHeight, RenderTextureFormat.Default);
                    sourceDesc.volumeDepth = 1;
                }
                sourceDesc.msaaSamples = 1;
                sourceDesc.useMipMap = false;
                sourceDesc.depthBufferBits = 0;

                cbSmoothBlend.GetTemporaryRT(sourceRT, sourceDesc, FilterMode.Bilinear);
                if ((glow > 0 && smoothGlowVisibility == Visibility.AlwaysOnTop) || (outline > 0 && smoothOutlineVisibility == Visibility.AlwaysOnTop)) {
                    cbSmoothBlend.SetRenderTarget(sourceRT);
                } else {
                    cbSmoothBlend.SetRenderTarget(sourceRT, BuiltinRenderTextureType.CameraTarget);
                }
                cbSmoothBlend.ClearRenderTarget(false, true, new Color(0, 0, 0, 0));
            }

            // Add effects
            for (int k = 0; k < rmsCount; k++) {
                if (!rms[k].render)
                    continue;
                Mesh mesh = rms[k].mesh;

                // See-Through
                if (seeThroughReal) {
                    bool usesSeeThroughBorder = seeThroughBorder * seeThroughBorderWidth > 0;
                    if (rms[k].skinnedMesh) {
                        cbSeeThrough.Clear();
                        for (int l = 0; l < mesh.subMeshCount; l++) {
                            if (l < rms[k].fxMatSeeThroughInner.Length && rms[k].fxMatSeeThroughInner[l] != null) {
                                cbSeeThrough.DrawRenderer(rms[k].renderer, rms[k].fxMatSeeThroughInner[l], l);
                                if (usesSeeThroughBorder) {
                                    cbSeeThrough.DrawRenderer(rms[k].renderer, rms[k].fxMatSeeThroughBorder[l], l);
                                }
                            }
                        }
                        Graphics.ExecuteCommandBuffer(cbSeeThrough);
                    } else {
                        for (int l = 0; l < mesh.subMeshCount; l++) {
                            if (l < rms[k].fxMatSeeThroughInner.Length && rms[k].fxMatSeeThroughInner[l] != null) {
                                rms[k].fxMatSeeThroughInner[l].SetPass(0);
                                Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                                if (usesSeeThroughBorder) {
                                    rms[k].fxMatSeeThroughBorder[l].SetPass(0);
                                    Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                                }
                            }
                        }
                    }
                }

                if (!_highlighted)
                    continue;

                if (useSmoothBlend) {
                    if (k == 0) {
                        smoothBounds = rms[k].renderer.bounds;
                    } else {
                        smoothBounds.Encapsulate(rms[k].renderer.bounds);
                    }
                }

                for (int l = 0; l < mesh.subMeshCount; l++) {

                    // Render object body for glow/outline highest quality
                    if (useSmoothBlend) {
                        if (l < rms[k].fxMatSolidColor.Length) {
                            if (rms[k].skinnedMesh) {
                                cbSmoothBlend.DrawRenderer(rms[k].renderer, rms[k].fxMatSolidColor[l], l);
                            } else {
                                cbSmoothBlend.DrawMesh(mesh, rms[k].renderingMatrix, rms[k].fxMatSolidColor[l], l);
                            }
                        }
                    }

                    // Glow
                    if (glow > 0 && glowQuality != QualityLevel.Highest) {
                        Material mat = fxMatGlow;
                        mat.SetVector("_GlowDirection", Vector3.zero);
                        for (int j = 0; j < glowPasses.Length; j++) {
                            mat.SetColor("_GlowColor", glowPasses[j].color);
                            mat.SetVector("_Glow", new Vector4(fade * glowReal * glowPasses[j].alpha, glowPasses[j].offset * glowWidth / 100f, glowMagicNumber1, glowMagicNumber2));
                            if (glowQuality == QualityLevel.High) {
                                for (int o = 0; o < offsets.Length; o++) {
                                    Vector3 direction = offsets[o];
                                    direction.y *= cam.aspect;
                                    mat.SetVector("_GlowDirection", direction);

                                    if (rms[k].skinnedMesh) {
                                        cbGlow.Clear();
                                        cbGlow.DrawRenderer(rms[k].renderer, mat, l);
                                        Graphics.ExecuteCommandBuffer(cbGlow);
                                    } else {
                                        mat.SetPass(0);
                                        Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                                    }
                                }
                            } else {
                                if (rms[k].skinnedMesh) {
                                    cbGlow.Clear();
                                    cbGlow.DrawRenderer(rms[k].renderer, mat, l);
                                    Graphics.ExecuteCommandBuffer(cbGlow);
                                } else {
                                    mat.SetPass(0);
                                    Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                                }
                            }
                        }
                    }

                    // Outline
                    if (outline > 0 && outlineQuality != QualityLevel.Highest) {
                        Color outlineColor = this.outlineColor;
                        outlineColor.a = outline * fade;
                        Material mat = fxMatOutline;
                        mat.SetColor("_OutlineColor", outlineColor);
                        if (outlineQuality == QualityLevel.High) {
                            for (int o = 0; o < offsets.Length; o++) {
                                Vector3 direction = offsets[o] * (outlineWidth / 100f);
                                direction.y *= cam.aspect;
                                mat.SetVector("_OutlineDirection", direction);
                                if (rms[k].skinnedMesh) {
                                    cbOutline.Clear();
                                    cbOutline.DrawRenderer(rms[k].renderer, mat, l, 0);
                                    Graphics.ExecuteCommandBuffer(cbOutline);
                                } else {
                                    mat.SetPass(0);
                                    Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                                }
                            }
                        } else {
                            if (rms[k].skinnedMesh) {
                                cbOutline.Clear();
                                cbOutline.DrawRenderer(rms[k].renderer, mat, l, 0);
                                Graphics.ExecuteCommandBuffer(cbOutline);
                            } else {
                                mat.SetPass(0);
                                Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                            }
                        }
                    }

                    // Overlay
                    if (overlay > 0) {
                        if (l < rms[k].fxMatOverlay.Length && rms[k].fxMatOverlay[l] != null) {
                            Color overlayColor = this.overlayColor;
                            overlayColor.a = overlay * fade;
                            rms[k].fxMatOverlay[l].color = overlayColor;
                            rms[k].fxMatOverlay[l].SetVector("_OverlayData", new Vector3(overlayAnimationSpeed, overlayMinIntensity, overlayBlending));

                            if (rms[k].skinnedMesh) {
                                cbOverlay.Clear();
                                cbOverlay.DrawRenderer(rms[k].renderer, rms[k].fxMatOverlay[l], l);
                                Graphics.ExecuteCommandBuffer(cbOverlay);
                            } else {
                                rms[k].fxMatOverlay[l].SetPass(0);
                                Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                            }
                        }
                    }
                    // Inner Glow
                    if (innerGlow > 0 && innerGlowWidth > 0) {
                        if (l < rms[k].fxMatInnerGlow.Length && rms[k].fxMatInnerGlow[l] != null) {
                            Color innerGlowColorA = innerGlowColor;
                            innerGlowColorA.a = innerGlow * fade;
                            rms[k].fxMatInnerGlow[l].SetColor("_Color", innerGlowColorA);

                            if (rms[k].skinnedMesh) {
                                cbInnerGlow.Clear();
                                cbInnerGlow.DrawRenderer(rms[k].renderer, rms[k].fxMatInnerGlow[l], l);
                                Graphics.ExecuteCommandBuffer(cbInnerGlow);
                            } else {
                                rms[k].fxMatInnerGlow[l].SetPass(0);
                                Graphics.DrawMeshNow(mesh, rms[k].renderingMatrix, l);
                            }
                        }
                    }
                }

                // Target
                if (targetFX) {
                    float fadeOut = 1f;
                    if (Application.isPlaying) {
                        fadeOut = (Time.time - highlightStartTime);
                        if (fadeOut >= targetFXStayDuration) {
                            fadeOut -= targetFXStayDuration;
                            fadeOut = 1f - fadeOut;
                        }
                        if (fadeOut > 1f) {
                            fadeOut = 1f;
                        }
                    }
                    if (fadeOut > 0) {
                        float scaleT = 1f;
                        float time;
                        if (Application.isPlaying) {
                            scaleT = (Time.time - highlightStartTime) / targetFXTransitionDuration;
                            if (scaleT > 1f) {
                                scaleT = 1f;
                            }
                            scaleT = Mathf.Sin(scaleT * Mathf.PI * 0.5f);
                            time = Time.time;
                        } else {
                            time = (float)DateTime.Now.Subtract(DateTime.Today).TotalSeconds;
                        }
                        Bounds bounds = rms[k].renderer.bounds;
                        Vector3 size = bounds.size;
                        float minSize = size.x;
                        if (size.y < minSize) {
                            minSize = size.y;
                        }
                        if (size.z < minSize) {
                            minSize = size.z;
                        }
                        size.x = size.y = size.z = minSize;
                        size = Vector3.Lerp(size * targetFXInitialScale, size * targetFXEndScale, scaleT);
                        Quaternion camRot = Quaternion.LookRotation(cam.transform.position - rms[k].transform.position);
                        Quaternion rotation = Quaternion.Euler(0, 0, time * targetFXRotationSpeed);
                        camRot *= rotation;
                        Vector3 center = targetFXCenter != null ? targetFXCenter.transform.position : bounds.center;
                        Matrix4x4 m = Matrix4x4.TRS(center, camRot, size);
                        Color color = targetFXColor;
                        color.a *= fade * fadeOut;
                        Material mat = fxMatTarget;
                        mat.color = color;
                        mat.SetPass(0);
                        Graphics.DrawMeshNow(quadMesh, m);
                    }
                }
            }

            if (useSmoothBlend) {
                if (ComputeSmoothQuadMatrix(cam, smoothBounds)) {
                    // Smooth Glow
                    if (useSmoothGlow) {
                        float intensity = glow * fade;
                        fxMatComposeGlow.color = new Color(glowHQColor.r * intensity, glowHQColor.g * intensity, glowHQColor.b * intensity, glowHQColor.a * intensity);
                        SmoothGlow(smoothRTWidth / glowDownsampling, smoothRTHeight / glowDownsampling);
                    }

                    // Smooth Outline
                    if (useSmoothOutline) {
                        fxMatComposeOutline.color = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 5f * outlineColor.a * outline * fade);
                        SmoothOutline(smoothRTWidth / outlineDownsampling, smoothRTHeight / outlineDownsampling);
                    }

                    // Bit result
                    ComposeSmoothBlend(smoothGlowVisibility, smoothOutlineVisibility);
                }
            }
        }

        bool ComputeSmoothQuadMatrix(Camera cam, Bounds bounds) {
            // Compute bounds in screen space and enlarge for glow space
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            corners[0] = new Vector3(min.x, min.y, min.z);
            corners[1] = new Vector3(min.x, min.y, max.z);
            corners[2] = new Vector3(max.x, min.y, min.z);
            corners[3] = new Vector3(max.x, min.y, max.z);
            corners[4] = new Vector3(min.x, max.y, min.z);
            corners[5] = new Vector3(min.x, max.y, max.z);
            corners[6] = new Vector3(max.x, max.y, min.z);
            corners[7] = new Vector3(max.x, max.y, max.z);
            Vector3 scrMin = new Vector3(float.MaxValue, float.MaxValue, 0);
            Vector3 scrMax = new Vector3(float.MinValue, float.MinValue, 0);
            float distance = float.MaxValue;
            for (int k = 0; k < corners.Length; k++) {
                corners[k] = cam.WorldToScreenPoint(corners[k]);
                if (corners[k].x < scrMin.x) {
                    scrMin.x = corners[k].x;
                }
                if (corners[k].y < scrMin.y) {
                    scrMin.y = corners[k].y;
                }
                if (corners[k].x > scrMax.x) {
                    scrMax.x = corners[k].x;
                }
                if (corners[k].y > scrMax.y) {
                    scrMax.y = corners[k].y;
                }
                if (corners[k].z < distance) {
                    distance = corners[k].z;
                    if (distance < cam.nearClipPlane) {
                        scrMin.x = scrMin.y = 0;
                        scrMax.x = cam.pixelWidth;
                        scrMax.y = cam.pixelHeight;
                        break;
                    }
                }
            }
            if (scrMax.y == scrMin.y)
                return false;

            if (distance < cam.nearClipPlane) {
                distance = cam.nearClipPlane + 0.01f;
            }
            scrMin.z = scrMax.z = distance;
            if (outline > 0) {
                BuildMatrix(cam, scrMin, scrMax, (int)(10 + 20 * outlineWidth + 5 * outlineDownsampling), ref quadOutlineMatrix);
            }
            if (glow > 0) {
                BuildMatrix(cam, scrMin, scrMax, (int)(20 + 30 * glowWidth + 10 * glowDownsampling), ref quadGlowMatrix);
            }
            return true;
        }

        void BuildMatrix(Camera cam, Vector3 scrMin, Vector3 scrMax, int border, ref Matrix4x4 quadMatrix) {

            // Insert padding to make room for effects
            scrMin.x -= border;
            scrMin.y -= border;
            scrMax.x += border;
            scrMax.y += border;

            // Back to world space
            Vector3 third = new Vector3(scrMax.x, scrMin.y, scrMin.z);
            scrMin = cam.ScreenToWorldPoint(scrMin);
            scrMax = cam.ScreenToWorldPoint(scrMax);
            third = cam.ScreenToWorldPoint(third);

            float width = Vector3.Distance(scrMin, third);
            float height = Vector3.Distance(scrMax, third);

            quadMatrix = Matrix4x4.TRS((scrMin + scrMax) * 0.5f, cam.transform.rotation, new Vector3(width, height, 1f));
        }

        void SmoothGlow(int rtWidth, int rtHeight) {
            const int blurPasses = 4;

            // Blur buffers
            int bufferCount = blurPasses * 2;
            if (mipGlowBuffers == null || mipGlowBuffers.Length != bufferCount) {
                mipGlowBuffers = new int[bufferCount];
                for (int k = 0; k < bufferCount; k++) {
                    mipGlowBuffers[k] = Shader.PropertyToID("_HPSmoothGlowTemp" + k);
                }
                glowRT = Shader.PropertyToID("_HPComposeGlowFinal");
                mipGlowBuffers[bufferCount - 2] = glowRT;
            }
            RenderTextureDescriptor glowDesc = sourceDesc;
            glowDesc.depthBufferBits = 0;
            if (glowDesc.vrUsage == VRTextureUsage.TwoEyes) {
                glowDesc.vrUsage = VRTextureUsage.None;
                fxMatBlurGlow.SetFloat("_StereoRendering", 0.5f);
                fxMatComposeGlow.SetFloat("_StereoRendering", 0.5f);
            } else {
                fxMatBlurGlow.SetFloat("_StereoRendering", 1f);
                fxMatComposeGlow.SetFloat("_StereoRendering", 1f);
            }

            for (int k = 0; k < bufferCount; k++) {
                float reduction = k / 2 + 2;
                int reducedWidth = (int)(rtWidth / reduction);
                int reducedHeight = (int)(rtHeight / reduction);
                if (reducedWidth <= 0) {
                    reducedWidth = 1;
                }
                if (reducedHeight <= 0) {
                    reducedHeight = 1;
                }
                glowDesc.width = reducedWidth;
                glowDesc.height = reducedHeight;
                cbSmoothBlend.GetTemporaryRT(mipGlowBuffers[k], glowDesc, FilterMode.Bilinear);
            }

            Material matBlur = fxMatBlurGlow;
            for (int k = 0; k < bufferCount - 1; k += 2) {
                if (k == 0) {
                    cbSmoothBlend.Blit(sourceRT, mipGlowBuffers[k + 1], matBlur, 0);
                } else {
                    cbSmoothBlend.Blit(mipGlowBuffers[k], mipGlowBuffers[k + 1], matBlur, 0);
                }
                cbSmoothBlend.Blit(mipGlowBuffers[k + 1], mipGlowBuffers[k], matBlur, 1);

                if (k < bufferCount - 2) {
                    cbSmoothBlend.Blit(mipGlowBuffers[k], mipGlowBuffers[k + 2], matBlur, 2);
                }
            }
        }

        void SmoothOutline(int rtWidth, int rtHeight) {
            const int blurPasses = 2;

            // Blur buffers
            int bufferCount = blurPasses * 2;
            if (mipOutlineBuffers == null || mipOutlineBuffers.Length != bufferCount) {
                mipOutlineBuffers = new int[bufferCount];
                for (int k = 0; k < bufferCount; k++) {
                    mipOutlineBuffers[k] = Shader.PropertyToID("_HPSmoothOutlineTemp" + k);
                }
                outlineRT = Shader.PropertyToID("_HPComposeOutlineFinal");
                mipOutlineBuffers[bufferCount - 2] = outlineRT;
            }
            RenderTextureDescriptor outlineDesc = sourceDesc;
            outlineDesc.depthBufferBits = 0;
            if (outlineDesc.vrUsage == VRTextureUsage.TwoEyes) {
                outlineDesc.vrUsage = VRTextureUsage.None;
                fxMatBlurOutline.SetFloat("_StereoRendering", 0.5f);
                fxMatComposeOutline.SetFloat("_StereoRendering", 0.5f);
            } else {
                fxMatBlurOutline.SetFloat("_StereoRendering", 1f);
                fxMatComposeOutline.SetFloat("_StereoRendering", 1f);
            }

            for (int k = 0; k < bufferCount; k++) {
                float reduction = k / 2 + 2;
                int reducedWidth = (int)(rtWidth / reduction);
                int reducedHeight = (int)(rtHeight / reduction);
                if (reducedWidth <= 0) {
                    reducedWidth = 1;
                }
                if (reducedHeight <= 0) {
                    reducedHeight = 1;
                }
                outlineDesc.width = reducedWidth;
                outlineDesc.height = reducedHeight;
                cbSmoothBlend.GetTemporaryRT(mipOutlineBuffers[k], outlineDesc, FilterMode.Bilinear);
            }

            Material matBlur = fxMatBlurOutline;
            for (int k = 0; k < bufferCount - 1; k += 2) {
                if (k == 0) {
                    cbSmoothBlend.Blit(sourceRT, mipOutlineBuffers[k + 1], matBlur, 0);
                } else {
                    cbSmoothBlend.Blit(mipOutlineBuffers[k], mipOutlineBuffers[k + 1], matBlur, 0);
                }
                cbSmoothBlend.Blit(mipOutlineBuffers[k + 1], mipOutlineBuffers[k], matBlur, 1);

                if (k < bufferCount - 2) {
                    cbSmoothBlend.Blit(mipOutlineBuffers[k], mipOutlineBuffers[k + 2], matBlur, 2);
                }
            }
        }

        void ComposeSmoothBlend(Visibility smoothGlowVisibility, Visibility smoothOutlineVisibility) {
            bool renderSmoothGlow = glow > 0 && glowQuality == QualityLevel.Highest;
            if (renderSmoothGlow) {
                Material matComposeGlow = fxMatComposeGlow;
                matComposeGlow.SetVector("_Flip", (UnityEngine.XR.XRSettings.enabled && flipY) ? new Vector3(1, -1, 0) : new Vector3(0, 1, 0));
                if (glowOptimalBlit) {
                    cbSmoothBlend.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                    matComposeGlow.SetInt("_ZTest", GetZTestValue(smoothGlowVisibility));
                    matComposeGlow.SetColor("_Debug", glowBlitDebug ? debugColor : blackColor);
                    cbSmoothBlend.DrawMesh(quadMesh, quadGlowMatrix, matComposeGlow, 0, 0);
                } else {
                    cbSmoothBlend.Blit(glowRT, BuiltinRenderTextureType.CameraTarget, matComposeGlow, 1);
                }
            }
            bool renderSmoothOutline = outline > 0 && outlineQuality == QualityLevel.Highest;
            if (renderSmoothOutline) {
                Material matComposeOutline = fxMatComposeOutline;
                matComposeOutline.SetVector("_Flip", (UnityEngine.XR.XRSettings.enabled && flipY) ? new Vector3(1, -1, 0) : new Vector3(0, 1, 0));
                if (outlineOptimalBlit) {
                    cbSmoothBlend.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                    matComposeOutline.SetInt("_ZTest", GetZTestValue(smoothOutlineVisibility));
                    matComposeOutline.SetColor("_Debug", outlineBlitDebug ? debugColor : blackColor);
                    cbSmoothBlend.DrawMesh(quadMesh, quadOutlineMatrix, matComposeOutline, 0, 0);
                } else {
                    cbSmoothBlend.Blit(outlineRT, BuiltinRenderTextureType.CameraTarget, matComposeOutline, 1);
                }
            }
            // Release render textures
            if (renderSmoothGlow) {
                for (int k = 0; k < mipGlowBuffers.Length; k++) {
                    cbSmoothBlend.ReleaseTemporaryRT(mipGlowBuffers[k]);
                }
            }
            if (renderSmoothOutline) {
                for (int k = 0; k < mipOutlineBuffers.Length; k++) {
                    cbSmoothBlend.ReleaseTemporaryRT(mipOutlineBuffers[k]);
                }
            }

            cbSmoothBlend.ReleaseTemporaryRT(sourceRT);

            Graphics.ExecuteCommandBuffer(cbSmoothBlend);
        }

        void InitMaterial(ref Material material, string shaderName) {
            if (material == null) {
                Shader shaderFX = Shader.Find(shaderName);
                if (shaderFX == null) {
                    Debug.LogError("Shader " + shaderName + " not found.");
                    enabled = false;
                    return;
                }
                material = new Material(shaderFX);
            }
        }

        //public void SetTarget(Transform transform = null) {


        //    if (_highlighted) {
        //        SetHighlighted(false);
        //    }

        //    target = transform;

        //    this.customRenderers = customRenderers;

        //    SetupMaterial();
        //}

        public void SetTargetRenderers(Renderer[] renderers)
        {
            if (renderers == customRenderers && renderers?.Length == customRenderers?.Length)
                return;

            var cacheHL = _highlighted;

            if (_highlighted)
            {
                SetHighlighted(false);
            }

            customRenderers = renderers;

            SetupMaterial();

            if (cacheHL && renderers != null)
                SetHighlighted(true);
        }

        /// <summary>
        /// Start or finish highlight on the object
        /// </summary>
        public void SetHighlighted(bool state) {

            if (!Application.isPlaying) {
                _highlighted = state;
                return;
            }

            if (fading == FadingState.NoFading) {
                fadeStartTime = Time.time;
            }

            if (state && !ignore) {
                if (_highlighted && fading == FadingState.NoFading) {
                    return;
                }
                if (OnObjectHighlightStart != null) {
                    if (!OnObjectHighlightStart(gameObject)) {
                        return;
                    }
                }
                SendMessage("HighlightStart", null, SendMessageOptions.DontRequireReceiver);
                highlightStartTime = Time.time;
                if (fadeInDuration > 0) {
                    if (fading == FadingState.FadingOut) {
                        float remaining = fadeOutDuration - (Time.time - fadeStartTime);
                        fadeStartTime = Time.time - remaining;
                    }
                    fading = FadingState.FadingIn;
                } else {
                    fading = FadingState.NoFading;
                }
                _highlighted = true;
                requireUpdateMaterial = true;
            } else if (_highlighted) {
                if (fadeOutDuration > 0) {
                    if (fading == FadingState.FadingIn) {
                        float elapsed = Time.time - fadeStartTime;
                        fadeStartTime = Time.time + elapsed - fadeInDuration;
                    }
                    fading = FadingState.FadingOut; // when fade out ends, highlighted will be set to false in OnRenderObject
                } else {
                    fading = FadingState.NoFading;
                    _highlighted = false;
                    if (OnObjectHighlightEnd != null) {
                        OnObjectHighlightEnd(gameObject);
                    }
                    SendMessage("HighlightEnd", null, SendMessageOptions.DontRequireReceiver);
                    requireUpdateMaterial = true;
                }
            }
        }

        void SetupMaterial() {

#if UNITY_EDITOR
            staticChildren = false;
#endif
 
            if (fxMatMask == null)
                return;

            Renderer[] rr = customRenderers;

            if((rr == null || rr.Length == 0) && target != null)
            {
                switch (effectGroup)
                {
                    case TargetOptions.OnlyThisObject:
                        Renderer renderer = target.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            rr = new Renderer[1];
                            rr[0] = renderer;
                        }
                        break;
                    case TargetOptions.RootToChildren:
                        Transform root = target;
                        while (root.parent != null)
                        {
                            root = root.parent;
                        }
                        rr = root.GetComponentsInChildren<Renderer>();
                        break;
                    case TargetOptions.LayerInScene:
                        {
                            HighlightEffect eg = this;
                            if (target != transform)
                            {
                                HighlightEffect targetEffect = target.GetComponent<HighlightEffect>();
                                if (targetEffect != null)
                                {
                                    eg = targetEffect;
                                }
                            }
                            rr = FindRenderersWithLayerInScene(eg.effectGroupLayer);
                        }
                        break;
                    case TargetOptions.LayerInChildren:
                        {
                            HighlightEffect eg = this;
                            if (target != transform)
                            {
                                HighlightEffect targetEffect = target.GetComponent<HighlightEffect>();
                                if (targetEffect != null)
                                {
                                    eg = targetEffect;
                                }
                            }
                            rr = FindRenderersWithLayerInChildren(eg.effectGroupLayer);
                        }
                        break;
                    default:
                        rr = target.GetComponentsInChildren<Renderer>();
                        break;
                }
            }
 
            if (rr == null) {
                rr = new Renderer[0];
            }

            if (rms == null || rms.Length < rr.Length) {
                rms = new ModelMaterials[rr.Length];
            }

            rmsCount = 0;
            for (int k = 0; k < rr.Length; k++) {
                rms[rmsCount] = new ModelMaterials();
                Renderer renderer = rr[k];
                rms[rmsCount].renderer = renderer;
                rms[rmsCount].renderWasVisibleDuringSetup = renderer.isVisible;
 
                if (OnRendererHighlightStart != null) {
                    if (!OnRendererHighlightStart(renderer)) {
                        rmsCount++;
                        continue;
                    }
                }

                if (renderer is SkinnedMeshRenderer) {
                    // ignore cloth skinned renderers
                    rms[rmsCount].skinnedMesh = true;
                    rms[rmsCount].mesh = ((SkinnedMeshRenderer)renderer).sharedMesh;
                    CheckCommandBuffers();
                } else if (Application.isPlaying && renderer.isPartOfStaticBatch) {
                    // static batched objects need to have a mesh collider in order to use its original mesh
                    MeshCollider mc = renderer.GetComponent<MeshCollider>();
                    if (mc != null) {
                        rms[rmsCount].mesh = mc.sharedMesh;
                    }
                } else {
                    MeshFilter mf = renderer.GetComponent<MeshFilter>();
                    if (mf != null) {
                        rms[rmsCount].mesh = mf.sharedMesh;

#if UNITY_EDITOR
                        if (renderer.gameObject.isStatic && renderer.GetComponent<MeshCollider>() == null) {
                            staticChildren = true;
                        }
#endif

                    }
                }

                if (rms[rmsCount].mesh == null) {
                    continue;
                }

                rms[rmsCount].transform = renderer.transform;
                rms[rmsCount].fxMatMask = Fork(fxMatMask, rms[rmsCount].mesh);
                rms[rmsCount].fxMatSeeThroughInner = Fork(fxMatSeeThroughInner, rms[rmsCount].mesh);
                rms[rmsCount].fxMatSeeThroughBorder = Fork(fxMatSeeThroughBorder, rms[rmsCount].mesh);
                rms[rmsCount].fxMatOverlay = Fork(fxMatOverlay, rms[rmsCount].mesh);
                rms[rmsCount].fxMatInnerGlow = Fork(fxMatInnerGlow, rms[rmsCount].mesh);
                rms[rmsCount].fxMatSolidColor = Fork(fxMatSolidColor, rms[rmsCount].mesh);
                rms[rmsCount].originalMesh = rms[rmsCount].mesh;
                if (!rms[rmsCount].skinnedMesh) {
                    if (innerGlow > 0 || (glow > 0 && glowQuality != QualityLevel.Highest) || (outline > 0 && outlineQuality != QualityLevel.Highest)) {
                        AverageNormals(rmsCount);
                    }
                    // check if scale is negative
                    BakeTransform(rmsCount, true);
                }
                rmsCount++;
            }

#if UNITY_EDITOR
            // Avoids command buffer issue when refreshing asset inside the Editor
            if (!Application.isPlaying) {
                mipGlowBuffers = null;
                mipOutlineBuffers = null;
            }
#endif

            UpdateMaterialPropertiesNow();
        }

        List<Renderer> tempRR;

        Renderer[] FindRenderersWithLayerInScene(LayerMask layer) {
            Renderer[] rr = FindObjectsOfType<Renderer>();
            if (tempRR == null) {
                tempRR = new List<Renderer>();
            } else {
                tempRR.Clear();
            }
            for (var i = 0; i < rr.Length; i++) {
                Renderer r = rr[i];
                if (((1 << r.gameObject.layer) & layer) != 0) {
                    tempRR.Add(r);
                }
            }
            return tempRR.ToArray();
        }

        Renderer[] FindRenderersWithLayerInChildren(LayerMask layer) {
            Renderer[] rr = target.GetComponentsInChildren<Renderer>();
            if (tempRR == null) {
                tempRR = new List<Renderer>();
            } else {
                tempRR.Clear();
            }
            for (var i = 0; i < rr.Length; i++) {
                Renderer r = rr[i];
                if (((1 << r.gameObject.layer) & layer) != 0) {
                    tempRR.Add(r);
                }
            }
            return tempRR.ToArray();
        }

        void CheckGeometrySupportDependencies() {
            InitMaterial(ref fxMatMask, "HighlightPlus/Geometry/Mask");
            InitMaterial(ref fxMatOverlay, "HighlightPlus/Geometry/Overlay");
            InitMaterial(ref fxMatSeeThroughInner, "HighlightPlus/Geometry/SeeThroughInner");
            InitMaterial(ref fxMatSeeThroughBorder, "HighlightPlus/Geometry/SeeThroughBorder");
            InitMaterial(ref fxMatSolidColor, "HighlightPlus/Geometry/SolidColor");
            InitMaterial(ref fxMatClearStencil, "HighlightPlus/ClearStencil");
            InitMaterial(ref fxMatOutlineRef, "HighlightPlus/Geometry/Outline");
            InitMaterial(ref fxMatGlowRef, "HighlightPlus/Geometry/Glow");
            InitMaterial(ref fxMatInnerGlow, "HighlightPlus/Geometry/InnerGlow");
            InitMaterial(ref fxMatTargetRef, "HighlightPlus/Geometry/Target");
            InitMaterial(ref fxMatComposeOutlineRef, "HighlightPlus/Geometry/ComposeOutline");
            InitMaterial(ref fxMatComposeGlowRef, "HighlightPlus/Geometry/ComposeGlow");
            InitMaterial(ref fxMatBlurOutlineRef, "HighlightPlus/Geometry/BlurOutline");
            InitMaterial(ref fxMatBlurGlowRef, "HighlightPlus/Geometry/BlurGlow");
        }

        void CheckCommandBuffers() {
            if (cbMask == null) {
                cbMask = new CommandBuffer();
                cbMask.name = "Mask";
            }
            if (cbSeeThrough == null) {
                cbSeeThrough = new CommandBuffer();
                cbSeeThrough.name = "See Through";
            }
            if (cbGlow == null) {
                cbGlow = new CommandBuffer();
                cbGlow.name = "Outer Glow";
            }
            if (cbOutline == null) {
                cbOutline = new CommandBuffer();
                cbOutline.name = "Outline";
            }
            if (cbOverlay == null) {
                cbOverlay = new CommandBuffer();
                cbOverlay.name = "Overlay";
            }
            if (cbInnerGlow == null) {
                cbInnerGlow = new CommandBuffer();
                cbInnerGlow.name = "Inner Glow";
            }
        }

        void CheckBlurCommandBuffer() {
            if (cbSmoothBlend == null) {
                cbSmoothBlend = new CommandBuffer();
                cbSmoothBlend.name = "Smooth Blend";
            }
        }

        Material[] Fork(Material mat, Mesh mesh) {
            if (mesh == null)
                return null;
            int count = mesh.subMeshCount;
            Material[] mm = new Material[count];
            for (int k = 0; k < count; k++) {
                mm[k] = Instantiate<Material>(mat);
            }
            return mm;
        }

        void BakeTransform(int objIndex, bool duplicateMesh) {
            if (rms[objIndex].mesh == null)
                return;
            Transform t = rms[objIndex].transform;
            Vector3 scale = t.localScale;
            if (scale.x >= 0 && scale.y >= 0 && scale.z >= 0) {
                rms[objIndex].bakedTransform = false;
                return;
            }

            Debug.LogError("------´ýÈ·¶¨Òì³£BakeTransform:" + rms[objIndex].renderer);

            // Duplicates mesh and bake rotation
            //Mesh fixedMesh = duplicateMesh ? Instantiate<Mesh>(rms[objIndex].originalMesh) : rms[objIndex].mesh;
            //Vector3[] vertices = fixedMesh.vertices;
            //for (int k = 0; k < vertices.Length; k++) {
            //    vertices[k] = t.TransformPoint(vertices[k]);
            //}
            //fixedMesh.vertices = vertices;
            //Vector3[] normals = fixedMesh.normals;
            //if (normals != null) {
            //    for (int k = 0; k < normals.Length; k++) {
            //        normals[k] = t.TransformVector(normals[k]).normalized;
            //    }
            //    fixedMesh.normals = normals;
            //}
            //fixedMesh.RecalculateBounds();
            //rms[objIndex].mesh = fixedMesh;
            //rms[objIndex].bakedTransform = true;
            //rms[objIndex].currentPosition = t.position;
            //rms[objIndex].currentRotation = t.eulerAngles;
            //rms[objIndex].currentScale = t.lossyScale;
        }

        public void UpdateMaterialProperties(bool forceNow = false) {
            if (forceNow || !Application.isPlaying) {
                UpdateMaterialPropertiesNow();
            } else {
                requireUpdateMaterial = true;
            }
        }


        void UpdateMaterialPropertiesNow() {

            if (rms == null)
                return;

            if (ignore) {
                _highlighted = false;
            }

            Color seeThroughTintColor = this.seeThroughTintColor;
            seeThroughTintColor.a = this.seeThroughTintAlpha;

            if (lastOutlineVisibility != outlineVisibility) {
                // change by scripting?
                if (glowQuality == QualityLevel.Highest && outlineQuality == QualityLevel.Highest) {
                    glowVisibility = outlineVisibility;
                }
                lastOutlineVisibility = outlineVisibility;
            }
            if (outlineWidth < 0) {
                outlineWidth = 0;
            }
            if (glowWidth < 0) {
                glowWidth = 0;
            }
            if (overlay < overlayMinIntensity) {
                overlay = overlayMinIntensity;
            }
            if (targetFXTransitionDuration <= 0) {
                targetFXTransitionDuration = 0.0001f;
            }
            if (targetFXStayDuration <= 0) {
                targetFXStayDuration = 0.0001f;
            }
            if (seeThroughBorderWidth < 0) {
                seeThroughBorderWidth = 0;
            }

            bool useSmoothGlow = glow > 0 && glowQuality == QualityLevel.Highest;
            if (useSmoothGlow) {
                CheckBlurCommandBuffer();
                fxMatComposeGlow.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                fxMatBlurGlow.SetFloat("_BlurScale", glowWidth / glowDownsampling);
                fxMatBlurGlow.SetFloat("_Speed", glowAnimationSpeed);
            }

            bool useSmoothOutline = outline > 0 && outlineQuality == QualityLevel.Highest;
            if (useSmoothOutline) {
                CheckBlurCommandBuffer();
                fxMatComposeOutline.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                fxMatBlurOutline.SetFloat("_BlurScale", outlineWidth / outlineDownsampling);
            }

            // Setup materials
            for (int k = 0; k < rmsCount; k++) {
                if (rms[k].mesh != null) {
                    // Outline
                    float scaledOutlineWidth = outlineQuality == QualityLevel.High ? 0f : outlineWidth / 100f;
                    Material matOutline = fxMatOutline;
                    matOutline.SetFloat("_OutlineWidth", scaledOutlineWidth);
                    matOutline.SetVector("_OutlineDirection", Vector3.zero);
                    matOutline.SetInt("_OutlineZTest", GetZTestValue(outlineVisibility));
                    matOutline.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                    matOutline.SetFloat("_ConstantWidth", constantWidth ? 1.0f : 0);

                    // Glow
                    Material matGlow = fxMatGlow;
                    matGlow.SetVector("_Glow2", new Vector3(outlineWidth / 100f, glowAnimationSpeed, glowDithering ? 0 : 1));
                    matGlow.SetInt("_GlowZTest", GetZTestValue(glowVisibility));
                    matGlow.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                    matGlow.SetFloat("_ConstantWidth", constantWidth ? 1.0f : 0);
                    if (glowBlendMode == GlowBlendMode.AlphaBlend) {
                        fxMatComposeGlow.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        fxMatComposeGlow.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    } else {
                        fxMatComposeGlow.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        fxMatComposeGlow.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    }

                    // Target
                    if (targetFX) {
                        if (targetFXTexture == null) {
                            targetFXTexture = Resources.Load<Texture2D>("HighlightPlus/target");
                        }
                        fxMatTarget.mainTexture = targetFXTexture;
                    }

                    // Mask, See-through & Overlay per submesh
                    for (int l = 0; l < rms[k].mesh.subMeshCount; l++) {
                        Renderer renderer = rms[k].renderer;
                        if (renderer == null)
                            continue;

                        Material mat = null;
                        if (renderer.sharedMaterials != null && l < renderer.sharedMaterials.Length) {
                            mat = renderer.sharedMaterials[l];
                        }
                        if (mat == null)
                            continue;

                        bool hasTexture = mat.HasProperty("_MainTex");
                        bool useAlphaTest = alphaCutOff > 0 && hasTexture;

                        // Mask
                        if (rms[k].fxMatMask != null && rms[k].fxMatMask.Length > l) {
                            Material fxMat = rms[k].fxMatMask[l];
                            if (fxMat != null) {
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                                if (depthClip) {
                                    fxMat.EnableKeyword(SKW_DEPTHCLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_DEPTHCLIP);
                                }
                                fxMat.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                            }
                        }

                        // See-through inner
                        if (rms[k].fxMatSeeThroughInner != null && rms[k].fxMatSeeThroughInner.Length > l) {
                            Material fxMat = rms[k].fxMatSeeThroughInner[l];
                            if (fxMat != null) {
                                fxMat.SetFloat("_SeeThrough", seeThroughIntensity);
                                fxMat.SetFloat("_SeeThroughNoise", seeThroughNoise);
                                fxMat.SetColor("_SeeThroughTintColor", seeThroughTintColor);
                                fxMat.SetFloat("_SeeThroughBorderWidth", (seeThroughBorder * seeThroughBorderWidth) > 0 ? seeThroughBorderWidth / 100f : 0);
                                fxMat.SetFloat("_SeeThroughBorderConstantWidth", constantWidth ? 1.0f : 0);

                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                            }
                        }

                        // See-through border
                        if (rms[k].fxMatSeeThroughBorder != null && rms[k].fxMatSeeThroughBorder.Length > l) {
                            Material fxMat = rms[k].fxMatSeeThroughBorder[l];
                            if (fxMat != null) {
                                fxMat.SetColor("_SeeThroughBorderColor", new Color(seeThroughBorderColor.r, seeThroughBorderColor.g, seeThroughBorderColor.b, seeThroughBorder));
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                            }
                        }

                        // Overlay
                        if (rms[k].fxMatOverlay != null && rms[k].fxMatOverlay.Length > l) {
                            Material fxMat = rms[k].fxMatOverlay[l];
                            if (fxMat != null) {
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                if (mat.HasProperty("_Color")) {
                                    fxMat.SetColor("_OverlayBackColor", mat.GetColor("_Color"));
                                }
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                            }
                        }

                        // Inner Glow
                        if (rms[k].fxMatInnerGlow != null && rms[k].fxMatInnerGlow.Length > l) {
                            Material fxMat = rms[k].fxMatInnerGlow[l];
                            if (fxMat != null) {
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                fxMat.SetFloat("_Width", innerGlowWidth);
                                fxMat.SetInt("_InnerGlowZTest", GetZTestValue(innerGlowVisibility));
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                            }
                        }

                        // Solid Color for smooth glow
                        if (rms[k].fxMatSolidColor != null && rms[k].fxMatSolidColor.Length > l) {
                            Material fxMat = rms[k].fxMatSolidColor[l];
                            if (fxMat != null) {
                                fxMat.color = glowHQColor;
                                fxMat.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                                fxMat.SetInt("_ZTest", GetZTestValue(useSmoothGlow ? glowVisibility : outlineVisibility));
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                                if (depthClip) {
                                    fxMat.EnableKeyword(SKW_DEPTHCLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_DEPTHCLIP);
                                }
                            }
                        }
                    }
                }
            }
        }

        int GetZTestValue(Visibility param) {
            switch (param) {
                case Visibility.AlwaysOnTop: return (int)UnityEngine.Rendering.CompareFunction.Always;
                case Visibility.OnlyWhenOccluded: return (int)UnityEngine.Rendering.CompareFunction.Greater;
                default:
                    return (int)UnityEngine.Rendering.CompareFunction.LessEqual;
            }
        }

        void BuildQuad() {
            quadMesh = new Mesh();

            // Setup vertices
            Vector3[] newVertices = new Vector3[4];
            float halfHeight = 0.5f;
            float halfWidth = 0.5f;
            newVertices[0] = new Vector3(-halfWidth, -halfHeight, 0);
            newVertices[1] = new Vector3(-halfWidth, halfHeight, 0);
            newVertices[2] = new Vector3(halfWidth, -halfHeight, 0);
            newVertices[3] = new Vector3(halfWidth, halfHeight, 0);

            // Setup UVs
            Vector2[] newUVs = new Vector2[newVertices.Length];
            newUVs[0] = new Vector2(0, 0);
            newUVs[1] = new Vector2(0, 1);
            newUVs[2] = new Vector2(1, 0);
            newUVs[3] = new Vector2(1, 1);

            // Setup triangles
            int[] newTriangles = new int[] { 0, 1, 2, 3, 2, 1 };

            // Setup normals
            Vector3[] newNormals = new Vector3[newVertices.Length];
            for (int i = 0; i < newNormals.Length; i++) {
                newNormals[i] = Vector3.forward;
            }

            // Create quad
            quadMesh.vertices = newVertices;
            quadMesh.uv = newUVs;
            quadMesh.triangles = newTriangles;
            quadMesh.normals = newNormals;

            quadMesh.RecalculateBounds();
        }


        bool CheckOcclusion(Camera cam) {

            if (Time.time - occlusionCheckLastTime < seeThroughOccluderCheckInterval) return lastOcclusionTestResult;
            occlusionCheckLastTime = Time.time;

            if (rms.Length == 0 || rms[0].renderer == null) return false;

            Vector3 camPos = cam.transform.position;

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
            lastOcclusionTestResult = Physics.BoxCast(pos - offset, bounds.extents * seeThroughOccluderThreshold, offset.normalized, Quaternion.identity, maxDistance, seeThroughOccluderMask);
            return lastOcclusionTestResult;
        }

        #region Normals handling

        static List<Vector3> vertices;
        static List<Vector3> normals;
        static Vector3[] newNormals;
        static int[] matches;
        static Dictionary<Vector3, int> vv = new Dictionary<Vector3, int>();
        static Dictionary<Mesh, Mesh> smoothMeshes = new Dictionary<Mesh, Mesh>();

        void AverageNormals(int objIndex) {
            if (rms == null || objIndex >= rms.Length) return;
            Mesh mesh = rms[objIndex].mesh;

            Mesh newMesh;
            if (!smoothMeshes.TryGetValue(mesh, out newMesh)) {
                if (!mesh.isReadable) return;
                if (normals == null) {
                    normals = new List<Vector3>();
                } else {
                    normals.Clear();
                }
                mesh.GetNormals(normals);
                int normalsCount = normals.Count;
                if (normalsCount == 0)
                    return;
                if (vertices == null) {
                    vertices = new List<Vector3>();
                } else {
                    vertices.Clear();
                }
                mesh.GetVertices(vertices);
                int vertexCount = vertices.Count;
                if (normalsCount < vertexCount) {
                    vertexCount = normalsCount;
                }
                if (newNormals == null || newNormals.Length < vertexCount) {
                    newNormals = new Vector3[vertexCount];
                } else {
                    Vector3 zero = Vector3.zero;
                    for (int k = 0; k < vertexCount; k++) {
                        newNormals[k] = zero;
                    }
                }
                if (matches == null || matches.Length < vertexCount) {
                    matches = new int[vertexCount];
                }
                // Locate overlapping vertices
                vv.Clear();
                for (int k = 0; k < vertexCount; k++) {
                    int i;
                    Vector3 v = vertices[k];
                    if (!vv.TryGetValue(v, out i)) {
                        vv[v] = i = k;
                    }
                    matches[k] = i;
                }
                // Average normals
                for (int k = 0; k < vertexCount; k++) {
                    int match = matches[k];
                    newNormals[match] += normals[k];
                }
                for (int k = 0; k < vertexCount; k++) {
                    int match = matches[k];
                    normals[k] = newNormals[match].normalized;
                }
                // Reassign normals
                newMesh = Instantiate<Mesh>(mesh);
                newMesh.hideFlags = HideFlags.DontSave;
                newMesh.SetNormals(normals);
                smoothMeshes[mesh] = newMesh;
            }
            rms[objIndex].mesh = newMesh;
        }

        #endregion

    }
}
