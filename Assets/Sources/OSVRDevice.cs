/// OSVRDevice
/// Version: 4

using OSVR.Unity;
using UnityEngine;
using System.Collections;
using UnityEngine.XR;

namespace Demonixis.Toolbox.XR
{
    /// <summary>
    /// OSVRDevice - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OSVRDevice : XRDeviceBase
    {
#if UNITY_STANDALONE
        private const string AppID = "net.demonixis.GunSpinningVR";
        private const bool AutoStartServer = false;
        private static ClientKit _clientKit = null;
        private DisplayController _displayController = null;
        private OsvrUnityNativeVR _unityNativeVR = null;
        private GameObject _dummyCamera = null;

        public override bool IsAvailable
        {
            get
            {
                var clientKit = GetClientKit();
                return clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();
            }
        }

        public override VRDeviceType VRDeviceType { get { return VRDeviceType.OSVR; } }

        public bool IsLegacyIntegration { get { return _unityNativeVR != null; } }

        public override float RenderScale
        {
            get
            {
                if (_unityNativeVR)
                    return XRSettings.eyeTextureResolutionScale;

                return 1.0f;
            }
            set { }
        }

        public override int EyeTextureWidth
        {
            get
            {
                if (_unityNativeVR)
                    return XRSettings.eyeTextureWidth;

                return Screen.width;
            }
        }

        public override int EyeTextureHeight
        {
            get
            {
                if (_unityNativeVR)
                    return XRSettings.eyeTextureHeight;

                return Screen.height;
            }
        }

        public override void Dispose()
        {
            Destroy(_unityNativeVR);

            if (_displayController != null)
                SetActive(false);
        }

        public static ClientKit GetClientKit()
        {
            if (_clientKit == null)
            {
                var go = new GameObject("ClientKit");
                go.SetActive(false);

                _clientKit = go.AddComponent<ClientKit>();
                _clientKit.AppID = AppID;
#if UNITY_STANDALONE_WIN
                _clientKit.autoStartServer = false;
#endif

                go.SetActive(true);
            }

            return _clientKit;
        }

        public override void Recenter()
        {
            var clientKit = GetClientKit();

            if (clientKit == null)
                return;

            if (_displayController != null && _displayController.UseRenderManager)
                _displayController.RenderManager.SetRoomRotationUsingHead();
            else
                clientKit.context.SetRoomRotationUsingHead();
        }

        public override void SetActive(bool isEnabled)
        {
            var clientKit = GetClientKit();
            var camera = Camera.main;

            if (clientKit == null || camera == null)
                return;

            if (clientKit.context == null || !clientKit.context.CheckStatus())
                return;

            var setupLegacy = System.Environment.CommandLine.Contains("--osvr-legacy");

#if !UNITY_STANDAONE_WIN
            setupLegacy = true;
#endif

            if (setupLegacy)
                SetupLegacySupport(camera, isEnabled);
            else
                SetupUnityXRSupport(camera, isEnabled);
        }

        private void SetupUnityXRSupport(Camera camera, bool isEnabled)
        {
            if (isEnabled && _unityNativeVR == null)
            {
                // OSVR doesn't support deferred renderer for now.
                if (camera.renderingPath != RenderingPath.Forward)
                    QualitySettings.antiAliasing = 0;

                _unityNativeVR = camera.gameObject.AddComponent<OsvrUnityNativeVR>();

                // Recenter and mirror mode.
                StartCoroutine(FinishSetup());
            }
            else if (_unityNativeVR != null)
            {
                Destroy<OsvrMirrorDisplay>();
                Destroy<OsvrUnityNativeVR>();
                Destroy<OsvrRenderManager>(false);
                Destroy(_dummyCamera);
                _unityNativeVR = null;
            }
        }

        private void SetupLegacySupport(Camera camera, bool isEnabled)
        {
            if (isEnabled && _displayController == null)
            {
                // OSVR doesn't support deferred renderer for now.
                if (camera.renderingPath != RenderingPath.Forward)
                    QualitySettings.antiAliasing = 0;

                _displayController = camera.transform.parent.gameObject.AddComponent<DisplayController>();
                camera.gameObject.AddComponent<VRViewer>();

                // Recenter and mirror mode.
                StartCoroutine(FinishSetup());
            }
            else if (_displayController != null)
            {
                Destroy(_displayController);
                Destroy<OsvrMirrorDisplay>();
                Destroy<VREye>();
                Destroy<VRSurface>();
                Destroy<VRViewer>();
                Destroy<OsvrRenderManager>(false);
                _displayController = null;
            }
        }

        private IEnumerator FinishSetup()
        {
            yield return new WaitForEndOfFrame();

            var osvrMirror = (OsvrMirrorDisplay)null;

            if ((_displayController != null && _displayController.UseRenderManager) || _unityNativeVR != null)
                osvrMirror = gameObject.AddComponent<OsvrMirrorDisplay>();

            if (_unityNativeVR != null && osvrMirror != null)
            {
                osvrMirror.MirrorCamera = Camera.main;

                _dummyCamera = new GameObject("OsvrDummyCamera");

                var cam = _dummyCamera.AddComponent<Camera>();
                cam.clearFlags = CameraClearFlags.Nothing;
                cam.stereoTargetEye = StereoTargetEyeMask.None;
                cam.useOcclusionCulling = false;
                cam.allowMSAA = false;
                cam.allowHDR = false;
                cam.cullingMask = 0;
            }

            yield return new WaitForEndOfFrame();

            var manager = GetComponent<XRManager>();
            if (manager)
                manager.RecenterAndFixOffset();
        }
#else
        public override bool IsAvailable { get { return false; } }
        public override VRDeviceType VRDeviceType { get { return VRDeviceType.None; } }
        public override float RenderScale { get; set; } 
        public override int EyeTextureWidth { get { return 0; } }
        public override int EyeTextureHeight { get { return 0; } }
        public override void Recenter() { }
        public override void SetActive(bool isEnabled) { }
#endif
    }
}