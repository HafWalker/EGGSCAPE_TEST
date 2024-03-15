using UnityEngine;

/// <summary>
/// OnSwordCollision Script
/// This script is assigned to the collider (Trigger) of the weapon
/// and is responsible for passing the data from the other collider to it
/// </summary>
public class OnSwordCollision : MonoBehaviour
{
    // Reference to the weapon to which this collider belongs
    public Sword sword;

    // OnTriggerEnter fires and calls the respective method of the weapon
    private void OnTriggerEnter(Collider other)
    {
        sword.OnCollision(other);
    }
}