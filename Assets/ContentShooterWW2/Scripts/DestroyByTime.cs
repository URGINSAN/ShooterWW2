using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    public float DestroyTime = 5;

    void Start()
    {
        Destroy(gameObject, DestroyTime);    
    }
}