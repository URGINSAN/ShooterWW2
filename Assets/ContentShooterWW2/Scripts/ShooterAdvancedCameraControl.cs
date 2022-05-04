using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ShooterAdvancedCameraControl : MonoBehaviour
{
    public float RotateSpeedX = 12;
    public float RotateSpeedY = 10;
    public Transform ParentCam;
    public Transform PlayerTransform;
    public bool OnRotateArea;
    public enum InputSource
    {
        GYROSCOPE,
        TOUCH
    }

    public float rotateSpeed = 90;
    [SerializeField] float smoothing = 2;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY = true;
    [SerializeField] float limitX = 70;
    [SerializeField] float XFactor = -30;
    public float SmoothDamping = 0.05f;
    public float JoyLerpedX;
    public float JoyLerpedY;
    static bool inputSourceLoaded = false;
    static InputSource inputSource = InputSource.GYROSCOPE;
    public static InputSource Source
    {
        get
        {
            if (!inputSourceLoaded)
            {
                inputSourceLoaded = true;
                InputSource defaultSource = SystemInfo.supportsGyroscope ? InputSource.GYROSCOPE : InputSource.TOUCH;
                inputSource = (InputSource)PlayerPrefs.GetInt("InputSource", (int)defaultSource);
            }
            return inputSource;
        }
        set
        {
            inputSourceLoaded = true;
            inputSource = value;
            PlayerPrefs.SetInt("InputSource", (int)value);
        }
    }

    public void SetInputMouse()
    {
        Source = InputSource.TOUCH;
    }

    public void SetInputGyro()
    {
        Source = InputSource.GYROSCOPE;
    }

    Vector3 lastMousePosition;

    Transform child;
    Transform lerper;
    Transform root;

    void Start()
    {
        Input.gyro.enabled = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;

        root = new GameObject("AdvancedCameraRoot").transform;
        child = new GameObject("Camera").transform;
        lerper = new GameObject("Interpolator").transform;
        root.SetParent(transform.parent);
        root.localPosition = transform.localPosition;
        root.localRotation = transform.localRotation;
        child.SetParent(root, false);
        lerper.SetParent(root, false);
        transform.SetParent(child, false);
        transform.localPosition = Vector3.zero;

        Source = InputSource.TOUCH;
    }

    void FixedUpdate()
    {
        switch (Source)
        {
            case InputSource.TOUCH:

#if !UNITY_EDITOR
                JoyLerpedX = Mathf.Lerp(JoyLerpedX, CrossPlatformInputManager.GetAxis("RightPadX"), SmoothDamping);
                JoyLerpedY = Mathf.Lerp(JoyLerpedY, CrossPlatformInputManager.GetAxis("RightPadY"), SmoothDamping);
                UpdateFromDelta(JoyLerpedX * RotateSpeedX, JoyLerpedY * RotateSpeedY);
                //UpdateFromDelta(CrossPlatformInputManager.GetAxis("RightPadX") * RotateSpeedX, CrossPlatformInputManager.GetAxis("RightPadY") * RotateSpeedY);14,8
#endif
#if UNITY_EDITOR

                UpdateFromDelta(Input.GetAxisRaw("Mouse X") * RotateSpeedX, Input.GetAxisRaw("Mouse Y") * RotateSpeedY);
#endif
                break;
/*            case InputSource.GYROSCOPE:
                PlayerTransform.localEulerAngles += new Vector3(0, -Input.gyro.rotationRateUnbiased.y, 0);

                lerper.rotation = Input.gyro.attitude;
                lerper.Rotate(0f, 0f, 180f, Space.Self);
                lerper.Rotate(90f, 180f, 0f, Space.World);
                lerper.localEulerAngles = new Vector3(lerper.transform.localEulerAngles.x + XFactor, 0, 0);

                child.rotation = Quaternion.Lerp(child.rotation, lerper.rotation, Mathf.Clamp01(smoothing * Time.deltaTime));
                break;*/
        }
    }

    void UpdateFromDelta(float deltaX, float deltaY)
    {
        if (invertY)
            deltaX = -deltaX;
        if (invertX)
            deltaY = -deltaY;
        deltaX /= Screen.width;
        deltaY /= Screen.height;
        deltaX *= rotateSpeed;
        deltaY *= rotateSpeed;
        Vector3 newAngles = child.localEulerAngles;
        newAngles.x += deltaY;
        while (newAngles.x > 180)
            newAngles.x -= 360;
        while (newAngles.x < -180)
            newAngles.x += 360;
        newAngles.x = Mathf.Clamp(newAngles.x, -limitX, limitX);

        child.localEulerAngles = newAngles;
        PlayerTransform.localEulerAngles += new Vector3(0, deltaX, 0);
    }

    public void CheckOnRotateArea(bool State)
    {
        OnRotateArea = State;
    }
}