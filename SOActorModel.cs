using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Actor", menuName = "Create Actor")]
public class SOActorModel : ScriptableObject
{
    public string actorName;
    public enum ActorType
    {
        Player,
        SmallEnemy,
        MediumEnemy,
        LargeEnemy,
        BossEnemy
    }
    public ActorType actorType;

    public string description;
    public int health;
    public float speed;
    public int hitPower;
    public GameObject actor;
    public GameObject actorBullet;


}
