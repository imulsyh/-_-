using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    public float moveSpeed=6f,jumpSpeed=12f,slideSpeed=2f;
    private Rigidbody2D rb2d;
    public bool touchGround=true,canJump=true,canSlide=true,canAirAttack=true;
    public Transform footPoint;
    public PolygonCollider2D Polygon2D;
    public Animator anim;
    CapsuleCollider2D coll2D;

    void Animation()
    {
        if(Input.GetAxis("Horizontal") != 0) 
        {
            anim.SetBool("Run",true);
        }
        else
        {
            anim.SetBool("Run",false);
        }
        if(Input.GetButtonDown("Shift") && Input.GetAxis("Horizontal")==0)
        {
            anim.SetBool("Crouch",true);
            coll2D.offset= new Vector2(0.01152535f,-0.0722235f);
            coll2D.size= new Vector2(0.1505144f,0.2100782f);
        }
        else if(Input.GetButtonUp("Shift") && Input.GetAxis("Horizontal")==0)
        {
            anim.SetBool("Crouch",false);
            coll2D.offset= new Vector2(-0.007168535f,-0.02970124f);
            coll2D.size= new Vector2(0.1294472f,0.2951235f);
        }
        if(Input.GetButtonDown("Shift") && Input.GetAxis("Horizontal")!=0 && touchGround && canSlide && anim.GetBool("KnockBack") == false)
        {
            performSlide();
        }
    }    
    void performSlide()
    {
        if(GameController.isGameAlive && anim.GetBool("KnockBack") == false)
        {
            anim.SetBool("Slide",true);
            if(Input.GetAxis("Horizontal")>0)
            {
                rb2d.AddForce (Vector2.right * slideSpeed);
                rb2d.mass=0.001f;
                moveSpeed=8f;
            }
            else if(Input.GetAxis("Horizontal")<0)
            {
                rb2d.AddForce (Vector2.left * slideSpeed);
                rb2d.mass=0.001f;
                moveSpeed=8f;
            }
            coll2D.offset= new Vector2(-0.03587767f,-0.09597211f);
            coll2D.size= new Vector2(0.1852191f,0.1573012f);
            canSlide=false;
            StartCoroutine ("stopSlide");
        }
    }
    IEnumerator stopSlide()
    {
        yield return new WaitForSeconds (0.8f);
        anim.SetBool ("Slide",false);
        coll2D.offset= new Vector2(-0.0004247498f,-0.02970124f);
        coll2D.size= new Vector2(0.1618349f,0.2951235f);
        canSlide=true;
        rb2d.mass=1;
        moveSpeed=6f;
    }
    void SwitchAnimation()
    {
        anim.SetBool("Idel",false);
        if(anim.GetBool("Jump"))
        {
           if(rb2d.velocity.y < 0.0f)
           {
                anim.SetBool("Jump",false);
                anim.SetBool("Fall",true);
           }
        }
        else if(touchGround)
        {
            anim.SetBool("Fall",false);
            anim.SetBool("Idel",true);
        }
        if(anim.GetBool("Double"))
        {
           if(rb2d.velocity.y < 0.0f)
           {
                anim.SetBool("Double",false);
                anim.SetBool("Fall",true);
           }
        }
        else if(rb2d.velocity.y < 0.0f && touchGround==false)
        {
            anim.SetBool("Fall",true);
            anim.SetBool("Slide",false);
        }
    }
    void Move()
    {
        float key=0.0f;
        rb2d.velocity = new Vector2(moveSpeed*Input.GetAxis("Horizontal"),rb2d.velocity.y);
        if(anim.GetBool("Die")==false)
        {
            if(Input.GetAxis("Horizontal")<0) 
            {
                key=-5.068677f;
            }
            else if(Input.GetAxis("Horizontal")>0) 
            {
                key=5.068677f;
            }
            if(key!=0)
            {
                transform.localScale = new Vector2(key,5.998244f);
            }
        }
    }
    void Jump()
    {
        if(Input.GetButtonDown("Jump") && touchGround)
        {
            anim.SetBool("Jump",true);
            Vector2 jumpVel = new Vector2(0.0f,jumpSpeed);
            rb2d.velocity = Vector2.up * jumpVel;
        }
        else if(Input.GetButtonDown("Jump") && canJump && touchGround==false)
        {
            anim.SetBool("Double",true);
            anim.SetBool("Jump",false);
            anim.SetBool("Fall",false);
            Vector2 jumpVel = new Vector2(0.0f,jumpSpeed);
            rb2d.velocity = Vector2.up * jumpVel;
            canJump=false;
        }
    }
    void PointCheck()
    {
        touchGround=Physics2D.OverlapCircle(footPoint.position,0.3f,LayerMask.GetMask("Ground & Wall") | LayerMask.GetMask("Platform") | LayerMask.GetMask("monster")  );
        if(touchGround==true) canJump = true;
    }
    void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Ground")
        {
           if(other.contacts[0].normal != new Vector2(0f,1f))
           {
                rb2d.angularDrag = 0.0f;
           }
           else 
           {
                rb2d.angularDrag = 0.05f;
           }
        }
    }
    void Start()
    {
        rb2d=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        coll2D=GetComponent<CapsuleCollider2D>();
        Polygon2D=GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.isGameAlive)
        {
            Jump(); 
            PointCheck();
            Animation();
            SwitchAnimation();
            Move();  
            
        }
        if(anim.GetBool("KnockBack"))
        {
            moveSpeed = 3f;
        }            
        else if(anim.GetBool("Slide") == false)
        {
            moveSpeed = 6f;
        }
    }
}
