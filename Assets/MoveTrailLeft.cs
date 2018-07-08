using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrailLeft : MonoBehaviour {
    public int moveSpeed = 230;

	// Update is called once per frame
	void Update () {
        transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);
        Destroy(gameObject, 1);
	}
}
