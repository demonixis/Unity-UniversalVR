/// UnityVRDevice
/// Last Modified Date: 01/07/2017

using UnityEngine;
using UnityEngine.XR;

namespace Demonixis.Toolbox.XR
{
    /// <summary>
    /// The UnityVRDevice is an abstract device that uses the UnityEngine.VR implementation.
    /// </summary>
    public class UnityXRDevice : XRDeviceBase
    {
        #region Public Fields

        public override float RenderScale
        {
            get { return XRSettings.eyeTextureResolutionScale; }
            set { XRSettings.eyeTextureResolutionScale = value; }
        }

        public override int EyeTextureWidth { get { return XRSettings.eyeTextureWidth; } }

        public override int EyeTextureHeight { get { return XRSettings.eyeTextureHeight; } }

        public override VRDeviceType VRDeviceType { get { return VRDeviceType.UnityVR; } }

        public override Vector3 HeadPosition { get { return InputTracking.GetLocalPosition(XRNode.Head); } }

        public override bool IsAvailable { get { return XRDevice.isPresent && XRSettings.enabled; } }

        #endregion

        public override void Recenter()
        {
            InputTracking.Recenter();
        }

        public override void SetActive(bool active)
        {
        }
    }
}
