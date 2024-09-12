using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public int hitDamage;
    void Update()
    {
        MoveForward();
    }

    void MoveForward()
    {
        if (gameObject.tag == "PBullet")
            transform.Translate(Vector3.forward * Time.deltaTime * 50);
        if (gameObject.tag == "EBullet")
            transform.Translate(Vector3.back * Time.deltaTime * 10);
        if (transform.position.z > 20 || transform.position.z < -20)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.tag == "EBullet" && collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit");
            Destroy(gameObject);
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(hitDamage);
        }
        else if (gameObject.tag == "PBullet" && collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit");
            Destroy(gameObject);
            collision.gameObject.GetComponent<EnemyController>().TakeDamage(hitDamage);
        }
        else if (gameObject.tag == "PBullet" && collision.gameObject.tag == "EBullet")
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
        else
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>(), true);

    }
}
