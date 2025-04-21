using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedkitItem : MonoBehaviour
{
    private PlayerHealth playerHealth;

    [SerializeField] int heal;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player")
        && other.GetType().ToString() == "UnityEngine.CapsuleCollider2D")
        {
            if(playerHealth != null)
            {
                playerHealth.HealPlayer(heal);
            }
            Destroy(gameObject);
        }
    }
}
