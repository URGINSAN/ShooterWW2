using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraTouchControl : MonoBehaviour
{
    float controlSpeed;
    public float sensitivity = 15;
    public int maxSpeed = 250;
    public float LimitX = 80;
    private float RotX;
    public Transform Camera;
    public Transform Player;

    void Update()
    {
        if (Input.touchSupported)
        {
            Touch touch = Input.GetTouch(0);
            controlSpeed = touch.deltaPosition.magnitude * sensitivity;
            //controlSpeed = Input.GetTouch(0).deltaPosition.magnitude * sensitivity;
            controlSpeed = controlSpeed > maxSpeed ? maxSpeed : controlSpeed;
        }
        else
            controlSpeed = 3 * sensitivity;
        controlSpeed = controlSpeed * Time.deltaTime;

        //RotX = ;
        //RotX = Mathf.Clamp(-controlSpeed * CrossPlatformInputManager.GetAxis("RightPadY"), -LimitX, LimitX);
        //transform.Rotate(-controlSpeed * CrossPlatformInputManager.GetAxis("RightPadY"), controlSpeed * CrossPlatformInputManager.GetAxis("RightPadX"), 0);
        Camera.Rotate(-controlSpeed * CrossPlatformInputManager.GetAxis("RightPadY"), 0, 0);
        //Player.Rotate(0, controlSpeed * CrossPlatformInputManager.GetAxis("RightPadX"), 0);
    }

    private void LateUpdate()
    {
        //Camera.localEulerAngles = Vector3.ClampMagnitude()
    }
}