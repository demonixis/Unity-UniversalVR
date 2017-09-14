using UnityEngine;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.VR;
#endif

namespace Demonixis.Toolbox.XR
{
    public sealed class GoogleVRDevice : UnityXRDevice
    {
#region Public Fields

        [SerializeField]
        private Vector3 _headPosition = new Vector3(0.0f, 1.8f, 0.0f);

        public override bool IsAvailable
        {
            get
            {
#if UNITY_ANDROID || UNITY_IOS
                return !UnityEngine.XR.XRDevice.isPresent && UnityEngine.XR.XRDevice.model == "cardboard" || UnityEngine.XR.XRDevice.model == "daydream";
#else
                return false;
#endif
            }
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.GoogleVR; }
        }

        public override Vector3 HeadPosition
        {
            get { return _headPosition; }
        }

#endregion

        public override void SetActive(bool isEnabled)
        {
            if (!IsAvailable)
                return;

#if UNITY_ANDROID || UNITY_IOS
            UnityEngine.XR.XRSettings.enabled = isEnabled;
#endif
        }

        public override void Dispose()
        {
            Destroy(this);
        }
    }
}