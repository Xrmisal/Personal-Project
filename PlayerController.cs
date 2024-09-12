using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IActorTemplate
{
    public ParticleSystem dirtSplatter;

    [SerializeField]
    float speed;
    int health;
    int hitPower;
    GameObject actor;
    GameObject bullet;
    SOActorModel actorModel;

    float hInput;
    float vInput;
    float hBound = 11f;
    float lowBound = -6f;
    float topBound = 7f;
    public int maxHealth;
    public GameObject powerupIndicator;
    public GameManager gameManager;
    public AudioClip fire, takeDamage;
    private AudioSource playerAudio;
    float fireTimer = 0.12f;
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
        playerAudio = GetComponent<AudioSource>();
    //    GameObject.Find("Player").GetComponent<IActorTemplate>().ActorStats(actorModel);
    }

    void FixedUpdate()
    {
        if (fireTimer > 0)
          fireTimer -= Time.deltaTime;
        Move();
        Fire();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            gameManager.PauseGame();
    }

    void Move()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * speed * vInput);
        transform.Translate(Vector3.right * Time.deltaTime * speed * hInput);

        // SetDirtRotation();
        KeepInBounds();
    }
    void Fire()
    {
        Vector3 offset = new Vector3 (0, 1, 2);
        float maxTimer = 0.12f;
        if (Input.GetKey(KeyCode.Space) && fireTimer <= 0)
        {
            Instantiate(bullet, transform.position + offset, bullet.transform.rotation);
            playerAudio.PlayOneShot(fire, 0.1f); 
            bullet.GetComponent<BulletBehaviour>().hitDamage = hitPower;
            fireTimer = maxTimer;
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        gameManager.GameOver();
    }
    public void TakeDamage(int incomingDamage)
    {
        health -= incomingDamage;
        gameManager.AlterHealth();
        playerAudio.PlayOneShot(takeDamage, 0.1f);
        if (health <= 0)
            Die();
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

        maxHealth = health;
    }
    void SetDirtRotation()
    {
        if (vInput == 0 && hInput == 0)
            dirtSplatter.Play();

        if (vInput > 0)
            dirtSplatter.transform.eulerAngles = new Vector3(0, 90);
        else if (vInput < 0)
            dirtSplatter.transform.eulerAngles = new Vector3(0, -90);
        else if (hInput > 0)
            dirtSplatter.transform.eulerAngles = new Vector3(0, 180);
        else if (hInput < 0)
            dirtSplatter.transform.eulerAngles = new Vector3(0, 0);

        if (vInput > 0 && hInput > 0)
            dirtSplatter.transform.eulerAngles = new Vector3(0, 135);
        else if (vInput < 0 && hInput > 0)
            dirtSplatter.transform.eulerAngles = new Vector3(0, 225);
        else if (vInput < 0 && hInput < 0)
            dirtSplatter.transform.eulerAngles = new Vector3(0, 315);
        else if (vInput > 0 && hInput < 0)
            dirtSplatter.transform.eulerAngles = new Vector3(0, 45);

    }
    void KeepInBounds()
    {
        if (transform.position.x > hBound)
            transform.position = new Vector3(hBound, transform.position.y, transform.position.z);
        if (transform.position.x < -hBound)
            transform.position = new Vector3(-hBound, transform.position.y, transform.position.z);
        if (transform.position.z > topBound)
            transform.position = new Vector3(transform.position.x, transform.position.y, topBound);
        if (transform.position.z < lowBound)
            transform.position = new Vector3(transform.position.x, transform.position.y, lowBound);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case ("Enemy"):
                TakeDamage(hitPower);
                break;
            case ("Powerup"):
                Destroy(collision.gameObject);
                gameManager.ApplyPowerup(collision.gameObject.name);
                break;
            case ("PBullet"):
                Physics.IgnoreCollision(collision.gameObject.GetComponent<SphereCollider>(), GetComponent<BoxCollider>(), true);
                break;
        }
    }
}
