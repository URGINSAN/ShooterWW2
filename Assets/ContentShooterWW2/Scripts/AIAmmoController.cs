using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAmmoController : MonoBehaviour
{
    public enum WeaponType
    {
        Kar98 = 0,
        MP40 = 1
    }
    public WeaponType weaponType;
    public AIWeaponController CurrentWeapon;
    public AIWeaponController[] Weapons;
    private AIController MainAIController;
    public bool TestShooting;

    private CharacterHealthController PlayerHP;
    private Transform Player;
    public bool NearToPlayer;
    public string CurrentAction;
    IEnumerator TakeShoot;
    public Transform EyesTransform;
    public bool SeePlayer;

    RaycastHit hit0;

    private void Start()
    {
        MainAIController = GetComponent<AIController>();
        PlayerHP = FindObjectOfType<CharacterHealthController>();
        Player = FindObjectOfType<CharacterMovement>().PlayerOrigin;

        switch (weaponType)
        {
            case WeaponType.Kar98:
                CurrentWeapon = Weapons[0];
                CurrentWeapon.gameObject.SetActive(true);
                break;
            case WeaponType.MP40:
                CurrentWeapon = Weapons[1];
                CurrentWeapon.gameObject.SetActive(true);
                break;
        }

        CurrentAction = "Waiting";
    }

    private void Update()
    {

        if (Physics.Raycast(EyesTransform.position, (Player.position - transform.position), out hit0, 150f))
        {
            if (hit0.collider.gameObject.layer == 11)
            {
                SeePlayer = true;
            }
            else
            {
                SeePlayer = false;
            }
        }

        Debug.DrawRay(EyesTransform.position, (Player.position - transform.position), Color.blue);

        if (TestShooting && !PlayerHP.Dead)
        {
            if (SeePlayer)
            {
                Shoot();
            }
            else
            {
                if (CurrentAction != "FindingPlayer")
                {
                
                    StartCoroutine(TakeFindPlayer());
                    CurrentAction = "FindingPlayer";
                }
            }
        }

        if (NearToPlayer)
        {
            if (CurrentAction != "Shooting")
            {
                TakeShoot = TakeShootingIE();
                StartCoroutine(TakeShoot);
                CurrentAction = "Shooting";
            }
        }
        else
        {
            if (CurrentAction != "FindingPlayer")
            {
                if (TakeShoot != null)
                    StopCoroutine(TakeShoot);
                CurrentAction = "FindingPlayer";
            }
        }
    }

    IEnumerator TakeShootingIE()
    {
        TestShooting = true;
        yield return new WaitForSeconds(Random.RandomRange(0, MainAIController.SkillFactor));
        TestShooting = false;
        yield return new WaitForSeconds(Random.RandomRange(0, MainAIController.SkillFactor));
        TakeShoot = TakeShootingIE();
        StartCoroutine(TakeShoot);
    }

    public void Shoot()
    {
        CurrentWeapon.Shoot();
    }

    public void ReleaseWeapon()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.GetComponent<BoxCollider>().enabled = true;
            CurrentWeapon.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    IEnumerator TakeFindPlayer()
    {
        CurrentAction = "Waiting";
        yield return new WaitForSeconds(Random.RandomRange(1, 4));
        if ((MainAIController.AIMovement.DistToShoot / 2) > MainAIController.NavAgent.stoppingDistance)
        {
            MainAIController.AIMovement.DistToShoot = MainAIController.AIMovement.DistToShoot / 2;
        }
        else
        {
            MainAIController.AIMovement.DistToShoot = MainAIController.NavAgent.stoppingDistance;
            //MainAIController.AIMovement.TakeRandomStrafe();
        }
        CurrentAction = "FindingPlayer";
    }
}