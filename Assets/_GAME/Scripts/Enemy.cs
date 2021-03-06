using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Lean.Pool;
public class Enemy : MonoBehaviour
{
    [SerializeField] private EntityProperties m_EnemyProperties;
    [SerializeField] private Transform m_Target;
    [SerializeField] private ParticleSystem m_PS;

    private bool walkEnd = false;

    private NavMeshAgent m_Agent;
    private Animator m_Anim;
    private Collider m_Coll;
    private Rigidbody m_Rigid;
    private Renderer m_Rend;
    private bool lastEnemy;
    private bool firstEnable = false;
    [SerializeField] private int health;

    public bool IsLastEnemy
	{
		get { return lastEnemy; }
		set { lastEnemy = true; }
	}

    public int Health
    {
        get { return health; }
        set { health = value; }
    }


    private void Awake()
	{
        m_Agent = GetComponent<NavMeshAgent>();
        m_Anim = GetComponent<Animator>();
        m_Coll = GetComponent<Collider>();
        m_Rigid = GetComponent<Rigidbody>();
        m_Rend = transform.GetChild(0).GetComponent<Renderer>();
    }

	void Start()
    {
        m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        m_Agent.speed = m_EnemyProperties.Speed;
        health = m_EnemyProperties.Health;
    }

	void Update()
	{
		if (!walkEnd)
		{
			transform.LookAt(m_Target);
			if (m_Agent.isActiveAndEnabled) m_Agent.SetDestination(m_Target.position);
		}
	}
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            //if (!col.transform.gameObject.GetComponent<Bullet>().IsPiercingBullet)
            //    LeanPool.Despawn(col.gameObject);
            col.gameObject.GetComponent<Bullet>().DecreasePierce = 1;
            StartCoroutine(FlashEffect());
            health--;
            //If no health left, enemy dies
            if (health <= 0)
            {
                Player.playerGainExp.Invoke(m_EnemyProperties.Experience);
                StartCoroutine(DespawnWaiter());
            }
            //If enemy survives, apply knockback
            else
            {
                m_Rigid.AddForce(Vector3.back * 1000f, ForceMode.Force);

            }

        }
    }

    //private void OnCollisionEnter(Collision col)
    //{
    //	if (col.gameObject.tag == "Bullet")
    //	{
    //           //if (!col.transform.gameObject.GetComponent<Bullet>().IsPiercingBullet)
    //           //    LeanPool.Despawn(col.gameObject);
    //           col.gameObject.GetComponent<Bullet>().DecreasePierce = 1;
    //           StartCoroutine(FlashEffect()); 
    //           health--;
    //           //If no health left, enemy dies
    //           if(health <= 0)
    //		{
    //               Player.playerGainExp.Invoke(m_EnemyProperties.Experience);
    //               StartCoroutine(DespawnWaiter());
    //           }
    //		//If enemy survives, apply knockback
    //		else
    //		{
    //               m_Rigid.AddForce(Vector3.back * 1000f,ForceMode.Force);

    //           }

    //       }
    //   }
    private IEnumerator FlashEffect()
	{
        //Apply flash effects
        MaterialPropertyBlock MPB = new MaterialPropertyBlock();
        Color c = transform.GetChild(0).GetComponent<Renderer>().material.GetColor("_Color");
        m_Rend.GetPropertyBlock(MPB);
        MPB.SetColor("_Color", Color.yellow);
        m_Rend.SetPropertyBlock(MPB);
        yield return new WaitForSeconds(0.1f);
        MPB.SetColor("_Color", c);
        m_Rend.SetPropertyBlock(MPB);
    }


    IEnumerator DespawnWaiter()
	{
        m_PS.Play();
        m_Anim.SetBool("Death", (true));
        m_Coll.enabled = false;
        StopEnemy();
        yield return new WaitForSeconds(1.5f);
        LeanPool.Despawn(this.gameObject);

    }

    private void StopEnemy()
	{
        walkEnd = true;
        m_Target = this.transform;
        m_Anim.SetBool("Walking", (false));
        transform.LookAt(m_Target);
		if (m_Agent.isActiveAndEnabled) m_Agent.SetDestination(this.gameObject.transform.position);
	}

    private void OnEnable()
    {
        if(firstEnable)
        m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        firstEnable = true;
        walkEnd = false;
        m_Coll.enabled = true;
        health = m_EnemyProperties.Health;

        if (Player.playerDeath) StopEnemy();
        else Player.playerDeathEvent += StopEnemy;
    }

    private void OnDisable()
    {
        //If this was last enemy, Invoke level end with success
        if (lastEnemy)
		{
            if(GameObject.FindGameObjectsWithTag("Enemy").Length <= 1)
                GameManager.levelEndStatus.Invoke(true);
            Debug.Log("Last Enemy Killed");
            lastEnemy = false;
        }
        walkEnd = false;
        Player.playerDeathEvent -= StopEnemy;
    }
}
