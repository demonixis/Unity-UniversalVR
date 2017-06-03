#if UNITY_WSA_10_0
using UnityEngine.VR;
#endif

namespace Demonixis.Toolbox.VR
{
    public class WindowsMRDevice: UnityVRDevice
    {
        public override bool IsAvailable
        {
#if UNITY_WSA_10_0
            get { return VRDevice.isPresent && VRSettings.enabled; }
#else
            get { return false; }
#endif
        }
    }
}
