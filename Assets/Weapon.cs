using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    public float fireRate = 0;
    public float damage = 10;
    public LayerMask whatToHit;
    public Transform BulletTrailPrefab;
    float timeToSpawnEffect;
    public float effectSpawnRate = 10;

    float timeToFire = 0;
    Transform firePoint;

	// Use this for initialization
	void Awake () {
        firePoint = transform.Find("FirePoint");
        if(firePoint == null) {
            Debug.LogError("No firepoint found");
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(fireRate == 0) {
            if(Input.GetButtonDown("Fire1")) {
                Shoot();
            }
        }
        else {
            if(Input.GetButton("Fire1") && Time.time > timeToFire) {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }
	}

    void Shoot() {
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, Vector2.right, 100, whatToHit);
        if(Time.time >= timeToSpawnEffect) {
            Effect();
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect() {
        Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation);
    }
}
