using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameKernel : MonoBehaviour {

	public static int life;

	public Text lifeText;
	public Spawner spawner;

	void Start () {
		life = 30;
		lifeText.text = life.ToString();
	}

	void Update () {
		if (life.ToString() != lifeText.text) {
			lifeText.text = life.ToString ();
			if (life <= 0) {
				Debug.Log ("Game Over, reloading at 1s later... ");
				Invoke ("ReloadScene", 1.0f);
				this.enabled = false;
			}
			if (life <= 10) {
				lifeText.color = Color.red;
			}
		}

		if (spawner.isSpawning && spawner.curLevel > spawner.level && GameObject.FindGameObjectsWithTag("enemy").Length == 0) {
			Debug.Log ("Game Win, reloading at 3s later... ");
			Invoke ("ReloadScene", 3.0f);
			this.enabled = false;
		}
	}

	void ReloadScene () {
		SceneManager.LoadScene ("mainscene");
	}
}
