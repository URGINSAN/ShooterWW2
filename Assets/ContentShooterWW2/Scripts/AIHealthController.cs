using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHealthController : MonoBehaviour
{
    [Header("Здоровье")]
    public float Health = 100;
    private float PreviousHealth;
    public bool Dead;
    private bool CanDeath = true;
    public Rigidbody RootRigid;
    [Space(20)]
    private AIController MainAIController;
    private AIMovementController MoveAIController;

    void Start()
    {
        MainAIController = GetComponent<AIController>();
        MoveAIController = GetComponent<AIMovementController>();
        KinemateRigids(true);
        PreviousHealth = Health;
    }

    private void Update()
    {
        if ((Health < 0 || Dead) && CanDeath)
            Death();
    }

    private void LateUpdate()
    {
        if (PreviousHealth > Health)
        {
            GetPain();
            PreviousHealth = Health;
        }
    }

    public void Death()
    {
        MainAIController.Angry = false;
        MainAIController.Alarm = false;
        GetComponent<CapsuleCollider>().enabled = false;
        
        MainAIController.Anim.enabled = false;
        MainAIController.NavAgent.speed = 0;
        MainAIController.NavAgent.enabled = false;
        GetComponent<AIAmmoController>().ReleaseWeapon();
        KinemateRigids(false);

        StopAllCoroutines();
        MainAIController.OnDeath();
        Destroy(gameObject, 20);
        CanDeath = false;
    }

    void GetPain()
    {
        MainAIController.AISFX.clip = MainAIController.PainSounds[Random.RandomRange(0, MainAIController.PainSounds.Length)];
        MainAIController.AISFX.Play();
        MainAIController.Anim.Play("Hit", 1);

        MainAIController.Alarm = true;
        MainAIController.Angry = true;

        MoveAIController.TakeRandomStrafe();
    }

    public void PainFromExplosion(Transform Explosion)
    {
        float Power = Explosion.GetComponent<DetonatorForce>().power;
        float Dist = Vector3.Distance(transform.position, Explosion.position);

        if (Dist < 5 && Power > 3000)
        {
            Death();

            RootRigid.AddForce(Vector3.left / 2, ForceMode.Impulse);
        }
        Health -= Mathf.InverseLerp(10, 0, Dist) * 200;
    }

    void KinemateRigids(bool State)
    {
        var rigidbodyComponents = GetComponentsInChildren<Rigidbody>(true);
        foreach (var component in rigidbodyComponents)
        {
            component.isKinematic = State;
            component.mass = component.mass * 100;
        }
    }
}
