using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int health;
    public int blinks;
    public float time;
    private bool hurt=false;
    private int Direction;
    public bool KnockBackFromRight;
    public float hitBoxCdTime;
    public float knockbackForce; // 新增的擊退力量
    public GameObject floatPoint;

    private Transform MonsterTransform;

    private Animator anim;
    private Renderer myRender;
    private ScreenFlash sf;
    private Rigidbody2D rb2d;
    private PolygonCollider2D polygonCollider2D;
    // Start is called before the first frame update
    void Start()
    {
        HealthBar.HealthMax = 20;
        HealthBar.HealthCurrent = 20;
        myRender = GetComponent<Renderer>();
        anim=GetComponent<Animator>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        sf = GetComponent<ScreenFlash>();
        rb2d = GetComponent<Rigidbody2D>();
        MonsterTransform = GameObject.FindGameObjectWithTag("Monster").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MonsterDamagePlayer(int damage)
    {
        if (hurt == false)
        {
            
            sf.FlashScreen();
            health -= damage;
            GameController.camShake.Shake();
            if(health < 0)
            {
                health = 0;
            }
            HealthBar.HealthCurrent = health;
            anim.SetTrigger("hurt");
            hurt=true;
            StartCoroutine (HurtTime());
            if(health > 0)
            {
                StartCoroutine (Knockback());
                StartCoroutine (SlowPlayerMove()); 
                StartCoroutine (KnockbackWaitTime()); 
            }
            
            if(health <= 0)
            {
                rb2d.velocity = new Vector2(0,0);
                GameController.isGameAlive = false;
                StartCoroutine (playerDieAnim());
            }
            BlinkPlayer(blinks,time);
            polygonCollider2D.enabled = false;
            StartCoroutine(ShowPlayerHitBox());

        }
    }

    public void DamagePlayer(int damage)
    {
        if (hurt == false)
        {
            
            sf.FlashScreen();
            health -= damage;
            GameController.camShake.Shake();
            if(health < 0)
            {
                health = 0;
            }
            HealthBar.HealthCurrent = health;
            anim.SetTrigger("hurt");
            hurt=true;
            StartCoroutine (HurtTime());
            
            if(health <= 0)
            {
                rb2d.velocity = new Vector2(0,0);
                GameController.isGameAlive = false;
                StartCoroutine (playerDieAnim());
            }
            BlinkPlayer(blinks,time);
            polygonCollider2D.enabled = false;
            StartCoroutine(ShowPlayerHitBox());

        }
    }

    public void FlyMonsterDamagePlayer(int damage)
    {
        if (hurt == false)
        {
            
            sf.FlashScreen();
            health -= damage;
            GameController.camShake.Shake();
            if(health < 0)
            {
                health = 0;
            }
            HealthBar.HealthCurrent = health;
            anim.SetTrigger("hurt");
            hurt=true;
            StartCoroutine (HurtTime());

            if(health > 0)
            {
                StartCoroutine (SlowPlayerMove()); 
            }
            
            
            if(health <= 0)
            {
                rb2d.velocity = new Vector2(0,0);
                GameController.isGameAlive = false;
                StartCoroutine (playerDieAnim());
            }
            BlinkPlayer(blinks,time);
            polygonCollider2D.enabled = false;
            StartCoroutine(ShowPlayerHitBox());
        }
    }

    public void HealPlayer (int heal)
    {
        health = health + heal;
        GameObject gb = Instantiate(floatPoint, transform.position, Quaternion.identity) as GameObject;
        gb.transform.GetChild(0).GetComponent<TextMesh>().text = heal.ToString();
        if(health > 20)
        {
            health = 20;
        }
        HealthBar.HealthCurrent = health;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("DeadLine"))
        {
            health = 0;
            HealthBar.HealthCurrent = health;
            GameController.isGameAlive = false;
            Time.timeScale = 0f;
        }
    }


    IEnumerator ShowPlayerHitBox()
    {
        yield return  new WaitForSeconds (hitBoxCdTime);
        polygonCollider2D.enabled = true;
    }
    void BlinkPlayer(int numBlinks,float seconds)
    {
        StartCoroutine(DoBlinks(numBlinks,seconds));
    }

    IEnumerator DoBlinks(int numBlinks,float seconds)
    {
        for(int i=0; i < numBlinks * 2;i++)
        {
            myRender.enabled = !myRender.enabled;
            yield return new WaitForSeconds(seconds);
        }
        myRender.enabled = true;
    }
    IEnumerator playerDieAnim()
    {
        anim.SetBool("Die",true);
        yield return new WaitForSeconds(0.9f);
        Destroy(gameObject);
        Time.timeScale = 0f;
    }
    IEnumerator HurtTime()
    {
        yield return new WaitForSeconds(1.5f);
        hurt=false;
    }
    IEnumerator Knockback()
    {
        if(KnockBackFromRight)
        {
            Direction = -1;
        }
        else if(KnockBackFromRight == false)
        {
            Direction = 1;
        }
        rb2d.velocity = new Vector2(0,0);
        rb2d.velocity = new Vector2(Direction * knockbackForce, rb2d.velocity.y);
        yield return new WaitForSeconds(0.2f);
        rb2d.velocity = new Vector2(0,0);
    }
    IEnumerator KnockbackWaitTime()
    {
        GameController.isGameAlive = false;
        yield return new WaitForSeconds(0.4f);
        GameController.isGameAlive = true;
    }
    IEnumerator SlowPlayerMove()
    {
        anim.SetBool("KnockBack",true);
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("KnockBack",false);
    }
}
