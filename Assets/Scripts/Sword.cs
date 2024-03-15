using UnityEngine;

/// <summary>
///Sword Script
/// This script is the one assigned to the mele weapon
/// Contains its damage and attack methods / Collision
/// They also contain a reference to the animator
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

    // Method to carry out the local attack
    public void Attack(bool value)
    {
        // The animator flag is set
        animator.SetBool("Attack", value);
    }

    // Method to replicate the synchronized attack on clients
    public void AttackPredict(bool value, float delayTime)
    {
        // The animator bool is set
        animator.SetBool("Attack", value);
        if (value)
        {
            // In case the attack is being carried out (Not canceling), the start of the animation is calculated based on the delay defined by the server
            animator.Play("SwordAttack", -1, delayTime / animator.GetCurrentAnimatorClipInfo(0).Length);
        }
    }

    // Method that receives the Collider that the Trigger encountered
    public void OnCollision(Collider col)
    {
        // Verify that the Collider implements the Idamageable interface
        if (col.gameObject.GetComponent<IDamageable>() != null)
        {
            // If yes, call its respective method "TakeDamage" passing a reference of itself
            col.gameObject.GetComponent<IDamageable>().Takedamage(gameObject, damage);
        }
    }

    #endregion
}