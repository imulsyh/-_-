using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Skeletons : Monster
{
    public float speedWalk;
    public float speedRun;
    public float radius;

    public float startWaitTime;
    private float waitTime;
    private bool Drop;
    private bool Alive = true;
    private bool DisLimit = true;
    public float Attacktime,AttackStartTime,CdTime,RebornTime;
    private bool canAttack = true;

    String[] str1=new String[] {"Attack1","Attack2"};

    public Transform movePos;
    public Transform leftDownPos;
    public Transform rightUpPos; 

    private PolygonCollider2D Polygon2D;
    private BoxCollider2D Box2D;

    private Transform playerTransform;
    private Animator anim;
    public Transform Pos;
    // Start is called before the first frame update
    new void Start()
    {
        Drop = false;
        base.Start();
        anim=GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Polygon2D=GetComponent<PolygonCollider2D>();
        Box2D=GetComponent<BoxCollider2D>();
        waitTime = startWaitTime;
        movePos.position = GetRandomPos();
    }

    // Update is called once per frame
    new void Update()
    {
        if(health <= 0 && Drop == false)
        {
            anim.SetTrigger("Die");
            Alive = false;
            Box2D.enabled = false;
            StartCoroutine(Reborn());
            int probability=UnityEngine.Random.Range(0,32);
            Drop = true;
            if(probability < 20)
            {
                Instantiate(dropMedkit, transform.position, Quaternion.identity);
            }
            
        }
        else if(health > 0 && Alive)
        {
            if (playerTransform != null)
            {
                float distance = (transform.position - playerTransform.position).sqrMagnitude;

                if(playerTransform.position.x > (leftDownPos.position.x) && playerTransform.position.x < (rightUpPos.position.x))
                {
                    DisLimit = true;
                }

                if(distance < radius && 3.5f < distance && DisLimit)
                {

                    if((transform.position.x - playerTransform.position.x) > 0f)
                    {
                        transform.localScale = new Vector2(-1f,1f);
                    }
                    else if((transform.position.x - playerTransform.position.x) < 0f)
                    {
                        transform.localScale = new Vector2(1f ,1f);
                    }
                    if(playerTransform.position.x >= (rightUpPos.position.x) | playerTransform.position.x <= (leftDownPos.position.x) )
                    {
                        DisLimit = false;
                    }
                    Pos.position = Vector2.MoveTowards(transform.position, playerTransform.position,speedRun * Time.deltaTime);
                    transform.position = new Vector2(Pos.position.x,transform.position.y);  
                    anim.SetBool("Run",true);
                    anim.SetBool("Walk",false);
                    anim.SetBool("Idle",false);
                }
                else if(distance > radius | DisLimit == false )
                {
                    anim.SetBool("Run",false);
                    anim.SetBool("Walk",true);

                    if((transform.position.x - movePos.position.x) > 0f)
                    {
                        transform.localScale = new Vector2(-1f,1f);
                    }
                    else if((transform.position.x - movePos.position.x) < 0f)
                    {
                        transform.localScale = new Vector2(1f ,1f);
                    }

                    transform.position = Vector2.MoveTowards(transform.position, movePos.position,speedWalk * Time.deltaTime);

                    if(Vector2.Distance(transform.position, movePos.position) < 0.1f)
                    {
                        if(waitTime <= 0)
                        {
                            anim.SetBool("Walk",true);
                            anim.SetBool("Idle",false);
                            movePos.position = GetRandomPos();
                            waitTime = startWaitTime;
                        }
                        else
                        {
                            {
                                anim.SetBool("Idle",true);
                                anim.SetBool("Walk",false);
                                waitTime -= Time.deltaTime;
                            }
                        }
                    }
                }
                else if(distance <= 3.5f)
                {
                    anim.SetBool("Idle",true);
                    anim.SetBool("Walk",false);
                    anim.SetBool("Run",false);
                    if(canAttack)
                    {
                        StartCoroutine(Wait());
                        int num=UnityEngine.Random.Range(0,2);
                        canAttack = false;
                        string str2=str1[num];
                        if(num==1)
                        {
                            anim.SetTrigger(str2);
                            StartCoroutine(StartAttack());
                            StartCoroutine(stopAttack());
                        }
                        else if(num==0)
                        {
                            anim.SetTrigger(str2);
                            StartCoroutine(StartAttack());
                            StartCoroutine(stopAttack());
                        }    
                    }
                
                
                }
            }
        }

        
    }

    public override void TakeDamage(int damage)
    {
        health -= damage;
        if(Alive)
        {
            anim.SetTrigger("Hit"); 
        }
        GameObject gb = Instantiate(floatPoint, transform.position, Quaternion.identity) as GameObject;
        gb.transform.GetChild(0).GetComponent<TextMesh>().text = damage.ToString();
        FlashColor(flashTime);
        Instantiate(BloodEffect, transform.position, Quaternion.identity);
        GameController.camShake.Shake2();
        healthBar.UpdateHealthBar(health, HealthMax);
    }

    IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(AttackStartTime);
        Polygon2D.enabled = true;
        StartCoroutine(disableHitBox());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
    }

    IEnumerator disableHitBox()
    {
        yield return new WaitForSeconds(Attacktime);
        Polygon2D.enabled = false;
    }


    IEnumerator stopAttack()
    {
        yield return new WaitForSeconds (CdTime);
        canAttack = true;
    }

    IEnumerator Reborn()
    {
        yield return new WaitForSeconds (RebornTime);
        anim.SetTrigger("Reborn");
        StartCoroutine(ColliderWake());
    }


    IEnumerator ASecond()
    {
        yield return new WaitForSeconds (0.5f);
    }

    IEnumerator ColliderWake()
    {
        yield return new WaitForSeconds (3f);
        Box2D.enabled = true;
        for(int i=1;i < 11;i++)
        {
            health = i ;
            healthBar.UpdateHealthBar(i, HealthMax);
            StartCoroutine(ASecond());
            if(i==10)
            {
                anim.SetBool("Idle",true);
                anim.SetBool("Walk",false);
                anim.SetBool("Run",false);
                Alive = true;
                Drop = false;
            }
        }
    }

    Vector2 GetRandomPos()
    {
        Vector2 rndPos = new Vector2(UnityEngine.Random.Range(leftDownPos.position.x, rightUpPos.position.x),UnityEngine.Random.Range(leftDownPos.position.y, rightUpPos.position.y));
        return rndPos;
    }

    void SkeletonOnTriggerEnter2D(Collider2D trig) 
    {
        base.OnTriggerEnter2D(trig);
    }
}
