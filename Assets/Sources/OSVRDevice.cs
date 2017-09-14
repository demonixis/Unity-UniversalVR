/// OSVRDevice
/// Version: 3

using OSVR.Unity;
using UnityEngine;
using System.Collections;

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
        private DisplayController displayController = null;
#endif

        public override bool IsAvailable
        {
            get
            {
#if UNITY_STANDALONE
                var clientKit = GetClientKit();
                return clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();
#else
                return false;
#endif
            }
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.OSVR; }
        }

        public override float RenderScale
        {
            get { return 1.0f; }
            set { }
        }

        public override int EyeTextureWidth
        {
            get { return Screen.width; }
        }

        public override int EyeTextureHeight
        {
            get { return Screen.height; }
        }

        public override void Dispose()
        {
#if UNITY_STANDALONE
            if (displayController != null)
                SetActive(false);
#endif
            Destroy(this);
        }

        public static ClientKit GetClientKit()
        {
#if UNITY_STANDALONE
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
#else
            return null;
#endif
        }

        public override void Recenter()
        {
#if UNITY_STANDALONE
            var clientKit = GetClientKit();

            if (clientKit != null)
            {
                if (displayController != null && displayController.UseRenderManager)
                    displayController.RenderManager.SetRoomRotationUsingHead();
                else
                    clientKit.context.SetRoomRotationUsingHead();
            }
#endif
        }
        
        public override void SetActive(bool isEnabled)
        {
#if !UNITY_STANDALONE
        }
#else    
            if (isEnabled)
                UnityEngine.XR.XRSettings.enabled = false;

            var clientKit = GetClientKit();
            var camera = Camera.main;

            if (camera != null && clientKit != null && clientKit.context != null && clientKit.context.CheckStatus())
            {
                if (isEnabled && displayController == null)
                {
                    // OSVR doesn't support deferred renderer for now.
                    if (camera.renderingPath != RenderingPath.Forward)
                        QualitySettings.antiAliasing = 0;

                    displayController = camera.transform.parent.gameObject.AddComponent<DisplayController>();
                    camera.gameObject.AddComponent<VRViewer>();

                    // Recenter and mirror mode.
                    StartCoroutine(FinishSetup());
                }
                else if (displayController != null)
                {
                    Destroy(displayController);
                    Destroy<OsvrMirrorDisplay>();
                    Destroy<VREye>();
                    Destroy<VRSurface>();
                    Destroy<VRViewer>();
                    Destroy<OsvrRenderManager>(false);
                    displayController = null;
                }
            }
        }

        private IEnumerator FinishSetup()
        {
            yield return new WaitForEndOfFrame();

            if (displayController.UseRenderManager)
                gameObject.AddComponent<OsvrMirrorDisplay>();

            yield return new WaitForEndOfFrame();

            var manager = GetComponent<XRManager>();
            if (manager)
                manager.RecenterAndFixOffset();
        }
#endif
    }
}