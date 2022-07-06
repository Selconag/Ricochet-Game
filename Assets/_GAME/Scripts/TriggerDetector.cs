using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class TriggerDetector : MonoBehaviour
{
	[Tooltip("Determines how much more lifespan each bullet have.")]
	[SerializeField] private float additionalBulletLifeSpan = 2f;
	[Tooltip("Determines how many seconds remaining until new spawn.")]
	[SerializeField] private float spawnWaiter = 2f;

	private BulletPortal m_ParentObject;
	private float spawnerClock = 0f;
	private bool spawnReady = true;

	public float SetBulletLifeSpan
	{
		set { additionalBulletLifeSpan = value; }
	}
	private void Start()
	{
		m_ParentObject = transform.GetComponentInParent<BulletPortal>();
	}

	private void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Bullet")
		{
			if (!spawnReady) return;

			if (col.gameObject.GetComponent<Bullet>().IsInstantiatable)
			{
				Transform t = col.gameObject.transform;
				LeanPool.Despawn(col.gameObject);
				m_ParentObject.BulletSpawnerByRotation(t);
				StartCoroutine(SpawnWaiter());
			}
		}
	}

	IEnumerator SpawnWaiter()
	{
		spawnReady = false;
		for (int i = 0; i < spawnWaiter; i++)
		{
			yield return new WaitForEndOfFrame();
		}
		spawnReady = true;
	}
}
