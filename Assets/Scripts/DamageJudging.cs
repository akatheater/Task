using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageJudging : MonoBehaviour {


	public bool judging;
	// Use this for initialization
	void Start () {
		judging = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay2D (Collider2D other) {
		if (judging && other.tag == "enemy") {
			judging = false;
			var enemy = other.gameObject;
			enemy.GetComponent<Enemy> ().OnHit (this.transform.parent.GetComponent<Character> ().atk);
		}
	}
}
