using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickableItem : MonoBehaviour
{
    public bool FreezeAtStart;
    public bool UnParent;
    private Rigidbody Rigid;
    private AudioSource Audio;
    public AudioClip[] KickSound;

    void Start()
    {
        Audio = GetComponent<AudioSource>();
        Rigid = GetComponent<Rigidbody>();

        if (FreezeAtStart)
            Rigid.isKinematic = true;
    }

    public void Kick(Transform HitPos, float Force)
    {
        if (UnParent)
            transform.SetParent(null);

        Audio.PlayOneShot(KickSound[Random.RandomRange(0, KickSound.Length)]);
        Rigid.isKinematic = false;
        Rigid.AddTorque(transform.up * Force, ForceMode.Impulse);
        Rigid.AddTorque(HitPos.position * Force, ForceMode.Impulse);
        Rigid.AddForce(HitPos.position * Force, ForceMode.Impulse);
    }
}