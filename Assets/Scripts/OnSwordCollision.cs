using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSwordCollision : MonoBehaviour
{
    public Sword sword;
    
    private void OnTriggerEnter(Collider other)
    {
        sword.OnCollision(other);
    }
}