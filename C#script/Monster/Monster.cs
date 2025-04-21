using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int health,HealthMax;
    [SerializeField] int damage;
    public float flashTime=0.2f;
    public GameObject BloodEffect;
    public GameObject floatPoint;
    public GameObject dropMedkit;

    private SpriteRenderer sr;
    private Color oringinalColor;
    private PlayerHealth playerHealth;

    private Transform PlayerTransform;

    public MonsterHealthBar healthBar;
    // Start is called before the first frame update
    public void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        oringinalColor = sr.color;
        HealthMax = health;

        healthBar = GetComponentInChildren<MonsterHealthBar>();
    }
    // Update is called once per frame
    public void Update()
    {
        if(health <= 0)
        {
            int probability=UnityEngine.Random.Range(0,32);
            if(probability < 11)
            {
                Instantiate(dropMedkit, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        GameObject gb = Instantiate(floatPoint, transform.position, Quaternion.identity) as GameObject;
        gb.transform.GetChild(0).GetComponent<TextMesh>().text = damage.ToString();
        FlashColor(flashTime);
        Instantiate(BloodEffect, transform.position, Quaternion.identity);
        GameController.camShake.Shake2();
        healthBar.UpdateHealthBar(health, HealthMax);
    }
    public void FlashColor(float time)
    {
        sr.color = Color.red;
        Invoke("ResetColor",time); 
    }
    public void ResetColor()
    {
        sr.color = oringinalColor;
    }
    public void OnTriggerEnter2D(Collider2D trig) 
    {
        if (trig.gameObject.CompareTag("Player") && trig.GetType().ToString() == "UnityEngine.PolygonCollider2D")
        {
            if(playerHealth != null)
            {
                if(PlayerTransform.transform.position.x < transform.position.x)
                {
                    playerHealth.KnockBackFromRight = true;
                }
                else if(PlayerTransform.transform.position.x > transform.position.x)
                {
                    playerHealth.KnockBackFromRight = false;
                }
                playerHealth.MonsterDamagePlayer(damage);
            }
        }  
    }
}
