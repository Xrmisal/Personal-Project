using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActorTemplate
{
    // Template to set standard functions for actors such as the player and enemies.
    void Die();  

    void TakeDamage(int incomingDamage);

    int DealDamage();

    void ActorStats(SOActorModel actorModel); // SOActorModel is where statistics for actors are defined.




}
