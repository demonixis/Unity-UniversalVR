using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    public class MotionController : MonoBehaviour
    {
        private Transform _transform = null;

        [SerializeField]
        private bool _leftHand = true;

        private void Start()
        {
            _transform = GetComponent<Transform>();
        }

        private void Update()
        {
            _transform.localPosition = InputTracking.GetLocalPosition(_leftHand ? VRNode.LeftHand : VRNode.RightHand);
            _transform.localRotation = InputTracking.GetLocalRotation(_leftHand ? VRNode.LeftHand : VRNode.RightHand);
        }
    }
}