using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterPlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float Speed = 2;
    public Animator Anim;
    private bool Move;
    private bool BackMove;
    public bool End;
    private CharacterController Char;
    public bool CanWalk = true;
    public ShooterAdvancedCameraControl AdvGyro;
    Vector3 moveVector;
    [Header("Аудио")]
    public float WalkSndDelay = 0.2f;
    public AudioSource Audio;
    public AudioClip[] WalkSnd;
    [Space(20)]
    private Transform AdvancedCameraRoot;

    IEnumerator Start()
    {
        Char = GetComponent<CharacterController>();
        yield return new WaitForSeconds(0.5f);
        AdvancedCameraRoot = GameObject.Find("AdvancedCameraRoot").transform;
    }

    void Update()
    {
        if (Char.isGrounded == false)
            moveVector = Physics.gravity;
        Char.Move(moveVector * Time.deltaTime);

        if (Move || BackMove)
        {
            if (!BackMove)
                Char.Move(transform.forward * Speed * Time.deltaTime);
            if (BackMove)
                Char.Move(-transform.forward * Speed * Time.deltaTime);

            //Anim.SetBool("Walk", true);

            if (CanWalk)
            {
                Audio.PlayOneShot(WalkSnd[Random.Range(0, WalkSnd.Length)]);
                StartCoroutine(TakeWalkDelaySnd());
                CanWalk = false;
            }
        }
        else
        {
            //Anim.SetBool("Walk", false);
        }
    }
    
    IEnumerator TakeWalkDelaySnd()
    {
        yield return new WaitForSeconds(WalkSndDelay);
        CanWalk = true;
    }

    public void Moving()
    {
        if (!End)
            Move = true;
        BackMove = false;
    }
    public void MovingBack()
    {
        if (!End)
            BackMove = true;
    }
    public void Stop()
    {
        Move = false;
        BackMove = false;
    }
}