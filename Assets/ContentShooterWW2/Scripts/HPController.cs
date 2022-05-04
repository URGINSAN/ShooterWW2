using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPController : MonoBehaviour
{
    public float Health = 100;
    public bool CanDeath = true;
    [Space(20)]
    public bool CanPartiallyDamaged;
    public float PartiallyDamagedHealth = 50;
    [Space(20)]
    public GameObject PartiallyDamagedGO;
    public GameObject[] ToDestroy;
    public GameObject[] DestroyedGO;

    private void Update()
    {
        if (Health < 0 && CanDeath)
        {
            Death();
            CanDeath = false;
        }

        if (Health < PartiallyDamagedHealth && !PartiallyDamagedGO.activeInHierarchy)
        {
            PartiallyDamagedGO.SetActive(true);
        }
    }
    void Death()
    {
        if (DestroyedGO[0] != null)
        {
            for (int i = 0; i < DestroyedGO.Length; i++)
            {
                DestroyedGO[i].transform.SetParent(null);
                DestroyedGO[i].SetActive(true);
            }

            for (int i = 0; i < ToDestroy.Length; i++)
                Destroy(ToDestroy[i]);
        }
        else
        {
            enabled = false;
        }
    }

    public void PainFromExplosion(Transform Explosion)
    {
        float Dist = Vector3.Distance(transform.position, Explosion.position);
        Health -= Mathf.InverseLerp(10, 0, Dist) * 1000;
    }
}