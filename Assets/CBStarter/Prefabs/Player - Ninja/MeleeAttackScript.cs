using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if (collision.gameObject.tag == "Enemy")
        {
            // instantiate effect
            collision.gameObject.GetComponent<Enemy>().ApplyDamage(300);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * 300);
        }

    }


}
