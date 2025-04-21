using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.parent.localScale.x > 0)
        {
            transform.localScale = new Vector2(1f,1f); 
        }
        else if(transform.parent.localScale.x < 0)
        {
            transform.localScale = new Vector2(-1f,1f); 
        }
    }
}
