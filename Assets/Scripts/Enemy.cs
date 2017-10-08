using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class Enemy : MonoBehaviour {

	[System.Serializable]
	public class Boundary {
		public float yMin, yMax;
	}

	[System.Serializable]
	public class YAxisMovingType {
		public bool SIN, COS, RANDOMP, RANDOMN;
	}


	public Boundary boundary;
	public YAxisMovingType movingType;
	public Slider hpBar;
	public Text damageText;

	public float speed;
	public int damage;
	public float maxHP;
	[HideInInspector]
	public float curHp;

	private float timer;
	private float randomY;

	private Rigidbody2D m_rigidbody;
	private Transform m_transform;

    private JSONObject enemyData;

	void Start () {
		m_rigidbody = this.GetComponent<Rigidbody2D> ();
		m_transform = this.GetComponent<Transform> ();

		timer = 0.0f;
		randomY = Random.Range (0.03f, 0.15f);
		curHp = maxHP;

		hpBar.maxValue = maxHP;
		hpBar.value = curHp;

        LoadJsonFromFile();
        HandleEnemyData();

	}
	

	void Update () {
		Moving ();
       
	}

	private void Moving() {
		timer += Time.deltaTime;
		float moveX = Random.Range (-0.1f, -1.0f);
		float moveY;
		if (movingType.SIN) {
			moveY = Mathf.Sin (timer * speed / 2.0f);
		} else if (movingType.COS) {
			moveY = Mathf.Cos (timer * speed / 2.0f);
		} else if (movingType.RANDOMN) {
			moveY = -randomY;
		} else if (movingType.RANDOMP) {
			moveY = randomY;
		} else {
			moveY = 0;
		}


		Vector3 movement = new Vector3 (moveX, moveY, 0.0f);
		m_rigidbody.velocity = movement * speed;

		m_rigidbody.position = new Vector3 (
			m_rigidbody.position.x,
			Mathf.Clamp (m_rigidbody.position.y, boundary.yMin, boundary.yMax),
			0.0f
		);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.name == "edge") {
			GameKernel.life -= damage;
			Destroy (this.gameObject);
		}
	}

	public void OnHit (int damage) {
		curHp -= damage;
		damageText.text = "-" + damage.ToString ();
		Invoke ("RemoveDamageText", 0.5f);
		if (curHp <= 0) {
			Destroy (this.gameObject);
			return;
		}
		hpBar.value = curHp;
	}

	void RemoveDamageText () {
		damageText.text = "";
	}

    private void LoadJsonFromFile()
    {
        var jsonFile = Application.dataPath + "/Config/enemy.json";

        if (File.Exists(jsonFile))
        {
            enemyData = new JSONObject(File.ReadAllText(jsonFile));
        }
        else
        {
            Debug.LogError(jsonFile + " Not Found!");
        }
    }

    private void HandleEnemyData() {   

        enemyData.GetField("enemy", (JSONObject enemys) =>
        {
            foreach (JSONObject datas in enemys.list)
            {             
                if (this.name.Substring(0, 6) == datas.GetField("name").ToString().Substring(1, 6))
                {
                    speed = datas.GetField("speed").f;
                    damage = (int)datas.GetField("damage").f;
                    maxHP = datas.GetField("max HP").f;
                    boundary.yMin = datas.GetField("Y min").f;
                    boundary.yMax = datas.GetField("Y max").f;

                }

            }

        });
    }
}
