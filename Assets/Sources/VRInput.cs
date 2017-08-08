using System.Collections;
using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    public enum VRButton
    {
        Menu,
        Button1,
        Button2,
        Button3,
        Button4,
        Thumbstick,
        ThumbstickTouch,
        Trigger,
        Grip,
        ThumbstickUp,
        ThumbstickDown,
        ThumbstickLeft,
        ThumbstickRight
    }

    public enum VRAxis
    {
        Trigger,
        Grip,
        ThumbstickX,
        ThumbstickY,
        ThumbstickTouchX,
        ThumbstickTouchY
    }

    public enum VRAxis2D
    {
        Thumbstick,
        ThumbstickTouch
    }

    public class VRInput : MonoBehaviour
    {
        private static VRInput instance = null;
        private Vector2 _tmp = Vector2.zero;
        private bool[] _axisStates = null;
        private VRButton[] _buttons = null;
        private bool _running = true;

        [SerializeField]
        private float _deadZone = 0.1f;

        public static VRInput Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("VRInput");
                    instance = go.AddComponent<VRInput>();
                }

                return instance;
            }
        }

        public bool IsConnected
        {
            get
            {
                if (VRSettings.loadedDeviceName == "Oculus")
                {
                    var joysticks = Input.GetJoystickNames();
                    foreach (var joystick in joysticks)
                        if (joystick.Contains("Oculus"))
                            return true;
                }
                else if (VRSettings.loadedDeviceName == "OpenVR")
                    return true;

                return false;
            }
        }

        public float DeadZone
        {
            get { return _deadZone; }
            set
            {
                _deadZone = value;

                if (_deadZone < 0)
                    _deadZone = 0.0f;
                else if (_deadZone >= 1.0f)
                    _deadZone = 0.9f;
            }
        }

        public void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }

            _buttons = new VRButton[]
            {
                VRButton.Grip, VRButton.Trigger,
                VRButton.ThumbstickUp, VRButton.ThumbstickDown,
                VRButton.ThumbstickLeft, VRButton.ThumbstickRight
            };

            _axisStates = new bool[_buttons.Length * 2];

            StartCoroutine(UpdateAxisToButton());
        }

        private void OnDestroy()
        {
            _running = false;
        }

        private IEnumerator UpdateAxisToButton()
        {
            var endOfFrame = new WaitForEndOfFrame();
            var index = 0;

            while (_running)
            {
                index = 0;

                for (var i = 0; i < _buttons.Length; i++)
                {
                    _axisStates[index] = GetButton(_buttons[i], true);
                    _axisStates[index + 1] = GetButton(_buttons[i], false);
                    index += 2;
                }

                yield return endOfFrame;
            }
        }

        /// <summary>
        /// Gets the position of a specific node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual Vector3 GetLocalPosition(VRNode node)
        {
            return InputTracking.GetLocalPosition(node);
        }

        /// <summary>
        /// Gets the rotation of a specific node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual Quaternion GetLocalRotation(VRNode node)
        {
            return InputTracking.GetLocalRotation(node);
        }

        /// <summary>
        /// Indicates whether a button is pressed.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public bool GetButton(VRButton button, bool left)
        {
            if (button == VRButton.Menu)
            {
                if (VRSettings.loadedDeviceName == "OpenVR")
                    return Input.GetButton(left ? "Button 2" : "Button 0");

                return Input.GetButton("Button 7");
            }

            else if (button == VRButton.Button1)
                return Input.GetButton("Button 0");

            else if (button == VRButton.Button2)
                return Input.GetButton("Button 1");

            else if (button == VRButton.Button3)
                return Input.GetButton("Button 2");

            else if (button == VRButton.Button4)
                return Input.GetButton("Button 3");

            else if (button == VRButton.Thumbstick)
                return Input.GetButton(left ? "Button 8" : "Button 9");

            else if (button == VRButton.ThumbstickTouch)
                return Input.GetButton(left ? "Button 16" : "17");

            else if (button == VRButton.Trigger)
                return GetAxis(VRAxis.Trigger, left) > _deadZone;

            else if (button == VRButton.Grip)
                return GetAxis(VRAxis.Grip, left) > _deadZone;

            else if (button == VRButton.ThumbstickUp)
                return GetAxis(VRAxis.ThumbstickY, left) > _deadZone;

            else if (button == VRButton.ThumbstickDown)
                return GetAxis(VRAxis.ThumbstickY, left) < _deadZone * -1.0f;

            else if (button == VRButton.ThumbstickLeft)
                return GetAxis(VRAxis.ThumbstickX, left) < _deadZone * -1.0f;

            else if (button == VRButton.ThumbstickRight)
                return GetAxis(VRAxis.ThumbstickX, left) > _deadZone;

            return false;
        }

        /// <summary>
        /// Indicates whether a button was pressed.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public bool GetButtonDown(VRButton button, bool left)
        {
            if (button == VRButton.Menu)
            {
                if (VRSettings.loadedDeviceName == "OpenVR")
                    return Input.GetButtonDown(left ? "Button 2" : "Button 0");

                return Input.GetButtonDown("Button 7");
            }

            else if (button == VRButton.Button1)
                return Input.GetButtonDown("Button 0");

            else if (button == VRButton.Button2)
                return Input.GetButtonDown("Button 1");

            else if (button == VRButton.Button3)
                return Input.GetButtonDown("Button 2");

            else if (button == VRButton.Button4)
                return Input.GetButtonDown("Button 3");

            else if (button == VRButton.Thumbstick)
                return Input.GetButtonDown(left ? "Button 8" : "Button 9");

            else if (button == VRButton.ThumbstickTouch)
                return Input.GetButtonDown(left ? "Button 16" : "Button 17");

            // Simulate other buttons using previous states.
            var index = 0;
            for (var i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] != button)
                {
                    index += 2;
                    continue;
                }

                var prev = _axisStates[left ? index : index + 1];
                var now = GetButton(_buttons[i], left);

                return now && !prev;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether a button was released.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public bool GetButtonUp(VRButton button, bool left)
        {
            if (button == VRButton.Menu)
            {
                if (VRSettings.loadedDeviceName == "OpenVR")
                    return Input.GetButtonUp(left ? "Button 2" : "Button 0");

                return Input.GetButtonUp("Button 7");
            }

            else if (button == VRButton.Button1)
                return Input.GetButtonUp("Button 0");

            else if (button == VRButton.Button2)
                return Input.GetButtonUp("Button 1");

            else if (button == VRButton.Button3)
                return Input.GetButtonUp("Button 2");

            else if (button == VRButton.Button4)
                return Input.GetButtonUp("Button 3");

            else if (button == VRButton.Thumbstick)
                return Input.GetButtonUp(left ? "Button 8" : "Button 9");

            else if (button == VRButton.ThumbstickTouch)
                return Input.GetButtonUp(left ? "Button 16" : "Button 17");

            // Simulate other buttons using previous states.
            var index = 0;
            for (var i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] != button)
                {
                    index += 2;
                    continue;
                }

                var prev = _axisStates[left ? index : index + 1];
                var now = GetButton(_buttons[i], left);

                return !now && prev;
            }

            return false;
        }

        /// <summary>
        /// Gets an axis value.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns the axis value.</returns>
        public float GetAxis(VRAxis axis, bool left)
        {
            if (axis == VRAxis.Trigger)
                return Input.GetAxis(left ? "Axis 9" : "Axis 10");

            else if (axis == VRAxis.Grip)
                return Input.GetAxis(left ? "Axis 11" : "Axis 12");

            else if (axis == VRAxis.ThumbstickX)
                return Input.GetAxis(left ? "Axis 1" : "Axis 4");

            else if (axis == VRAxis.ThumbstickY)
                return Input.GetAxis(left ? "Axis 2" : "Axis 5");

            else if (GetButton(VRButton.ThumbstickTouch, left))
            {
                if (axis == VRAxis.ThumbstickTouchX)
                    return Input.GetAxis(left ? "Axis 1" : "Axis 4");

                else if (axis == VRAxis.ThumbstickTouchY)
                    return Input.GetAxis(left ? "Axis 2" : "Axis 5");
            }

            return 0.0f;
        }

        /// <summary>
        /// Gets two axis values.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns two axis values.</returns>
        public Vector2 GetAxis2D(VRAxis2D axis, bool left)
        {
            _tmp.x = Input.GetAxis(left ? "Axis 1" : "Axis 4");
            _tmp.y = Input.GetAxis(left ? "Axis 2" : "Axis 5");

            if (axis == VRAxis2D.Thumbstick)
                return _tmp;

            else if (GetButton(VRButton.ThumbstickTouch, left))
                return _tmp;

            return Vector2.zero;
        }
    }
}
