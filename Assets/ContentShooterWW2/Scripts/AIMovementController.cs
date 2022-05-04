using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIMovementController : MonoBehaviour
{
    [Header("Поведение")]
    //public Transform DestinationPoint;
    public string CurrentAction;
    private bool CanAngry = true;
    public float DistToShoot = 8;
    [Header("Разное")]
    private float NavAgentSpeed = 3f;
    private bool CanMoveSound = true;
    public float CanMoveDelay = 0.5f;
    private Transform Player;
    [Space(20)]
    [Header("Стрейфы")]
    public bool StrafeToLeftTest;
    private bool StrafeToLeft;
    public bool StrafeToRightTest;
    private bool StrafeToRight;
    [Space(20)]
    private AIController MainAIController;
    private AIAmmoController AmmoAIController;
    public float Dist;

    IEnumerator TakeRandomActionNearPlayer;

    void Start()
    {
        MainAIController = GetComponent<AIController>();
        AmmoAIController = GetComponent<AIAmmoController>();
        Player = FindObjectOfType<CharacterMovement>().transform;
        NavAgentSpeed = MainAIController.NavAgent.speed;
        MainAIController.NavAgent.speed = 0;

        if (MainAIController.Angry)
        {
            MainAIController.NavAgent.speed = NavAgentSpeed;
        }
        else
        {
            int Rand = Random.RandomRange(0, 2);
            switch (Rand)
            {
                case 0:
                    MainAIController.AnimSet(0);
                    break;
                case 1:
                    MainAIController.AnimSet(2);
                    break;
            }
            MainAIController.NavAgent.speed = 0;
            CurrentAction = "Stay";
        }
    }

    void Update()
    {
        Dist = Vector3.Distance(transform.position, Player.position);

        if (CanMoveSound && MainAIController.NavAgent.speed > 0)
        {
            MainAIController.AISFX.PlayOneShot(MainAIController.MoveRockSound[Random.RandomRange(0, MainAIController.MoveRockSound.Length)]);
            StartCoroutine(TakeMoveSoundDelay());
            CanMoveSound = false;
        }

        if (MainAIController.Angry)
        {
            if (MainAIController.NavAgent.speed > 0 && CurrentAction != "Move")
            {
                int Rand = 0;
                Rand = Random.RandomRange(0, 3);
                switch (Rand)
                {
                    case 0:
                        MainAIController.AnimSet(6);
                        break;
                    case 1:
                        MainAIController.AnimSet(5);
                        break;
                    case 2:
                        MainAIController.AnimSet(6);
                        break;
                }

                if (TakeRandomActionNearPlayer != null)
                    StopCoroutine(TakeRandomActionNearPlayer);
                CurrentAction = "Move";
            }

            if (MainAIController.NavAgent.speed <= 0 && CurrentAction != "StayNearPlayer")
            {
                int Rand = 0;
                Rand = Random.RandomRange(0, 2);
                switch (Rand)
                {
                    case 0:
                        MainAIController.AnimSet(0);
                        break;
                    case 1:
                        MainAIController.AnimSet(3);
                        break;
                }

                TakeRandomActionNearPlayer = TakeRandomActionNearPlayerIE();
                StartCoroutine(TakeRandomActionNearPlayer);
                CurrentAction = "StayNearPlayer";
            }
        }

        if (MainAIController.Angry)
        {
            MainAIController.NavAgent.SetDestination(Player.position);

            if (Dist > DistToShoot)
            {
                LookToPlayer(Player);
                AmmoAIController.NearToPlayer = false;

                if (!StrafeToLeft && !StrafeToRight)
                {
                    MainAIController.NavAgent.speed = NavAgentSpeed;

                    MainAIController.Alarm = true;
                    MainAIController.NavAgent.SetDestination(Player.position);
                }

                if (StrafeToLeft || StrafeToRight)
                {
                    LookToPlayer(Player);
                }
            }
            if (Dist < DistToShoot)
            {
                MainAIController.Alarm = true;
                AmmoAIController.NearToPlayer = true;
                MainAIController.NavAgent.speed = 0;
                LookToPlayer(Player);
            }
        }

        if (StrafeToLeftTest)
        {
            StrafeToLeft = true;
            StartCoroutine(StrafeTimer());
            StrafeToLeftTest = false;
        }

        if (StrafeToLeft)
        {
            MainAIController.AnimSet(7);
            transform.Translate(Vector3.left * NavAgentSpeed * Time.deltaTime);
        }

        if (StrafeToRightTest)
        {
            StrafeToRight = true;
            StartCoroutine(StrafeTimer());
            StrafeToRightTest = false;
        }

        if (StrafeToRight)
        {
            MainAIController.AnimSet(8);
            transform.Translate(Vector3.right * NavAgentSpeed * Time.deltaTime);
        }
    }

    IEnumerator TakeRandomActionNearPlayerIE()
    {
        CurrentAction = "StayNearPlayer";
        yield return new WaitForSeconds(Random.RandomRange(2, 5));
        TakeRandomStrafe();
        TakeRandomActionNearPlayer = TakeRandomActionNearPlayerIE();
        StartCoroutine(TakeRandomActionNearPlayer);
    }

    public void TakeRandomStrafe()
    {
        if (TakeRandomActionNearPlayer != null)
            StopCoroutine(TakeRandomActionNearPlayer);
        StrafeToLeftTest = false;
        StrafeToRightTest = false;

        int Rand = Random.RandomRange(0, 3);
        switch (Rand)
        {
            case 0:
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -transform.right, out hit, 6f))
                {
                    if (hit.collider.gameObject.layer != 10)
                    {
                        TakeRandomStrafe();
                    }
                }
                else
                {
                    StrafeToLeftTest = true;
                    CurrentAction = "StrafeLeft";
                }
                
                break;
            case 1:
                RaycastHit hit2;
                if (Physics.Raycast(transform.position, transform.right, out hit2, 6f))
                {
                    if (hit2.collider.gameObject.layer != 10)
                    {
                        TakeRandomStrafe();
                    }
                }
                else
                {
                    StrafeToRightTest = true;
                    CurrentAction = "StrafeRight";
                }
                
                break;
            case 2:
                CurrentAction = "Waiting";
                break;
            case 3:
                CurrentAction = "Waiting";
                break;
        }
    }

    IEnumerator StrafeTimer()
    {
        MainAIController.NavAgent.speed = NavAgentSpeed;
        yield return new WaitForSeconds(1);
        StrafeToLeft = false;
        StrafeToRight = false;
        MainAIController.AnimSet(3);
        MainAIController.NavAgent.speed = 0;
    }

    IEnumerator TakeMoveSoundDelay()
    {
        yield return new WaitForSeconds(CanMoveDelay);
        CanMoveSound = true;
    }

    void LookToPlayer(Transform Target)
    {
        Vector3 lookVector = Target.transform.position - transform.position;
        lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10 * Time.deltaTime);
    }
}
