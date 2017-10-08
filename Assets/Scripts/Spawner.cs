using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

	private float timer;
//	private int spawnOrder;

	public Transform[] spawnZones;
	public GameObject[] spawnUnits;
	public Text levelText;

	public float spawnTime;
	public int level;
	public float spawnDeltaTime;

	[HideInInspector]
	public int curLevel;
	[HideInInspector]
	public bool isSpawning;

	private JSONObject spawnData;
	private List<Queue<int>> levels;

	void Start () {
		levels = new List<Queue<int>> ();

		LoadJsonFromFile ();
		HandleSpawnData ();

//		Debug.Log (levels [1]);

		curLevel = 1;
		timer = 0.0f;
		isSpawning = true;

		levelText.text = curLevel.ToString ();
//		spawnOrder = 0;
	}

	void Update () {
		timer += Time.deltaTime;
		if (timer >= spawnTime) {
			timer = 0.0f;

			Queue<int> units = null;
//			Debug.Log (curLevel);
			if (curLevel <= level) {
				units = levels[curLevel - 1];
			}
//			Debug.Log (units.Count);
			if (isSpawning && curLevel <= level) {
				GameObject enemy;
				try {
					enemy = Instantiate (spawnUnits[units.Dequeue()]);
					enemy.transform.SetParent (null);
//					Debug.Log (units.Count);
					//				Debug.Log (enemy.name);
				} catch (Exception e) {
					Debug.LogError (e);
				}
			}

			if (curLevel <= level && units.Count == 0) {
				curLevel++;
				isSpawning = false;
				Invoke ("NextSpawn", spawnDeltaTime);
			}
		}
			
//			if (spawnOrder / 3 == 1) { 
//				spawnOrder = 0;
//			}
//			Debug.Log (spawnOrder);
//			var enemy = Instantiate (spawnUnits[Random.Range(0, 3)], spawnZones[spawnOrder]) as GameObject;
//			enemy.transform.SetParent (null);
//			spawnOrder++;
//		}
	}

	void NextSpawn () {
		isSpawning = true;
		levelText.text = curLevel.ToString ();
	}

	private void LoadJsonFromFile () {
		var jsonFile = Application.dataPath + "/Config/spawn.json";

		if (File.Exists (jsonFile)) {
			spawnData = new JSONObject (File.ReadAllText (jsonFile));
		} else {
			Debug.LogError (jsonFile + " Not Found!");
		}
	}

	private void HandleSpawnData () {
		level = (int) spawnData.GetField ("config").GetField ("level").f;
		spawnTime = spawnData.GetField ("config").GetField ("spawnTime").f;
		spawnDeltaTime = spawnData.GetField ("config").GetField ("spawnDeltaTime").f;

		spawnData.GetField ("levels", (JSONObject levels) => {
			foreach (JSONObject spawnUnits in levels.list) {
				spawnUnits.GetField ("spawnUnits", (JSONObject datas) => {
					Queue<int> unitsId = new Queue<int> ();
					foreach (JSONObject data in datas.list) {
						for (int i = 0; i < (int)data.GetField ("quantity").f; i++) {
							unitsId.Enqueue ((int)data.GetField ("id").f);
						}
					}
					this.levels.Add (unitsId);
				});

			}
		});
			
	}
}
