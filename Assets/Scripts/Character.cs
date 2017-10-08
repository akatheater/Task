using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Character : MonoBehaviour {


	[System.Serializable]
	public class Boundary {
		public float xMin, xMax, yMin, yMax;
	}

	public Boundary boundary;

	public float speed;
	public int atk;
	public float skillCD;
	public float skillDuration;

    private JSONObject playerData;

	private bool isAtking;
	private bool isSkilling;
	private float atkTimer;
	private float skillCDTimer;
	private float skillDurationTimer;
	private DamageJudging damageJudging;

	private Transform m_transform;
	private Rigidbody2D m_rigidbody;
	private Animator m_animator;

	void Start () {
		m_transform = this.GetComponent<Transform> ();
		m_rigidbody = this.GetComponent<Rigidbody2D> ();
		m_animator = this.GetComponent<Animator> ();

		isAtking = false;
		isSkilling = false;
		atkTimer = 0.0f;
		skillCDTimer = 0.0f;
		skillDurationTimer = skillDuration;
		damageJudging = this.GetComponentInChildren<DamageJudging>();

        LoadJsonFromFile();
        HandlePlayerData();
	}
	

	void FixedUpdate () {
		Moving ();
		Attacking ();
		Skill ();
	}

	private void Moving() {
		float moveX = Input.GetAxis ("Horizontal");
		float moveY = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveX, moveY, 0.0f);
		m_rigidbody.velocity = movement * speed;

		m_animator.SetFloat ("speed", Mathf.Abs ((m_rigidbody.velocity.x + m_rigidbody.velocity.y) * 10.0f));
		if (m_rigidbody.velocity.x < 0) {
			m_transform.localScale = new Vector2 (-2.0f, 2.0f);
		}
		if (m_rigidbody.velocity.x > 0) {
			m_transform.localScale = new Vector2 (2.0f, 2.0f);
		}

		m_rigidbody.position = new Vector3 (
			Mathf.Clamp (m_rigidbody.position.x, boundary.xMin, boundary.xMax),
			Mathf.Clamp (m_rigidbody.position.y, boundary.yMin, boundary.yMax),
			0.0f
		);
	}

	private void Attacking () {
		isAtking = false;
		if (Input.GetKey (KeyCode.Space)) {
			isAtking = true;
		}
		m_animator.SetBool ("isAtking", isAtking);
		if (isAtking) {
			atkTimer += Time.deltaTime;
		}
		if (!isAtking) {
			atkTimer = 0.0f;
			damageJudging.judging = false;
		}

		if (isAtking && atkTimer >= 1.0f) {
			atkTimer = 0.0f;
			damageJudging.judging = true;
			// Debug.Log (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}
	}

	private void Skill () {

		if (skillCDTimer > 0.0f) {
			skillCDTimer -= Time.deltaTime;
		}
		if (skillCDTimer <= 0.0f && Input.GetKey (KeyCode.LeftControl)) {
			skillCDTimer = skillCD;
			isSkilling = true;
		}
		if (skillDurationTimer <= 0.0f) {
			isSkilling = false;
			skillDurationTimer = skillDuration;
		}
		if (isSkilling) {
			damageJudging.judging = true;
			skillDurationTimer -= Time.deltaTime;
			m_animator.SetFloat ("skill_time", skillDurationTimer);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (!isSkilling && this.GetComponent<Collider2D> ().IsTouching (other)) {
			GameKernel.life -= other.gameObject.GetComponent<Enemy> ().damage;
			Destroy (other.gameObject);
		}
	}

    private void LoadJsonFromFile()
    {
        var jsonFile = Application.dataPath + "/Config/player.json";

        if (File.Exists(jsonFile))
        {
            playerData = new JSONObject(File.ReadAllText(jsonFile));
        }
        else
        {
            Debug.LogError(jsonFile + " Not Found!");
        }
    }

    private void HandlePlayerData()
    {
        speed = playerData.GetField("speed").f;
        atk = (int)playerData.GetField("atk").f;
        skillCD = playerData.GetField("skill CD").f;
        skillDuration = playerData.GetField("skill duration").f;
        boundary.xMin = playerData.GetField("X min").f;
        boundary.xMax = playerData.GetField("X max").f;
        boundary.yMin = playerData.GetField("Y min").f;
        boundary.yMax = playerData.GetField("Y max").f;       

                  
    }
}
  