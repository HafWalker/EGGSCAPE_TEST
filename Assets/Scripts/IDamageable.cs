using UnityEngine;

/// <summary>
/// Interfaz IDamageable 
/// Necesaria para cualquier elemento que reciba daño
/// En este caso solo hay un elemento, un jugador
/// </summary>
public interface IDamageable
{
    //  Para evitar utilizar GetComponent estoy enviando el parametro de daño junto con el gameObject
    void Takedamage(GameObject gameObject, float value);
}
