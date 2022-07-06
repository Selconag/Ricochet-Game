using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System;
using System.Threading.Tasks;
public class Player : MonoBehaviour
{
	public static Action playerDeathEvent;
	public static Action playerLevelEvent;
	public static Action<int> playerGainExp;
	public static bool playerDeath = false;

	[Header("Movement")]
	[Tooltip("Determines movement speed.")]
	[SerializeField] private float forwardMoveSpeed;
	[Range(1.0f,60f)]
	[Tooltip("Determines rotation speed.")]
	[SerializeField] private float rotateSpeed = 5.0f;

	[Header("Bullet")]
	[Range(0f,1f)]
	[Tooltip("Determines time amount between bullet spawns.")]
	[SerializeField] private float bulletSpawnInterval = 0.2f, spawnTimer = 0f;
    [Tooltip("Spawns bullets infinitely until end of turn if checked.")]
	[SerializeField] private bool unlimitedAmmo = true;
    [Tooltip("Determines how many bullets will be spawn until end of turn.")]
	[SerializeField] private int bulletAmountToSpawn;

	[Header("Object References")]
	[SerializeField] private GameObject m_BulletObject;
	[SerializeField] private ParticleSystem m_PS;
	[SerializeField] private Transform m_BulletSpawnPoint;
	[SerializeField] private GameObject m_LaserSight;
	[SerializeField] private Transform m_BulletPool;
	[SerializeField] private int expPerLevelModifier;

	private static Player _instance;
	AbilityManager AbilityM;
	private Joystick m_Joystick;
	private Rigidbody m_Rigid;
	//private Animator m_Animator;
	private float activeVelocity;
	private bool gameStarted = false;
	private float experienceCurrent = 0, experienceMax = 100;
	private int playerLevel = 1;
	private bool rainbowMode = false;

	#region Getter-Setters
	public static Player Instance
	{
		get { return _instance; }
	}

	private void GameStarted()
	{
		gameStarted = true;
	}

	public Vector3 PlayerPosition
	{
		get { return transform.position; }
		set { transform.position = value; }
	}

	public float BulletSpawnSpeed
	{
		get { return bulletSpawnInterval; }
		set 
		{
			//Decrease time by %20
			bulletSpawnInterval = bulletSpawnInterval - (bulletSpawnInterval / 5);
		}
	}

	public int BulletAmount
	{
        get { return bulletAmountToSpawn; }
		set
		{
			if (unlimitedAmmo) return;
			bulletAmountToSpawn--;
		}
	}

	public bool PlayerIsDead
	{
		set { playerDeath = true;}
	}

	public float CurrentExperience => experienceCurrent;
	public float MaxExperience => experienceMax;
	public int PlayerLevel => playerLevel;

	//EXPERIMENTAL PART
	public bool RainbowMode => rainbowMode;

	public async void StartRainbowMode()
	{
		rainbowMode = true;
		Debug.Log("Rainbow Start");
		await Task.Delay(5000);
		rainbowMode = false;
		Debug.Log("Rainbow End");
	}

	#endregion

	#region MonoBehaviour
	private void Start()
	{
		_instance = this;
		playerDeath = false;
		m_Joystick = Joystick.Instance;
		AbilityM = AbilityManager.Instance;
		m_Rigid = GetComponent<Rigidbody>();
		//m_Animator = GetComponent<Animator>();
		m_BulletPool = GameObject.Find("BulletPool").transform;
		GameManager.startGame += GameStarted;
		playerGainExp += UpdateExperienceSystem;
	}

	private void OnDestroy()
	{
		GameManager.startGame -= GameStarted;
		playerGainExp -= UpdateExperienceSystem;
	}

	void Update()
	{
		if (m_BulletPool == null) m_BulletPool = GameObject.Find("BulletPool").transform;
		if (!gameStarted) return;
		if (playerDeath) return;

		spawnTimer += Time.deltaTime;
		//m_Animator.SetBool("Walking", (false));
		//m_Animator.SetFloat("Velocity", (0));

		if (Input.touchCount > 0 || Input.GetMouseButton(0))
		{
			//Determine the direction
			Vector3 direction = Vector3.forward * m_Joystick.Vertical + Vector3.right * m_Joystick.Horizontal;
			float horizontal = m_Joystick.Horizontal;
			float vertical = m_Joystick.Vertical;
			Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
			float joystickMagnitude = Mathf.Sqrt(Mathf.Pow(horizontal, 2) + Mathf.Pow(vertical, 2));


			//Vector3 lookDirection = new Vector3(horizontal, 0, forward).normalized;
			if (moveDirection.magnitude == 0)
				return;

			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(moveDirection), rotateSpeed * Time.deltaTime);

			//Move towards the direction
			////MOVE SYSTEM 2
			//Vector3 xMove = new Vector3(horizontal, 0, 0), zMove = new Vector3(0, 0, vertical);
			//transform.Translate(xMove * forwardMoveSpeed * Time.deltaTime, Space.Self);
			//transform.Translate(zMove * forwardMoveSpeed * Time.deltaTime, Space.Self);
			//MOVE SYSTEM 1
			transform.Translate((Vector3.forward * joystickMagnitude) * forwardMoveSpeed * Time.deltaTime, Space.Self);


			//m_Animator.SetFloat("Velocity", (moveDirection.magnitude));
			//m_Animator.SetBool("Walking", (true));

		}

		if(unlimitedAmmo || bulletAmountToSpawn > 0)
        {
			if ((spawnTimer >= bulletSpawnInterval) && !playerDeath)
			{
				BulletAmount = 0;
				SpawnBullets();
				spawnTimer = 0f;
			}
		}
	}

	private void SpawnBullets()
	{
		GameObject go = LeanPool.Spawn(m_BulletObject, m_BulletSpawnPoint.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), m_BulletPool);
		Bullet bullet = go.GetComponent<Bullet>();
		bullet.IsInstantiatable = true;
		bullet.PierceLevel = AbilityM.PiercingLevel;
		bullet.BounceLevel = AbilityM.BouncingLevel;
		go.transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
		//go.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (RainbowMode) return;

		if(collision.gameObject.tag == "Enemy")
		{
			//Game Ends
			Debug.Log("Player is Dead");
			m_PS.Play();
			//m_Animator.SetBool("Death", (true));
			playerDeathEvent.Invoke();
			GameManager.levelEndStatus.Invoke(false);
			//GameManager.levelEndStatus.Invoke(false);
			playerDeath = true;
			m_LaserSight.SetActive(false);
		}
	}

	private void UpdateExperienceSystem(int exp)
	{
		if (playerDeath) return;
		experienceCurrent += exp;
		if(experienceCurrent >= experienceMax)
		{
			experienceCurrent -= experienceMax;
			playerLevel++;
			playerLevelEvent.Invoke();
			experienceMax = (playerLevel * expPerLevelModifier) + 100;
			GameManager.Instance.UpdateExperienceSystem(playerLevel);
			//GameManager.Instance.SkillsPanel(true);
		}
	}
	#endregion
}
