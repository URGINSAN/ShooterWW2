using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterWeaponsController : MonoBehaviour
{
    private CharacterMovement CharMovement;
    private float CharMovementSpeed;
    public bool Scoped;
    private bool Shooting;
    public GameObject Crosshair;
    public CanvasGroup ExtCrosshair;
    public WeaponController CurrentWeaponController;
    private ShooterAdvancedCameraControl CameraControl;
    private float CameraControlRotateSpeedX;
    private float CameraControlRotateSpeedY;
    private float CameraControlRotateSpeed2;
    public CameraShake CamShake;
    public Camera[] Cameras;
    [Header("Патроны")]
    public Text AmmoCount;
    public GameObject FireBtn;
    public GameObject ScopeBtn;
    public GameObject ReloadBtn;
    public GameObject MeleeBtn;
    [Header("Аудио")]
    public AudioSource GunSfx;
    public AudioClip ScopeSound;
    public AudioClip[] HitSound;
    [Header("HUD")]
    public GameObject MovementUI;
    public GameObject AttackUI;
    public GameObject IndicatorsUI;
    public CanvasGroup GunMarks;
    public Image GunMarkImg;
    public Image GunMark2Img;
    [Header("Слоты оружия")]
    public int CurrentSlot;
    public bool PrimarySlot = true;
    public GameObject Slot0;
    public GameObject Slot1;
    public GameObject NotInSlotWeapon;
    public GameObject WeaponSlot0;
    public GameObject WeaponSlot1;
    [Space(10)]
    public int CurrentWeaponType;
    public GameObject[] WeaponNotInSlot;//0 = svt, 1 = ppsh, 2 = kar98, 3 = mp40, 4 = luger
    public GameObject[] WeaponItemsForCollect;//0 = svt, 1 = ppsh, 2 = kar98, 3 = mp40, 4 = luger

    private void Start()
    {
        CameraControl = FindObjectOfType<ShooterAdvancedCameraControl>();
        CameraControlRotateSpeedX = CameraControl.RotateSpeedX;
        CameraControlRotateSpeedY = CameraControl.RotateSpeedY;
        CameraControlRotateSpeed2 = CameraControl.rotateSpeed;
        CharMovement = GetComponent<CharacterMovement>();
        CharMovementSpeed = CharMovement.speed;
        ReloadBtn.SetActive(false);
        CheckWeapons();
    }

    private void Update()
    {
        if (CurrentWeaponController != null)
        {
            if (Scoped)
                ScopeCameras(CurrentWeaponController.ScopeFactor);
            else
                ScopeCameras(60);
        }

        if (Crosshair.transform.localScale.x > 1)
        {
            Crosshair.transform.localScale = Vector3.Lerp(Crosshair.transform.localScale, Vector3.one, 5 * Time.deltaTime);
        }

        if (ExtCrosshair.alpha > 0)
            ExtCrosshair.alpha = Mathf.Lerp(ExtCrosshair.alpha, 0, 5 * Time.deltaTime);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
            Shoot();
        if (Input.GetKeyUp(KeyCode.E))
            EndShoot();
#endif

        if (CurrentWeaponController != null)
        {
            if (Shooting && CurrentWeaponController.CanShoot)
            {
                Shoot();
            }
        }
    }

    public void Shoot()
    {
        if (CurrentWeaponController != null)
        {
            if (CurrentWeaponController.gunType == WeaponController.GunType.SemiAutomatic)
            {
                ShootFunc();
            }
            if (CurrentWeaponController.gunType == WeaponController.GunType.Automatic)
            {                
                Shooting = true;
                ShootFunc();
            }
        }
    }

    void ShootFunc()
    {
        if (CurrentWeaponController.CanShoot)
        {
            CamShake.shakeAmount = CurrentWeaponController.CamShakeAmount;
            CamShake.shakeDuration = CurrentWeaponController.CamShakeDuration; CurrentWeaponController.Shoot();
            Vector3 CrosshairScale = new Vector3(CurrentWeaponController.Accuracy/5, CurrentWeaponController.Accuracy/5, CurrentWeaponController.Accuracy/5);
            Crosshair.transform.localScale += CrosshairScale * 50;
            CheckAmmo();
        }
    }

    public void EndShoot()
    {
        Shooting = false;
    }

    public void Reload()
    {
        if (CurrentWeaponController != null)
        {
            CurrentWeaponController.Reloading();
            if (Scoped)
                ScopeState();
        }
    }

    public void Melee()
    {
        if (CurrentWeaponController != null)
        {
            CurrentWeaponController.MeleeAttack();
            CamShake.shakeAmount = 0.1f;
            CamShake.shakeDuration = 0.1f;
            if (Scoped)
                ScopeState();
        }
    }

    public void CheckAmmo()
    {
        AmmoCount.text = CurrentWeaponController.AmmoInClip + "/" + CurrentWeaponController.Ammo;
        CurrentWeaponController.CheckNeedReload();
    }

    public void ScopeState()
    {
        Scoped = !Scoped;
        GunSfx.PlayOneShot(ScopeSound);
        if (Scoped)
        {
            CharMovement.Anim.CrossFade("WeaponsCameraZoomIn", 0.1f);
            CharMovement.speed = CharMovementSpeed / 2;
            CameraControl.RotateSpeedX = CameraControlRotateSpeedX / 3;
            CameraControl.RotateSpeedY = CameraControlRotateSpeedY / 3;
            CameraControl.rotateSpeed = CameraControlRotateSpeed2 / 3;
            Crosshair.SetActive(false);
        }
        else
        {
            CharMovement.Anim.CrossFade("WeaponsCameraZoomOut", 0.1f);
            CharMovement.speed = CharMovementSpeed;
            CameraControl.RotateSpeedX = CameraControlRotateSpeedX;
            CameraControl.RotateSpeedY = CameraControlRotateSpeedY;
            CameraControl.rotateSpeed = CameraControlRotateSpeed2;
            Crosshair.SetActive(true);
        }
    }

    void ScopeCameras(float CamerasScopeFactor)
    {
        for (int i = 0; i < Cameras.Length; i++)
            Cameras[i].fieldOfView = Mathf.Lerp(Cameras[i].fieldOfView, CamerasScopeFactor, 3 * Time.deltaTime);
    }

    public void ChangeSlot()
    {
        if (CurrentWeaponController != null && WeaponSlot1 != null)
        {
            PrimarySlot = !PrimarySlot;
            if (Scoped)
                ScopeState();
            StartCoroutine(TakeChangeSlotDelay());
        }
    }

    IEnumerator TakeChangeSlotDelay()
    {
        GunMarks.interactable = false;
        CurrentWeaponController.ChangeGun();
        yield return new WaitForSeconds(CurrentWeaponController.Deselect.length + 0.5f);

        switch (PrimarySlot)
        {
            case true:
                GunMark2Img.sprite = CurrentWeaponController.GunIcon;
                CurrentWeaponController = Slot0.GetComponentInChildren<WeaponController>();
                GunMarkImg.sprite = CurrentWeaponController.GunIcon;
                Slot0.SetActive(true);
                Slot1.SetActive(false);
                break;
            case false:
                GunMark2Img.sprite = CurrentWeaponController.GunIcon;
                CurrentWeaponController = Slot1.GetComponentInChildren<WeaponController>();
                GunMarkImg.sprite = CurrentWeaponController.GunIcon;
                Slot0.SetActive(false);
                Slot1.SetActive(true);
                break;
        }
        GunMarks.interactable = true;
    }

    public void CheckWeapons()
    {
        if (WeaponSlot0 == null)
        {
            AttackUI.SetActive(false);
        }
        else {
            AttackUI.SetActive(true);
            CurrentWeaponController = WeaponSlot0.GetComponent<WeaponController>();
            WeaponSlot0.transform.SetParent(Slot0.transform);
            if (WeaponSlot1 != null)
            {
                WeaponSlot1.transform.SetParent(Slot1.transform);
                GunMark2Img.sprite = WeaponSlot1.GetComponent<WeaponController>().GunIcon;
            }
        }
    }
}