using Demonixis.Toolbox.VR;
using UnityEngine;

public class VRInputTester : MonoBehaviour
{
    private VRInput vrInput = null;

    public Transform cube1;
    public Transform cube2;

    private void Start()
    {
        vrInput = gameObject.AddComponent<VRInput>();
    }

    private void Update()
    {
        if (vrInput.GetButtonDown(VRButton.Trigger, true))
            Debug.Log("LT");

        if (vrInput.GetButtonDown(VRButton.Grip, true))
            Debug.Log("LG");

        if (vrInput.GetButtonDown(VRButton.ThumbstickDown, true))
            Debug.Log("Left Thumbstick Down");

        if (vrInput.GetButtonDown(VRButton.ThumbstickLeft, true))
            Debug.Log("Left Thumbstick Left");

        if (vrInput.GetButtonUp(VRButton.ThumbstickUp, true))
            Debug.Log("Left Thumbstick Up was Up");

        return;

        var v = vrInput.GetAxis2D(VRAxis2D.Thumbstick, true);
        cube1.Translate(v.x, 0, v.y);

        v = vrInput.GetAxis2D(VRAxis2D.Thumbstick, false);
        cube2.Translate(v.x, 0, v.y);

        if (vrInput.GetButtonDown(VRButton.Button1, true))
            Debug.Log("Button 1");

        if (vrInput.GetButtonDown(VRButton.Button2, true))
            Debug.Log("Button 2");

        if (vrInput.GetButtonDown(VRButton.Button3, false))
            Debug.Log("Button 3");

        if (vrInput.GetButtonDown(VRButton.Button4, false))
            Debug.Log("Button 4");

        if (vrInput.GetButtonDown(VRButton.Menu, false))
            Debug.Log("Menu");

        if (vrInput.GetButtonDown(VRButton.ThumbstickTouch, true))
            Debug.Log("Thumbstick Touch left");

        if (vrInput.GetButtonDown(VRButton.ThumbstickTouch, false))
            Debug.Log("Thumbstick Touch right");

        if (vrInput.GetAxis(VRAxis.Grip, true) > 0.2f)
            Debug.Log("Grip Left: " + vrInput.GetAxis(VRAxis.Grip, true));

        if (vrInput.GetAxis(VRAxis.Grip, false) > 0.2f)
            Debug.Log("Grip Right: " + vrInput.GetAxis(VRAxis.Grip, false));

        if (vrInput.GetAxis(VRAxis.Trigger, true) > 0.2f)
            Debug.Log("Trigger Left: " + vrInput.GetAxis(VRAxis.Trigger, true));

        if (vrInput.GetAxis(VRAxis.Trigger, false) > 0.2f)
            Debug.Log("Trigger Right: " + vrInput.GetAxis(VRAxis.Trigger, false));

        if (vrInput.GetButtonDown(VRButton.Thumbstick, true))
            Debug.Log("Thumbstick left");

        if (vrInput.GetButtonDown(VRButton.Thumbstick, false))
            Debug.Log("Thumbstick right");
    }
}
