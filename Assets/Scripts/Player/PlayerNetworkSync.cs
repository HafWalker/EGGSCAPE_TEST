using FishNet.Object;

/// <summary>
/// PlayerNetworkSync Scrip
/// Contains the communication methods between the Client and the Server
/// (I found this way of handling calls in some video about FishNet implementation
/// and it seemed like the most verbose to me)
/// </summary>
public class PlayerNetworkSync : NetworkBehaviour
{
    public override void OnStartClient()
    {
        // In the same way as working in the PlayerController
        // I deactivate the script if you do not belong to the Client Owner
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<PlayerNetworkSync>().enabled = false;
        }
    }

    // Notification to the server about an attack carried out or not with its respective execution tick
    [ServerRpc]
    public void AttackServer(PlayerController p, bool value, uint startTick)
    {
        Attack(p, value, startTick);
    }

    // Call to clients to replicate the state of an attack with its respective execution Tick
    // This method excludes the Owner of this client
    [ObserversRpc(ExcludeOwner = true)]
    public void Attack(PlayerController p, bool value, uint startTick)
    {
        // Calculation of the difference between the execution time of the attack and its arrival at the server
        // The value is used to predict how long clients should advance the attack
        float timeDiff = (float)(TimeManager.Tick - startTick) / TimeManager.TickRate;
        p.PerformAttackFromServer(p, value, timeDiff);
    }

    // Method to update the Player's life on the server
    // The Health variable is synchronized on the server when modified locally
    // But it is necessary to replicate the change on the clients
    [ServerRpc]
    public void UpdateHealth(PlayerController p, float value)
    {
        // The change in this player's life is replicated in the rest of the Clients
        p.currentHealth = value;
    }
  
}