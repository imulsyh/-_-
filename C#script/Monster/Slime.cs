using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Slime : Monster
{
    public float speed;
    public Animator anim;
    public bool MoveRight;
    public bool Animation=false;
    public float WaitTime;
    //public int health;
    //public int damage;
   
    void Move()
    {
        
    }
    // Start is called before the first frame update
    new void Start()
    {
       anim=GetComponent<Animator>();
       base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        if(MoveRight && speed!=0)
        {
            transform.Translate(2 * Time.deltaTime * speed , 0,0);
            transform.localScale= new Vector2(1.3f,1.3f);
            anim.SetBool("Move",true);
            anim.SetBool("idle",false);
        }
        else if(MoveRight==false && speed!=0)
        {
            transform.Translate(-2 * Time.deltaTime * speed , 0,0);
            transform.localScale= new Vector2(-1.3f,1.3f);
            anim.SetBool("Move",true);
            anim.SetBool("idle",false);
        }
        base.Update();
    }
    new void OnTriggerEnter2D(Collider2D trig) 
    {
        if(trig.gameObject.CompareTag("turn"))
        {
            if(MoveRight)
            {
                speed=0;
                anim.SetBool("Move",false);
                anim.SetBool("idle",true);
                StartCoroutine(MoveLeft());
            }
            else 
            {
                speed=0;
                anim.SetBool("Move",false);
                anim.SetBool("idle",true);
                StartCoroutine(MoveRight1());
            }
        }
        base.OnTriggerEnter2D(trig);
    }
    IEnumerator MoveLeft()
    {
        yield return new WaitForSeconds (WaitTime);
        MoveRight = false;
        speed=1.2f;
    }
    IEnumerator MoveRight1()
    {
        yield return new WaitForSeconds (WaitTime);
        MoveRight = true;
        speed=1.2f;
    }
}
