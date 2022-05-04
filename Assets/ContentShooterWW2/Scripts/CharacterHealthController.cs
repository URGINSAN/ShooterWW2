using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthController : MonoBehaviour
{
    [Header("Здоровье")]
    public float Health = 100;
    private float PreviousHealth;
    public bool Dead;
    private bool CanDeath = true;
    [Header("Эффекты")]
    public CanvasGroup PainOverlay;
    [Header("Аудио")]
    public AudioSource PlayerSFX;
    public AudioClip[] PainSound;
    public AudioClip[] DeathSound;

    private void Start()
    {
        PreviousHealth = Health;
    }

    private void Update()
    {
        if ((Health < 0 || Dead) && CanDeath)
            Death();

        if (PainOverlay.alpha > 0 && !Dead)
            PainOverlay.alpha = Mathf.Lerp(PainOverlay.alpha, 0, Time.deltaTime / 2);
    }

    private void LateUpdate()
    {
        if (PreviousHealth > Health)
        {
            GetPain();
            PreviousHealth = Health;
        }
    }

    void Death()
    {
        GetComponent<CharacterMovement>().enabled = false;
        GetComponent<CharacterAmmoController>().enabled = false;
        CharacterWeaponsController CharacterWeapons = GetComponent<CharacterWeaponsController>();
        CharacterWeapons.enabled = false;
        CharacterWeapons.AttackUI.SetActive(false);
        CharacterWeapons.MovementUI.SetActive(false);
        CharacterWeapons.IndicatorsUI.SetActive(false);
        CharacterWeapons.Slot0.SetActive(false);
        CharacterWeapons.Slot1.SetActive(false);

        GetComponent<CharacterController>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(Vector3.up * Random.RandomRange(-5, 5), ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(Vector3.right * Random.RandomRange(-5, 5), ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque(Vector3.right * Random.RandomRange(-5, 5), ForceMode.Impulse);
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        GetComponentInChildren<ShooterAdvancedCameraControl>().enabled = false;

        PlayerSFX.clip = DeathSound[Random.RandomRange(0, DeathSound.Length)];
        PlayerSFX.Play();
        PainOverlay.alpha = 0.8f;

        Dead = true;
        enabled = false;
        CanDeath = false;
    }

    void GetPain()
    {
        PainOverlay.alpha += 0.8f;
        PlayerSFX.clip = PainSound[Random.RandomRange(0, PainSound.Length)];
        PlayerSFX.Play();
    }

    public void PainFromExplosion(Transform Explosion)
    {
        float Dist = Vector3.Distance(transform.position, Explosion.position);
        Health -= Mathf.InverseLerp(10, 0, Dist) * 200;
    }
}