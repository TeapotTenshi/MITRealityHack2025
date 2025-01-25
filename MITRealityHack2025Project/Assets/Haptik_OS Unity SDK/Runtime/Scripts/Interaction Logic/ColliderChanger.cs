using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChanger : MonoBehaviour
{
    [SerializeField] Collider[] colliders = new Collider[2];

    // Start is called before the first frame update
    void Start()
    {
        colliders = GetComponents<Collider>();

    }

    public void ChangeCollider()
    {
        if (colliders[0].enabled)
        {
            colliders[0].enabled = false;
            colliders[1].enabled = true;
        }
        else
        {
            colliders[0].enabled = true;
            colliders[1].enabled = false;
        }
    }
}
