using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public enum WeaponType
    {
        Rifle = 0,
        SMG = 1,
        SMG_German = 2,
        Rifle_German = 3,
        Pistol_German = 4,
        Panzerfaust = 5
    }
    public WeaponType weaponType;

    public enum GunType
    {
        SemiAutomatic = 0,
        Automatic = 1
    }
    public GunType gunType;

    [Header("Патроны")]
    public int AmmoInClip = 10;
    public int Ammo = 30;
    public int MaxAmmoInClip = 10;
    public Sprite GunIcon;
    [Header("Стрельба")]
    public bool CanShoot = false;
    public float LowAccuracy = 0.1f;
    public float HighAccuracy = 0.01f;
    public float Accuracy = 0;
    public float ShootDelay = 0.2f;
    public float CamShakeDuration = 0.2f;
    public float CamShakeAmount = 0.4f;
    public float Streight = 10;
    public GameObject FireLight;
    public GameObject MuzzleFlash;
    public float ScopeFactor = 40;
    public GameObject Spark;
    public GameObject[] Bulletholes;
    [Header("Аудио")]
    public AudioSource GunSfx;
    public AudioClip SelectSound;
    public AudioClip ShootSound;
    public AudioClip MeleeSound;
    public AudioClip[] MeleeHit;
    [Space(20)]
    public AudioClip Reload0Sound;
    public AudioClip Reload1Sound;
    public AudioClip Reload2Sound;
    public float[] ReloadSoundDelays;
    [Header("Анимация")]
    public Animator Anim;
    public AnimationClip Select;
    public AnimationClip Reload;
    public AnimationClip Melee;
    public AnimationClip Deselect;

    public CharacterWeaponsController CharacterWeapons;
    public CharacterAmmoController CharacterAmmo;
    public int CurrentWeaponType;

    private void OnEnable()
    {
        CharacterWeapons.CurrentWeaponType = CurrentWeaponType;
        StartCoroutine(OnStart());
        RecordAmmoType();
    }
    private void OnDisable()
    {
        CharacterWeapons.GunMarks.interactable = true;
    }

    IEnumerator OnStart()
    {
        ApplyAmmoType();
        CharacterWeapons.CurrentWeaponController = this;
        CharacterWeapons.CheckAmmo();
        CharacterWeapons.GunMarkImg.sprite = GunIcon;
        CharacterWeapons.AttackUI.SetActive(false);
        Accuracy = HighAccuracy;
        GunSfx.PlayOneShot(SelectSound);
        yield return new WaitForSeconds(Select.length + 0.5f);
        CheckBullets();
        CharacterWeapons.AttackUI.SetActive(true);
    }

    public void ApplyAmmoType()
    {
        switch (weaponType)
        {
            case WeaponType.Rifle:
                Ammo = CharacterAmmo.RifleBullets;
                HandlingButtonsState(true);
                break;
            case WeaponType.SMG:
                Ammo = CharacterAmmo.SMGBullets;
                HandlingButtonsState(true);
                break;
            case WeaponType.SMG_German:
                Ammo = CharacterAmmo.SMGGermanBullets;
                HandlingButtonsState(true);
                break;
            case WeaponType.Rifle_German:
                Ammo = CharacterAmmo.RifleGermanBullets;
                HandlingButtonsState(true);
                break;
            case WeaponType.Pistol_German:
                Ammo = CharacterAmmo.PistolGermanBullets;
                HandlingButtonsState(true);
                break;
            case WeaponType.Panzerfaust:
                Ammo = CharacterAmmo.PanzerfaustBullets;
                HandlingButtonsState(false);
                break;
        }
    }

    void RecordAmmoType()
    {
        switch (weaponType)
        {
            case WeaponType.Rifle:
                CharacterAmmo.RifleBullets = Ammo;// + AmmoInClip; 
                break;
            case WeaponType.SMG:
                CharacterAmmo.SMGBullets = Ammo;// + AmmoInClip;
                break;
            case WeaponType.SMG_German:
                CharacterAmmo.SMGGermanBullets = Ammo;// + AmmoInClip;
                break;
            case WeaponType.Rifle_German:
                CharacterAmmo.RifleGermanBullets = Ammo;// + AmmoInClip;
                break;
            case WeaponType.Pistol_German:
                CharacterAmmo.PistolGermanBullets = Ammo;// + AmmoInClip;
                break;
            case WeaponType.Panzerfaust:
                CharacterAmmo.PanzerfaustBullets = Ammo;// + AmmoInClip;
                break;
        }
    }

    void HandlingButtonsState(bool State)
    {
        CharacterWeapons.ScopeBtn.SetActive(State);
        CharacterWeapons.MeleeBtn.SetActive(State);
    }

    private void Update()
    {
        if (AmmoInClip == 0 && Ammo == 0)
            CanShoot = false;
    }

    public void Shoot()
    {
        if (CanShoot)
        {
            Anim.Play("Fire");
            GunSfx.PlayOneShot(ShootSound);
            FireLight.SetActive(true);
            if (MuzzleFlash != null)
            {
                MuzzleFlash.transform.localEulerAngles = new Vector3(MuzzleFlash.transform.localEulerAngles.x, MuzzleFlash.transform.localEulerAngles.y, Random.RandomRange(0, 360));
                MuzzleFlash.SetActive(true);
            }

            Vector3 AccuracyVector = new Vector3(Random.RandomRange(-Accuracy, Accuracy), Random.RandomRange(-Accuracy, Accuracy), Random.RandomRange(-Accuracy, Accuracy));
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward + AccuracyVector, out hit, 200))
            {
                if (hit.collider.gameObject.layer != 9 && hit.collider.gameObject.layer != 10)
                {
                    GameObject GO = Instantiate(Bulletholes[Random.RandomRange(0, Bulletholes.Length)], hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    GO.transform.SetParent(hit.transform);
                }

                if (hit.collider.gameObject.layer != 10)
                {
                    GameObject GO2 = Instantiate(Spark, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }
                /*
                if (hit.collider.gameObject.GetComponent<Rigidbody>() != null && hit.collider.gameObject.layer != 10)
                {
                    hit.rigidbody.AddForceAtPosition(Camera.main.transform.forward * Streight, hit.point);
                }*/

                if (hit.collider.gameObject.GetComponent<HPController>() != null)
                {
                    float Damage = Streight / 100;
                    hit.collider.gameObject.GetComponent<HPController>().Health -= Random.RandomRange(Damage, Damage*2);
                }

                if (hit.collider.gameObject.GetComponent<KickableItem>() != null)
                {
                    hit.collider.gameObject.GetComponent<KickableItem>().Kick(hit.transform, Streight / 200);
                }

                if (hit.collider.gameObject.GetComponent<CharacterHitBox>() != null)
                {
                    float Damage = Streight / 100;
                    hit.collider.gameObject.GetComponent<CharacterHitBox>().Damage(Random.RandomRange(Damage, Damage * 2));
                    CharacterWeapons.ExtCrosshair.alpha = 1;
                    CharacterWeapons.GunSfx.PlayOneShot(CharacterWeapons.HitSound[Random.RandomRange(0, CharacterWeapons.HitSound.Length)]);
                }

                FindClosestEnemy();
            }

            AmmoInClip--;
            RecordAmmoType();
            StartCoroutine(TakeShootDelay());
            CanShoot = false;
        }
    }

    IEnumerator TakeShootDelay()
    {
        yield return new WaitForSeconds(0.075f);
        FireLight.SetActive(false);
        if (MuzzleFlash != null) 
            MuzzleFlash.SetActive(false);
        yield return new WaitForSeconds(ShootDelay);
        CheckBullets();
    }

    public void CheckBullets()
    {
        CheckNeedReload();

        if (AmmoInClip > 0)
        {
            CanShoot = true;
        }
        if (AmmoInClip == 0 && Ammo > 0)
            Reloading();
    }

    public void CheckNeedReload()
    {
        if (AmmoInClip < MaxAmmoInClip && Ammo > 0)
            CharacterWeapons.ReloadBtn.SetActive(true);
        if (AmmoInClip == MaxAmmoInClip)
            CharacterWeapons.ReloadBtn.SetActive(false);
        if (AmmoInClip == 0 && Ammo == 0)
            CharacterWeapons.ReloadBtn.SetActive(false);
    }

    public void Reloading()
    {
        if (CharacterWeapons.Scoped)
            CharacterWeapons.ScopeState();
        StartCoroutine(TakeReload());
    }

    IEnumerator TakeReload()
    {
        CanShoot = false;
        CharacterWeapons.ScopeBtn.SetActive(false);
        CharacterWeapons.ReloadBtn.SetActive(false);
        CharacterWeapons.MeleeBtn.SetActive(false);
        CharacterWeapons.GunMarks.interactable = false;
        Anim.CrossFade("EmptyReload", 0.1f);
        StartCoroutine(TakeReloadSound());
        yield return new WaitForSeconds(Reload.length + 1);

        if (Ammo < MaxAmmoInClip)
        {
            int TempBullets = MaxAmmoInClip - AmmoInClip;

            if (Ammo < TempBullets)
            {
                AmmoInClip += Ammo;
                Ammo = 0;
            }
            if (Ammo >= TempBullets)
            {
                AmmoInClip += TempBullets;
                Ammo -= TempBullets;
            }
        }
        if (Ammo >= MaxAmmoInClip)
        {
            int TempBullets = MaxAmmoInClip - AmmoInClip;
            AmmoInClip = MaxAmmoInClip;
            Ammo -= TempBullets;
        }

        CharacterWeapons.CheckAmmo();
        RecordAmmoType();
        CharacterWeapons.ReloadBtn.SetActive(false);
        CharacterWeapons.ScopeBtn.SetActive(true);
        CharacterWeapons.MeleeBtn.SetActive(true);
        CharacterWeapons.GunMarks.interactable = true;

        if (weaponType == WeaponType.Panzerfaust)
            HandlingButtonsState(false);
        CanShoot = true;
    }

    IEnumerator TakeReloadSound()
    {
        yield return new WaitForSeconds(ReloadSoundDelays[0]);
        GunSfx.PlayOneShot(Reload0Sound);
        yield return new WaitForSeconds(ReloadSoundDelays[1]);
        GunSfx.PlayOneShot(Reload1Sound);
        yield return new WaitForSeconds(ReloadSoundDelays[2]);
        GunSfx.PlayOneShot(Reload2Sound);
    }

    public void MeleeAttack()
    {
        StartCoroutine(TakeMeleeAttack());
    }
    IEnumerator TakeMeleeAttack()
    {
        GunSfx.PlayOneShot(MeleeSound);
        Anim.Play("MeleeAttack");
        CharacterWeapons.AttackUI.SetActive(false);

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 1.5f))
        {
            GunSfx.PlayOneShot(MeleeHit[Random.RandomRange(0, MeleeHit.Length)]);
            if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                hit.rigidbody.AddForceAtPosition(Camera.main.transform.forward * 1000, hit.point);
            }
            if (hit.collider.gameObject.GetComponent<CharacterHitBox>() != null)
            {
                hit.collider.gameObject.GetComponent<CharacterHitBox>().Damage(5);
            }
            if (hit.collider.gameObject.GetComponent<KickableItem>() != null)
            {
                hit.collider.gameObject.GetComponent<KickableItem>().Kick(hit.transform, Streight / 200);
            }
        }

        yield return new WaitForSeconds(Melee.length);
        CharacterWeapons.AttackUI.SetActive(true);
    }

    public void ChangeGun()
    {
        if (CharacterWeapons.CurrentWeaponController != null && CharacterWeapons.WeaponSlot1 != null)
        {
            CharacterWeapons.AttackUI.SetActive(false);
            Anim.Play("Deselect");
        }
    }
    
    public void FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");

        float distance = 150;

        for (int i = 0; i < gos.Length; i++)
        {
            if (Vector3.Distance(transform.position, gos[i].transform.position) < distance)
                gos[i].GetComponent<AIController>().GetAlarmed();
        }
    }
}