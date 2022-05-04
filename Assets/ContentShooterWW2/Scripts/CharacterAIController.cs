using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAIController : MonoBehaviour
{
    public bool Alarmed;
    public bool Angry;
    private bool CanAngry;
    [Header("Здоровье")]
    public float Health = 100;
    private float PreviousHealth;
    public bool Dead;
    private bool CanDeath = true;
    public Rigidbody RootRigid;
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
    [Header("Поведение")]
    public string CurrentAction;//0 = shoot, 
    public bool CanChangePos = true;
    private int RandPos;
    private int LastRandPos;
    public Transform CurrentChoosedPos;
    private IEnumerator ChangePosition;
    [Header("Разное")]
    public Transform[] GunsItems;
    public Transform[] DestPoints;
    private NavMeshAgent NavAgent;
    private float NavAgentSpeed;
    private bool CanMoveSound = true;
    public float CanMoveDelay = 0.5f;
    [Header("Атака")]
    public AIWeaponController CurrentWeapon;
    private Transform Player;
    public bool DestinatedPlayer;
    //public float DestantionToPlayerForAttack = 4;
    public float Y;
    private IEnumerator ShootingIE;
    public bool CanShoot = true;

    public enum GunType
    {
        Rifle = 0,
        MP40 = 1
    }
    public GunType gunType;

    private void Start()
    {
        Player = FindObjectOfType<CharacterMovement>().transform;
        NavAgent = GetComponent<NavMeshAgent>();
        NavAgentSpeed = NavAgent.speed;
        NavAgent.speed = 0;
        PreviousHealth = Health;
        KinemateRigids(true);

        switch (gunType)
        {
            case GunType.Rifle:
                GunsItems[0].gameObject.SetActive(true);
                CurrentWeapon = GunsItems[0].GetComponent<AIWeaponController>();
                break;
            case GunType.MP40:
                GunsItems[1].gameObject.SetActive(true);
                CurrentWeapon = GunsItems[1].GetComponent<AIWeaponController>();
                break;
        }

        if (CanChangePos)
        {
            ChangePosition = TakeChangePosition(5);
            StartCoroutine(ChangePosition);
        }
    }

    IEnumerator TakeChangePosition(float Delay)
    {
        CanChangePos = false;
        
        yield return new WaitForSeconds(Random.RandomRange(Delay / 2, Delay));

        while (RandPos == LastRandPos)
        {
            RandPos = Random.RandomRange(0, DestPoints.Length);
        }
        while (RandPos != LastRandPos)
        {
            CurrentChoosedPos = DestPoints[RandPos];
            LastRandPos = RandPos;
            AnimSet(1);
        }
    }

    private void Update()
    {
        if (Alarmed && CanAlarmVoice)
        {
            StartCoroutine(TakeCanAlarmVoice());
            CanAlarmVoice = false;
        }

        if (Angry && CanAngry)
        {
            Alarmed = true;
            CanChangePos = true;
            CanAngry = false;
        }

        if ((Health < 0 || Dead) && CanDeath)
            Death();

        if (!Angry)
        {
            if (CurrentChoosedPos != null)
            {
                if (Vector3.Distance(transform.position, CurrentChoosedPos.position) <= (NavAgent.stoppingDistance + 0.2f) && !CanChangePos)
                {
                    NavAgent.speed = 0;
                    AnimSet(0);

                    ChangePosition = TakeChangePosition(7);
                    StartCoroutine(ChangePosition);
                    LookToPlayer(0);
                    //transform.LookAt(Player.transform);
                    CanChangePos = true;
                }
                if (Vector3.Distance(transform.position, CurrentChoosedPos.position) >= (NavAgent.stoppingDistance + 0.2f))
                {
                    CanChangePos = false;
                    if (NavAgent.enabled == true)
                        NavAgent.SetDestination(CurrentChoosedPos.position);

                    if (Alarmed)
                    {
                        CanMoveDelay = 0.35f;
                        NavAgent.speed = NavAgentSpeed * 4;
                    }
                    else
                    {
                        NavAgent.speed = NavAgentSpeed;
                        CanMoveDelay = 0.6f;
                    }
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, Player.transform.position) > 4) //////////////////ВИДИТ ПЛЕЕРА
            {
                if (NavAgent.enabled == true)
                    NavAgent.SetDestination(Player.position);

                CanMoveDelay = 0.35f;
                NavAgent.speed = NavAgentSpeed * 4;
                AnimSet(1);

                ShootingIE = Shooting();
                StopCoroutine(ShootingIE);
                DestinatedPlayer = false;
                //DestantionToPlayerForAttack = 4;
                //CanChangePos = false;
            }

            if (Vector3.Distance(transform.position, Player.transform.position) <= 4) //////////////////ВПРИТЫК К ПЛЕЕРУ
            {
                if (NavAgent.enabled == true)
                    NavAgent.speed = 0;

                LookToPlayer(0);

                ChooseAction();
                DestinatedPlayer = true;
                //CanChangePos = true;
            }
        }

        if (CanMoveSound && NavAgent.speed > 0)
        {
            AISFX.PlayOneShot(MoveRockSound[Random.RandomRange(0, MoveRockSound.Length)]);
            StartCoroutine(TakeMoveSoundDelay());
            CanMoveSound = false;
        }
    }
    /*
    IEnumerator TakeAttackPlayer()
    {
        yield return new WaitForSeconds(Random.RandomRange(2, 10));
        DestantionToPlayerForAttack = 1.5f;
    }*/

    private void LateUpdate()
    {
        if (PreviousHealth > Health)
        {
            GetPain();
            PreviousHealth = Health;
        }

        //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + Y, 0);
    }

    public void Death()
    {
        Angry = false;
        Alarmed = false;
        GetComponent<CapsuleCollider>().enabled = false;
        KinemateRigids(false);
        Anim.enabled = false;
        NavAgent.speed = 0;
        NavAgent.enabled = false;

        for (int i = 0; i < GunsItems.Length; i++)
        {
            GunsItems[i].SetParent(null);
            GunsItems[i].GetComponent<BoxCollider>().enabled = true;
            GunsItems[i].GetComponent<Rigidbody>().isKinematic = false;
        }

        StopAllCoroutines();
        CanMoveSound = false;
        StartCoroutine(AfterDeath());
        CanDeath = false;
    }

    IEnumerator Shooting()
    {
        CanShoot = false;
        //yield return new WaitForSeconds();// Random.RandomRange(0, CurrentWeapon.ShootingDelay / 2));
        Anim.Play("Shoot", 1);
        CurrentWeapon.Shoot();
        yield return new WaitForSeconds(CurrentWeapon.ShootingDelay);
        CanShoot = true;
    }

    void GetPain()
    {
        AISFX.clip = PainSounds[Random.RandomRange(0, PainSounds.Length)];
        AISFX.Play();
        Anim.Play("Hit", 1);

        Angry = true;
        Alarmed = true;
        CanChangePos = true;

        if (!Angry)
        {
            ChangePosition = TakeChangePosition(0.5f);
            StopCoroutine(ChangePosition);
            StartCoroutine(ChangePosition);
        }
    }

    void KinemateRigids(bool State)
    {
        var rigidbodyComponents = GetComponentsInChildren<Rigidbody>(true);
        foreach (var component in rigidbodyComponents)
            component.isKinematic = State;
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

    IEnumerator AfterDeath()
    {
        
        yield return new WaitForSeconds(1);

        AISFX.clip = DeathSounds[Random.RandomRange(0, DeathSounds.Length)];
        AISFX.Play();

        yield return new WaitForSeconds(20);
        Destroy(gameObject);
    }

    public void GetAlarmed()
    {
        if (!Angry)
        {
            Alarmed = true;
            CanChangePos = true;
            ChangePosition = TakeChangePosition(0f);
            StopCoroutine(ChangePosition);
            StartCoroutine(ChangePosition);
        }
    }

    void AnimSet(int State)
    {
        if (Alarmed)//Тревога
        {
            switch (State)
            {
                case 0:
                    if (CurrentAnim != "IdleCrouching")
                    {
                        Anim.CrossFade("IdleCrouching", 0.5f);
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
            }
        }
    }

    IEnumerator TakeMoveSoundDelay()
    {
        yield return new WaitForSeconds(CanMoveDelay);
        CanMoveSound = true;
    }

    IEnumerator TakeCanAlarmVoice()
    {
        yield return new WaitForSeconds(Random.RandomRange(0, 3));
        if (Health > 0)
            AISFX.PlayOneShot(AlarmVoices[Random.RandomRange(0, AlarmVoices.Length)]);
        yield return new WaitForSeconds(Random.RandomRange(3,10));
        CanAlarmVoice = true;
    }

    public void ChooseAction()
    {
        int Rand = Random.RandomRange(0, 2);
        if (Rand == 0)
            CurrentAction = "Shoot";
        if (Rand == 1)
            CurrentAction = "Attack";

        switch (CurrentAction)
        {
            case "Shoot":
                if (CanShoot && CurrentWeapon.Bullets > 0)
                {
                    ShootingIE = Shooting();
                    StartCoroutine(ShootingIE);
                }
                AnimSet(0);
                break;
            case "Attack":

                break;
        }
    }
    
    void LookToPlayer(float YFactor)
    {
        Vector3 lookVector = Player.transform.position - transform.position;
        lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10 * Time.deltaTime);
    }
}