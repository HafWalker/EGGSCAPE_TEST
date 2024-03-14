using FishNet.Object;

/// <summary>
/// PlayerNetworkSync Scrip
/// Contiene los metodo de comunicacion entre el Cliente y el Servidor
/// (Encontre esta manera de manejar las llamadas en algun video sobre la implementacion de FishNet
/// y me parecio el mas prolijo)
/// </summary>
public class PlayerNetworkSync : NetworkBehaviour
{
    public override void OnStartClient()
    {
        // De la misma manera que se trabaja en el PlayerController
        // Desactivo el scrip en caso de no perteneces al Owner del Cliente
        base.OnStartClient();
        if (!base.IsOwner) 
        { 
            GetComponent<PlayerNetworkSync>().enabled = false;
        }
    }

    // Aviso al servidor sobre un ataque realizado o no con su respectivo tick de ejecucion
    [ServerRpc]
    public void AttackServer(PlayerController p, bool value, uint startTick)
    {
        Attack(p, value, startTick);
    }

    // Llamada a los cliente para replicar el estado de un ataque con su respectivo Tick de ejecucion
    [ObserversRpc(ExcludeOwner = true)]
    public void Attack(PlayerController p, bool value, uint startTick)
    {
        // Calculo de diferencia entre el tiempo de ejecucion del ataque y su llegada al servidor
        // El valor se utiliza para predecir el tiempo que deben adelantar el ataque los clientes
        float timeDiff = (float)(TimeManager.Tick - startTick) / TimeManager.TickRate;
        p.PerformAttackFromServer(p, value, timeDiff);
    }

    [ServerRpc]
    public void UpdateHealth(PlayerController p, float value)
    {
        p.UpdatePlayerHealth(value);
        UpdateHealthInOthers(p,value);
    }

    [ObserversRpc(ExcludeOwner = true)]
    public void UpdateHealthInOthers(PlayerController p, float value) 
    { 
        p.UpdatePlayerHealth(value);
    }

}