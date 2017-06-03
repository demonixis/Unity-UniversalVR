using Demonixis.Toolbox.VR;
using UnityEngine;

public class VRInputTester : MonoBehaviour
{
    [SerializeField]
    private Transform _motionControllerLeft = null;
    [SerializeField]
    private Transform _motionControllerRight = null;
    [SerializeField]
    private GameObject _projectil = null;

    private void Update()
    {
        if (VRInput.Instance.GetButtonDown(VRButton.Trigger, true))
            Shoot(true);
        else if (VRInput.Instance.GetButtonDown(VRButton.Trigger, false))
            Shoot(false);
    }

    private void Shoot(bool left)
    {
        var target = left ? _motionControllerLeft : _motionControllerRight;
        var sphere = Instantiate(_projectil);
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sphere.transform.position = target.position;
        sphere.transform.rotation = target.rotation;
        sphere.transform.Translate(0, 0, 1);

        var rb = sphere.GetComponent<Rigidbody>();
        rb.AddRelativeForce(0, 0, 25, ForceMode.Impulse);
    }
}
