using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterMovement : MonoBehaviour {
    public Transform Player;
    public Transform PlayerOrigin;
    public float speed = 5;
    public float jumpForce = 5;
    private bool Jumped;
    private bool CanGround = true;
    public float CanMoveDelay = 0.5f;
    private bool CanMoveSound = true;
    CharacterController Controller;
    CharacterWeaponsController WeaponsController;
    Vector3 moveVector;
    [Header("Наклоны")]
    public Transform PlayerPitch;
    public bool TurnLeft;
    public bool TurnRight;
    public float MaxTurnLeft = 30;
    public float MaxTurnRight = -30;
    [Header("Аудио")]
    public AudioSource PlayerSFX;
    public AudioClip[] JumpSound;
    public AudioClip[] MoveRockSound;
    [Header("Анимация")]
    public string CurrentAnim;
    public Animator Anim;
    public string WalkBoolName;

    private float MoveX;
    private float MoveY;

	void Start () {
        Controller = GetComponent<CharacterController>();
        WeaponsController = GetComponent<CharacterWeaponsController>();
    }

	void Update () {
        if (Controller.isGrounded)
        {
            if (CanGround)
            {
                if (PlayerSFX.clip != JumpSound[1])
                {
                    PlayerSFX.clip = JumpSound[1];
                    PlayerSFX.Play();
                }
                Anim.Play("WeaponsCameraJumpOut");
                if (WeaponsController.Scoped)
                    WeaponsController.ScopeState();
                CanGround = false;
            }
#if !UNITY_EDITOR
            MoveX = CrossPlatformInputManager.GetAxis("LeftPadX");
            MoveY = CrossPlatformInputManager.GetAxis("LeftPadY");
#endif
#if UNITY_EDITOR
            MoveX = Input.GetAxis("LeftPadX");
            MoveY = Input.GetAxis("LeftPadY");
#endif
            moveVector = Player.forward * MoveY * Time.deltaTime * speed * 50 + Player.right * MoveX * Time.deltaTime * speed * 50;
            if (MoveX != 0 || MoveY != 0)
            {
                WalkingState(true);
                if (WeaponsController.CurrentWeaponController != null)
                    WeaponsController.CurrentWeaponController.Accuracy = Mathf.Lerp(WeaponsController.CurrentWeaponController.Accuracy, WeaponsController.CurrentWeaponController.LowAccuracy, 10 * Time.deltaTime);

                if (CanMoveSound)
                {
                    PlayerSFX.PlayOneShot(MoveRockSound[Random.RandomRange(0, MoveRockSound.Length)]);
                    StartCoroutine(TakeMoveSoundDelay());
                    CanMoveSound = false;
                }
            }

            if (MoveX == 0 && MoveY == 0)
            {

                WalkingState(false);
                if (WeaponsController.CurrentWeaponController != null)
                    WeaponsController.CurrentWeaponController.Accuracy = Mathf.Lerp(WeaponsController.CurrentWeaponController.Accuracy, WeaponsController.CurrentWeaponController.HighAccuracy, 10 * Time.deltaTime);
            }
        }
        else
        {
            PlayerSFX.clip = null;
            WalkingState(false);
            CanGround = true;
        }

        if (Jumped)
        {
            moveVector.y = jumpForce;
            PlayerSFX.PlayOneShot(JumpSound[0]);
            Anim.CrossFade("WeaponsCameraJumpIn", 0.1f);
            Jumped = false;
        }

        moveVector.y -= 20 * Time.deltaTime;
        Controller.Move(moveVector * Time.deltaTime);

        if (TurnLeft)
        {
            PlayerPitch.localEulerAngles = new Vector3(PlayerPitch.localEulerAngles.x, PlayerPitch.localEulerAngles.y, Mathf.Lerp(PlayerPitch.localEulerAngles.z, MaxTurnLeft, 5 * Time.deltaTime));
        }
        if (TurnRight)
        {
            PlayerPitch.localEulerAngles = new Vector3(PlayerPitch.localEulerAngles.x, PlayerPitch.localEulerAngles.y, Mathf.Lerp(PlayerPitch.localEulerAngles.z, MaxTurnRight, 5 * Time.deltaTime));
        }

        if (!TurnLeft && !TurnRight)
        {
            PlayerPitch.localEulerAngles = new Vector3(PlayerPitch.localEulerAngles.x, PlayerPitch.localEulerAngles.y, Mathf.Lerp(PlayerPitch.localEulerAngles.z, 90, 5 * Time.deltaTime));
        }
    }

    void WalkingState(bool State)
    {
        Anim.SetBool(WalkBoolName, State);
    }

    public void Jump()
    {
        if (Controller.isGrounded)
        {
            Jumped = true;
            if (WeaponsController.Scoped)
                WeaponsController.ScopeState();
        }
    }

    IEnumerator TakeMoveSoundDelay()
    {
        yield return new WaitForSeconds(CanMoveDelay);
        CanMoveSound = true;
    }

    public void TurningLeftDown()
    {
        TurnLeft = true;
        TurnRight = false;
    }

    public void TurningRightDown()
    {
        TurnLeft = false;
        TurnRight = true;
    }

    public void TurningLeftUp()
    {
        TurnLeft = false;
        TurnRight = false;
    }

    public void TurningRightUp()
    {
        TurnLeft = false;
        TurnRight = false;
    }
}