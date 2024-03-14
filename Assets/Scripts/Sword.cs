using UnityEngine;

/// <summary>
/// Sword Scrip
/// Este script es el que se asigna al arma mele 
/// Contiene su damage y metodos de ataque / Colision
/// Tambien conteien una referencia al animator
/// </summary>
public class Sword : MonoBehaviour
{
    #region REFERENCES

    [SerializeField]
    private float damage = 2f;

    [SerializeField]
    public Animator animator;

    #endregion

    #region METHODS

    // Metodo para efectuar el ataque local
    public void Attack(bool value)
    {
        // Se seteal el flag del animator
        animator.SetBool("Attack", value);
    }

    // Metodo apra replicar el ataque sincronizado en los clientes
    public void AttackPredict(bool value, float delayTime)
    {
        // Se setea el bool del animator
        animator.SetBool("Attack", value);
        if (value)
        {
            // En caso de que el ataque se este efectuando (No cancelando), se calcula el inicio de la animacion en funcion del delay definido por el server
            animator.Play("SwordAttack", -1, delayTime / animator.GetCurrentAnimatorClipInfo(0).Length);
        }
    }

    // Metodo que recibe el Collider con el que se encontro el Trigger
    public void OnCollision(Collider col)
    {
        // Verifico que el Collider implementa la interfaz IDamageable
        if (col.gameObject.GetComponent<IDamageable>() != null)
        {
            // En caso de que si, llamo a su respectivo metodo "TakeDamage" pasando un referencia de si misma 
            col.gameObject.GetComponent<IDamageable>().Takedamage(gameObject, damage);
        }
    }

    #endregion
}