using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IActorTemplate
{
    [SerializeField]
    float speed;
    int health;
    int hitPower;
    GameObject actor;
    [SerializeField]
    GameObject bullet;
    SOActorModel actorModel;
    public GameManager gameManager;
    public SpawnManager spawnManager;

    float hBound = 11f;
    float lowBound = -6f;
    private AudioSource enemyAudio;
    public AudioClip takeDamage, sendBullet;

    bool isFiring = false, dead = false; 
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public int HitPower
    {
        get { return hitPower; }
        set { hitPower = value; }
    }
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    public GameObject Bullet
    {
        get { return bullet; }
        set { bullet = value; }
    }

    void Start()
    {
        enemyAudio = GetComponent<AudioSource>();
    }
    void Update()
    {

    }
    private void FixedUpdate()
    {
        Debug.Log("Starting fire");
        if (!isFiring)
            StartCoroutine(Fire());
        Move();
        if (gameManager.isActive == false)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        if (dead == false)
        {
            dead = true;
            spawnManager.spawnedEnemies--;
            gameManager.enemiesRemaining--;
            gameManager.UpdateRemainingEnemies();
            gameManager.DeathExplosion(gameObject);
            Destroy(gameObject);
            
        }
        
    }
    IEnumerator Fire()
    {
        isFiring = true;
        while (isFiring)
        {
            yield return new WaitForSeconds(0.8f);
            Vector3 offset = new Vector3(0, 1, -2);
            Instantiate(bullet, transform.position + offset, bullet.transform.rotation);
            bullet.GetComponent<BulletBehaviour>().hitDamage = HitPower;
            enemyAudio.PlayOneShot(sendBullet, 0.05f);
        }
    }
    void Move()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        KeepInBounds();
        if (transform.position.z <= lowBound)
            gameManager.GameOver();
    }

    void KeepInBounds()
    {
        if (transform.position.x >= hBound)
        {
            transform.Translate(Vector3.right);
            transform.eulerAngles = new Vector3(0, -90);
        }
        if (transform.position.x <= -hBound)
        {
            transform.Translate(Vector3.left);
            transform.eulerAngles = new Vector3(0, 90);
        }
    }
    public void TakeDamage(int incomingDamage)
    {
        if (health <= 0)
            Die();
        else
        {
            health -= incomingDamage;
            enemyAudio.PlayOneShot(takeDamage, 0.1f);
        }
    
    }

    public int DealDamage()
    {
        return hitPower;
    }

    public void ActorStats(SOActorModel actorModel)
    {
        speed = actorModel.speed;
        health = actorModel.health;
        hitPower = actorModel.hitPower;

        actor = actorModel.actor;
        bullet = actorModel.actorBullet;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Powerup")
        {
            Debug.Log("Attempting collision ignoring");
            Physics.IgnoreCollision(collision.gameObject.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), true);
        }
    }
}
