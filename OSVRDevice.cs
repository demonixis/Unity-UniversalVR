/// OSVRDevice
/// Version: 3

using OSVR.Unity;
using UnityEngine;
using System.Collections;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OSVRDevice - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OSVRDevice : VRDeviceBase
    {
#if UNITY_STANDALONE
        private const string AppID = "net.demonixis.GunSpinningVR";
        private const bool AutoStartServer = false;
        private static ClientKit _clientKit = null;
        private DisplayController displayController = null;
#endif
        [SerializeField]
        private string[] _cameraComponentsToMove = null;
        [SerializeField]
        private string[] _cameraComponentsToRemove = null;

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
                UnityEngine.VR.VRSettings.enabled = false;

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

                    // Removed unwanted components.
                    if (_cameraComponentsToRemove != null && _cameraComponentsToRemove.Length > 0)
                        for (var i = 0; i < _cameraComponentsToRemove.Length; i++)
                            Destroy(camera.GetComponent(_cameraComponentsToRemove[i]));

                    // Move wanted components to eyes (effects)
                    if (_cameraComponentsToMove != null && _cameraComponentsToMove.Length > 0)
                        StartCoroutine(CopyComponentsToEyes());

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

        private IEnumerator CopyComponentsToEyes()
        {
            yield return new WaitForEndOfFrame();

            var component = (Component)null;
            var camera = Camera.main.transform;
            var eyes = camera.parent.GetComponentsInChildren<VRSurface>(true);

            for (var i = 0; i < _cameraComponentsToMove.Length; i++)
            {
                component = camera.GetComponent(_cameraComponentsToMove[i]);

                if (component == null)
                    continue;

                for (int j = 0, eyeCount = eyes.Length; j < eyeCount; j++)
                {
                    CopyComponent(component, eyes[j].gameObject);
                    Destroy(camera.GetComponent(_cameraComponentsToMove[i]));
                }
            }
        }

        private IEnumerator FinishSetup()
        {
            yield return new WaitForEndOfFrame();

            if (displayController.UseRenderManager)
                gameObject.AddComponent<OsvrMirrorDisplay>();

            yield return new WaitForEndOfFrame();

            var manager = GetComponent<VRManager>();
            if (manager)
                manager.RecenterAndFixOffset();
        }
#endif
    }
}