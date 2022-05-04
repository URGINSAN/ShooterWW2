using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [Header("Статус")]
    public bool Alarm;
    public bool Angry;
    [Header("Уровень")]
    public float SkillFactor = 2;//10 = noob ====> 2 = profi; 
    [Header("Навигация")]
    public NavMeshAgent NavAgent;
    [Header("Анимация")]
    public Animator Anim;
    public string CurrentAnim;
    [Header("Аудио")]
    public AudioSource AISFX;
    public AudioClip[] AlarmVoices;
    private bool CanAlarmVoice = true;
    public AudioClip[] PainSounds;
    public AudioClip[] DeathSounds;
    public AudioClip[] MoveRockSound;

    public AIMovementController AIMovement;
    public AIHealthController AIHealth;
    public AIAmmoController AIAmmo;

    private void Start()
    {
        AIMovement = GetComponent<AIMovementController>();
        AIHealth = GetComponent<AIHealthController>();
        AIAmmo = GetComponent<AIAmmoController>();
        AnimSet(0);    
    }

    private void Update()
    {
        if (Alarm && CanAlarmVoice)
        {
            StartCoroutine(TakeCanAlarmVoice());
            CanAlarmVoice = false;
        }
    }

    public void GetAlarmed()
    {
        if (!Angry)
        {
            Alarm = true;
            Angry = true;
        }
    }

    public void AnimSet(int State)
    {
        if (Alarm)//Тревога
        {
            switch (State)
            {
                case 0:
                    if (CurrentAnim != "IdleCrouching")
                    {
                        Anim.CrossFade("IdleCrouching", 0.2f);
                        CurrentAnim = "IdleCrouching";
                    }
                    break;
                case 1:
                    if (CurrentAnim != "Run")
                    {
                        Anim.CrossFade("Run", 0.1f);
                        CurrentAnim = "Run";
                    }
                    break;
                case 2:
                    if (CurrentAnim != "Attack")
                    {
                        Anim.CrossFade("Attack", 0.1f);
                        CurrentAnim = "Attack";
                    }
                    break;
                case 3:
                    if (CurrentAnim != "Crouch")
                    {
                        Anim.CrossFade("ToCrouch", 0.1f);
                        CurrentAnim = "Crouch";
                    }
                    break;
                case 4:
                    if (CurrentAnim != "WalkDamaged")
                    {
                        Anim.CrossFade("WalkDamaged", 0.1f);
                        CurrentAnim = "WalkDamaged";
                    }
                    break;
                case 5:
                    if (CurrentAnim != "Move1")
                    {
                        Anim.CrossFade("Move1", 0.1f);
                        CurrentAnim = "Move1";
                    }
                    break;
                case 6:
                    if (CurrentAnim != "Move2")
                    {
                        Anim.CrossFade("Move2", 0.1f);
                        CurrentAnim = "Move2";
                    }
                    break;
                case 7:
                    if (CurrentAnim != "StrafeLeft")
                    {
                        int Rand = Random.RandomRange(0, 2);
                        switch (Rand)
                        {
                            case 0:
                                Anim.CrossFade("StrafeLeft", 0.1f);
                                break;
                            case 1:
                                Anim.CrossFade("StrafeLeft2", 0.1f);
                                break;
                        }
                        CurrentAnim = "StrafeLeft";
                    }
                    break;
                case 8:
                    if (CurrentAnim != "StrafeRight")
                    {
                        int Rand = Random.RandomRange(0, 2);
                        switch (Rand)
                        {
                            case 0:
                                Anim.CrossFade("StrafeRight", 0.1f);
                                break;
                            case 1:
                                Anim.CrossFade("StrafeRight2", 0.1f);
                                break;
                        }
                        CurrentAnim = "StrafeRight";
                    }
                    break;
            }
        }
        else        //Спокойствие
        {
            switch (State)
            {
                case 0:
                    if (CurrentAnim != "Idle")
                    {
                        Anim.CrossFade("Idle", 0.1f);
                        CurrentAnim = "Idle";
                    }
                    break;
                case 1:
                    if (CurrentAnim != "Walk")
                    {
                        Anim.CrossFade("Walk", 0.1f);
                        CurrentAnim = "Walk";
                    }
                    break;
                case 2:
                    if (CurrentAnim != "RareIdle")
                    {
                        Anim.CrossFade("RareIdle", 0.1f);
                        CurrentAnim = "RareIdle";
                    }
                    break;
            }
        }
    }

    public void OnDeath()
    {
        enabled = false;
        AIHealth.enabled = false;
        AIMovement.enabled = false;
        AIAmmo.enabled = false;
        GetComponent<AIAmmoController>().enabled = false;
        GetComponent<AudioSource>().enabled = false;
    }

    IEnumerator TakeCanAlarmVoice()
    {
        yield return new WaitForSeconds(Random.RandomRange(0, 3));
        if (GetComponent<AIHealthController>().Health > 0)
            AISFX.PlayOneShot(AlarmVoices[Random.RandomRange(0, AlarmVoices.Length)]);
        yield return new WaitForSeconds(Random.RandomRange(3, 10));
        CanAlarmVoice = true;
    }
}