using UnityEngine;

/// <summary>
/// IDamageable interface
/// Required for any item that takes damage
/// In this case there is only one element, a player
/// </summary>
public interface IDamageable
{
    // To avoid using GetComponent I am sending the damage parameter along with the gameObject
    void Takedamage(GameObject gameObject, float value);
}
