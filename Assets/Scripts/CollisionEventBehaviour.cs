using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEventBehaviour : MonoBehaviour
{
    public event Action<Collision, Transform> OnCollisionEnterEvent;

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterEvent?.Invoke(collision, transform);
    }
}
