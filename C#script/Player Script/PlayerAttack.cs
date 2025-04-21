using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttack : Player
{
    public int damage;
    public float Attacktime,AttackStartTime;
    public int Magic = 0,MagicMax = 6;
    public float speed;


    public GameObject sickle; 

    String[] str1=new String[] {"groundAttack","groundAttack2","groundAttack3"};
    [SerializeField] MagicBar MGBar;
    //private Animator anim;
    private Transform playerTransform;
    private Rigidbody2D weapon;
    public bool canAttack=true;
    // Start is called before the first frame update
    void Start()
    {
        weapon = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Rigidbody2D>();
        anim=GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Polygon2D=GetComponent<PolygonCollider2D>();

        MGBar = GetComponent<MagicBar>();
        MGBar.UpdateMagicBar(Magic, MagicMax);

    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.isGameAlive)
        {
            Attack();
            PointCheck();
        }

    }
    void Attack()
    {
        if(Input.GetButtonDown("Fire1") && touchGround && canAttack && anim.GetBool("Crouch") == false && anim.GetBool("Die") == false && anim.GetBool("Slide") == false && GameController.isGameAlive)
        {
            int num=UnityEngine.Random.Range(0,3);
            string str2=str1[num];
            if(num==2)
            {
                anim.SetTrigger(str2);
                StartCoroutine(StartAttack());
                canAttack=false;
                StartCoroutine(stopAttack());
            }
            else if(num==1)
            {
                anim.SetTrigger(str2);
                StartCoroutine(StartAttack());
                canAttack=false;
                StartCoroutine(stopAttack());
            }
            else if(num==0)
            {
                anim.SetTrigger(str2);
                StartCoroutine(StartAttack());
                canAttack=false;
                StartCoroutine(stopAttack());
            }
        }
        if(Input.GetButtonDown("Fire1") && touchGround == false  && anim.GetBool("Die") == false && canAttack && GameController.isGameAlive)
        {
            anim.SetTrigger("AirAttack");
            anim.SetBool("AirAttackTrig",true);
            StartCoroutine(StarAirtAttack());
            canAttack=false;
            StartCoroutine(stopAttack());
        }


        if(Input.GetButtonDown("Fire2") && anim.GetBool("Die") == false && GameController.isGameAlive && Magic >= MagicMax )
        {
            anim.SetTrigger("MagicAttack");
            anim.SetBool("AirAttackTrig",true);
            StartCoroutine(stopAttack());
            
            Magic = 0;
            MGBar.UpdateMagicBar(Magic, MagicMax);
            if(playerTransform.transform.localScale.x > 0)
            {
                Instantiate(sickle, transform.position, Quaternion.Euler(0, 0, -180));
            }
            else if(playerTransform.transform.localScale.x < 0)
            {
                Instantiate(sickle, transform.position, Quaternion.Euler(0, 0, 0 ));
            } 
            
        }
        //SwordSlashAttack
    }
   IEnumerator stopAttack()
    {
        yield return new WaitForSeconds (0.5f);
        canAttack=true;
        anim.SetBool("AirAttackTrig",false);
    }

    IEnumerator disableHitBox()
    {
        yield return new WaitForSeconds(Attacktime);
        Polygon2D.enabled = false;
    }
    IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(AttackStartTime);
        Polygon2D.enabled = true;
        StartCoroutine(disableHitBox());
    }
    IEnumerator StarAirtAttack()
    {
        yield return new WaitForSeconds(0f);
        Polygon2D.enabled = true;
        StartCoroutine(disableHitBox());
    }
    void PointCheck()
    {
        touchGround=Physics2D.OverlapCircle(footPoint.position,0.3f,LayerMask.GetMask("Ground & Wall") | LayerMask.GetMask("Platform") | LayerMask.GetMask("monster") );
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Monster"))
        {
            other.GetComponent<Monster>().TakeDamage(damage);
            Magic = Magic + 1 ;
            MGBar.UpdateMagicBar(Magic, MagicMax);
        }
        else if(other.gameObject.CompareTag("FlyMonster"))
        {
            other.GetComponent<FlyMonster>().TakeDamage(damage);
            Magic = Magic + 1 ;
            MGBar.UpdateMagicBar(Magic, MagicMax);
        }
    }

}
