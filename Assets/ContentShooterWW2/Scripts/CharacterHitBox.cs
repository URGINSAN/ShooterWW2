using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitBox : MonoBehaviour
{
    public enum Type
    {
        Enemy = 0
    }
    public Type type;
    public float DamageFactor;

    public void Damage(float Damage)
    {
        switch (type)
        {
            case Type.Enemy:
                if (GetComponentInParent<AIHealthController>() != null)
                    GetComponentInParent<AIHealthController>().Health -= Damage * DamageFactor;
                break;
        }
    }
}