using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    public int damage;
    public float MaxDistance;
    public float speed;

    private Rigidbody2D rb2d;
    private Transform playerTransform;
    private Transform sickleTransform;
    private Vector2 startSpeed;

    private CameraShake camShake;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        sickleTransform = GetComponent<Transform>();
        camShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(transform.position.x - playerTransform.position.x) > MaxDistance )
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Monster"))
        {
            other.GetComponent<Monster>().TakeDamage(damage);
        }
        else if(other.gameObject.CompareTag("FlyMonster"))
        {
            other.GetComponent<FlyMonster>().TakeDamage(damage);
        }
    }
}
