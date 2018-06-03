using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardFireball : MonoBehaviour {

    [SerializeField] float speed = 20;
    [SerializeField] int dmg = 200;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //transform.Translate(transform.right * Time.deltaTime * speed);
        transform.position += transform.right * Time.deltaTime * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
            collision.gameObject.GetComponent<Enemy>().ApplyDamage(dmg);

        if (collision.gameObject.tag != "Player")
            Destroy(gameObject);
    }
}
