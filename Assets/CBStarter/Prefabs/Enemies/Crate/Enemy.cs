using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField] int health = 500;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void ApplyDamage(int dmg)
    {
        health -= dmg;
        if (health < 0)
            Destroy(transform.gameObject);
    }
}
