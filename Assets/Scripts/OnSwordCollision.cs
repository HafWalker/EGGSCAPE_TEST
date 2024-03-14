using UnityEngine;

/// <summary>
/// OnSwordCollision Scrip
/// Este scrip se asigna al collider (Trigger) del arma 
/// y se encarga de pasar el dato del otro collider a la misma
/// </summary>
public class OnSwordCollision : MonoBehaviour
{
    // Referencia al arma a la que perteneces este collider
    public Sword sword;
    
    // OnTriggerEnter se dispara y llama al respectivo metodo de su arma
    private void OnTriggerEnter(Collider other)
    {
        sword.OnCollision(other);
    }
}