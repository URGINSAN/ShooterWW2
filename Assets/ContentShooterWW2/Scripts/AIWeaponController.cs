using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWeaponController : MonoBehaviour
{
    public int Bullets = 30;
    public int BulletsInClip;
    public float ShootingDelay = 1;
    private bool CanShoot = true;
    public GameObject MuzzleFlash;
    public GameObject Light;
    [Header("Аудио")]
    public AudioSource GunSFX;
    public AudioClip FireSound;
    [Header("Стрельба")]
    public float LowAccuracy = 0.1f;
    public float HighAccuracy = 0.01f;
    public float Accuracy = 0;
    public float Streight = 10;
    public GameObject Spark;
    public GameObject[] Bulletholes;
    [Space(20)]
    private AIController MainAIController;
    private Transform Player;
    public Transform Origin;
    [Header("Перезарядка")]
    public AnimationClip ReloadAnim;
    public AudioClip[] ReloadSounds;
    public float[] ReloadSoundsDelays;

    private void Start()
    {
        Origin = GetComponentInParent<AIMovementController>().transform;
        Player = FindObjectOfType<CharacterMovement>().transform;
        MainAIController = GetComponentInParent<AIController>();
        BulletsInClip = Bullets;
    }

    public void Shoot()
    {
        if (Bullets > 0 && CanShoot)
        {
            StopAllCoroutines();
            StartCoroutine(TakeShoot());
            StartCoroutine(TakeCanShootDelay());

            Vector3 AccuracyVector = new Vector3(Random.RandomRange(-Accuracy, Accuracy), Random.RandomRange(-Accuracy, Accuracy), Random.RandomRange(-Accuracy, Accuracy));
            RaycastHit hit;
            //if (Physics.Raycast(transform.position, Origin.forward + AccuracyVector, out hit, 150f))
            Debug.DrawRay(transform.position, (Player.position - transform.position) + AccuracyVector, Color.blue);
            if (Physics.Raycast(transform.position, (Player.position - transform.position) + AccuracyVector, out hit, 150f))
            {
                if (hit.collider.gameObject.layer == 11)
                {
                    float Damage = Streight / 100;
                    if (hit.collider.GetComponent<CharacterHealthController>().Health != null)
                    {
                        hit.collider.GetComponent<CharacterHealthController>().Health -= Random.RandomRange(Damage, Damage * 2);
                        hit.collider.GetComponent<CharacterWeaponsController>().CamShake.shakeAmount = 0.05f;
                        hit.collider.GetComponent<CharacterWeaponsController>().CamShake.shakeDuration = 0.05f;
                    }
                }

                if (hit.collider.gameObject.layer != 9 && hit.collider.gameObject.layer != 10 && hit.collider.gameObject.layer != 11)
                {
                    GameObject GO = Instantiate(Bulletholes[Random.RandomRange(0, Bulletholes.Length)], hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    GO.transform.SetParent(hit.transform);
                }

                if (hit.collider.gameObject.layer != 10 && hit.collider.gameObject.layer != 11)
                {
                    GameObject GO2 = Instantiate(Spark, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }

                if (hit.collider.gameObject.GetComponent<Rigidbody>() != null && hit.collider.gameObject.layer != 10)
                {
                    hit.collider.GetComponent<Rigidbody>().AddForceAtPosition(Origin.forward * Streight, hit.point);
                }

                if (hit.collider.gameObject.GetComponent<KickableItem>() != null)
                {
                    hit.collider.gameObject.GetComponent<KickableItem>().Kick(hit.transform, Streight / 200);
                }

                if (hit.collider.gameObject.GetComponent<HPController>() != null)
                {
                    float Damage = Streight / 100;
                    hit.collider.gameObject.GetComponent<HPController>().Health -= Random.RandomRange(Damage, Damage * 2);
                }
            }
        }
    }

    IEnumerator TakeCanShootDelay()
    {
        CanShoot = false;
        yield return new WaitForSeconds(Random.RandomRange(ShootingDelay, ShootingDelay + 0.1f));
        CanShoot = true;
    }

    IEnumerator TakeShoot()
    {
        GunSFX.PlayOneShot(FireSound);
        Bullets--;
        MuzzleFlash.SetActive(true);
        MainAIController.Anim.CrossFade("Shoot", 0.02f, 2);
        //Light.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        MuzzleFlash.SetActive(false);
        //Light.SetActive(false);
        if (Bullets <= 0)
            StartCoroutine(TakeReload());
    }

    IEnumerator TakeReload()
    {
        GetComponentInParent<AIMovementController>().TakeRandomStrafe();
        StartCoroutine(TakeReloadSounds());
        MainAIController.Anim.CrossFade("Reload", 0.1f, 2);
        yield return new WaitForSeconds(ReloadAnim.length);
        Bullets = BulletsInClip;
    }

    IEnumerator TakeReloadSounds()
    {
        yield return new WaitForSeconds(ReloadSoundsDelays[0]);
        GunSFX.PlayOneShot(ReloadSounds[0]);
        yield return new WaitForSeconds(ReloadSoundsDelays[1]);
        GunSFX.PlayOneShot(ReloadSounds[1]);
        yield return new WaitForSeconds(ReloadSoundsDelays[2]);
        GunSFX.PlayOneShot(ReloadSounds[2]);
    }
}