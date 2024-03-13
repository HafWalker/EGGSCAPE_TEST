using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using System;

public class PlayerNetworkSync : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner) 
        { 
            GetComponent<PlayerNetworkSync>().enabled = false;
        }
    }

    [ServerRpc]
    public void AttackServer(PlayerController p, bool value, uint startTick)
    {
        Attack(p, value, startTick);
    }

    [ObserversRpc(ExcludeOwner = true)]
    public void Attack(PlayerController p, bool value, uint startTick)
    {
        float timeDiff = (float)(TimeManager.Tick - startTick) / TimeManager.TickRate;
        p.PerformAttackFromServer(p, value, timeDiff);
    }
}