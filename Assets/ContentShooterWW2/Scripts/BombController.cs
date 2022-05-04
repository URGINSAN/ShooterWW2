using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public bool StartBomb;
    private bool CanStartBomb = true;
    public float Timer = 10;
    private float Radius;
    [Space(20)]
    public Animator Anim;
    public GameObject Fire;
    public GameObject Explosion;

    private Collider[] _colliders;
    private void Update()
    {
        if (StartBomb && CanStartBomb)
        {
            StartBombFunc();
            CanStartBomb = false;
        }

        if (StartBomb)
        {
            if (Timer > 0)
            {
                Timer -= 1 * Time.deltaTime;
            }
            if (Timer <= 0)
            {
                Explode();
            }
        }
    }

    void StartBombFunc()
    {
        Fire.SetActive(true);
        Anim.speed = Anim.speed / Timer;
        Anim.Play("BombGo");
    }

    public void Explode()
    {
        Explosion.transform.SetParent(null);
        Explosion.SetActive(true);
        /*
        if (GetComponentInChildren<DetonatorForce>() != null)
            Radius = GetComponentInChildren<DetonatorForce>().radius;
        _colliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (Collider hit in _colliders)
        {
            if (!hit)
            {
                continue;
            }

            if (hit.GetComponent<BombController>())
            {
                hit.GetComponent<BombController>().Explode();
            }
        }*/

        Destroy(gameObject);
    }
}