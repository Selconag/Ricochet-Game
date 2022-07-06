using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lean.Pool;
public class BulletPortal : MonoBehaviour
{ 
	[Tooltip("It is recommended to determine the value as minimum as possible.")]
	[Range(0.05f, 0.5f)]
	[SerializeField] private float bulletSpawnDistance = 0.2f;
	[Range(0,12)]
	[Tooltip("Determines how many objects will be spawned from this portal.")]
	[SerializeField] private int multiplierAmount = 0;
	[Tooltip("Determines how much angle each bullet scatter.")]
	[SerializeField] private float anglePerBullet = 30;
	[Tooltip("Determines how much more lifespan each bullet have.")]
	[SerializeField] private float additionalBulletLifeSpan = 2f;

	[SerializeField] private TextMeshPro m_MultiplierText;
	[SerializeField] private GameObject m_BulletObject;
	[SerializeField] private Transform m_BulletPool;
	[SerializeField] private bool dynamicSpawnPortal;
	[SerializeField] private int despawnDistance = 30;

	public bool IsSpawnedDynamically => dynamicSpawnPortal;

	public int MultiplierAmount
	{
		get { return multiplierAmount; }
		set { multiplierAmount = value; }
	}

	private void IncreaseMultiplierAmount()
	{
		multiplierAmount++;
		m_MultiplierText.text = "x" + multiplierAmount.ToString();
	}

	#region Mono
	void Start()
    {
        m_MultiplierText.text = "x" + multiplierAmount.ToString();
		m_BulletPool = GameObject.Find("BulletPool").transform;
		transform.GetComponentInChildren<TriggerDetector>().SetBulletLifeSpan = additionalBulletLifeSpan;
		AbilityManager.IncreasePortalMultiplier += IncreaseMultiplierAmount;
	}

	private void OnDestroy()
	{
		AbilityManager.IncreasePortalMultiplier -= IncreaseMultiplierAmount;
	}

	private void FixedUpdate()
	{
		if(dynamicSpawnPortal)
		{
			//Check if the player is too far from this portal, despawn it
			Vector3 portalLoc = this.gameObject.transform.position, playerLoc;
			playerLoc = Player.Instance.transform.position;
			float distanceX = portalLoc.x - playerLoc.x; 
			float distanceZ = portalLoc.z - playerLoc.z;
			float distance = Mathf.Sqrt(Mathf.Pow(distanceX,2) + Mathf.Pow(distanceZ,2));
			if (distance >= despawnDistance)
			{
				LeanPool.Despawn(this.gameObject);
			}
		}
	}

	#endregion

	#region Bullet Spawn
	//If bullets will scatter by position, this method will be used.
	void BulletSpawnerByPosition(Transform target)
	{
		Bullet bullet;

		for (int i = 1; i < multiplierAmount + 1; i++)
		{
			Vector3 newPos = target.position;

			if (i / 2 == 1)
				newPos.x = target.position.x + (-(i / 2 + i % 2) * bulletSpawnDistance);
			else
				newPos.x = target.position.x + ((i / 2 + i % 2) * bulletSpawnDistance);

			bullet = LeanPool.Spawn(m_BulletObject, newPos, target.rotation,m_BulletPool).GetComponent<Bullet>();
			//bullet.IsInstantiatable = false;
		}

	}

	//If bullets will scatter by rotation, this method will be used.
	public void BulletSpawnerByRotation(Transform target)
	{
		Bullet newBullet;
		AbilityManager AbilityM = AbilityManager.Instance;
		Vector3[] rotations = new Vector3[multiplierAmount];
		Vector3 spawnPos = this.transform.position;
		spawnPos.y = target.position.y;

		//Calculate the angle change per bullet
		float cutAngle = anglePerBullet / (2);

		for (int i = 0; i < multiplierAmount; i++)
		{
			rotations[i] = target.rotation.eulerAngles;
		}
		
		if (multiplierAmount % 2 == 0)
		{
			rotations[0].y = rotations[0].y - (cutAngle);
			rotations[1].y = rotations[1].y + (cutAngle);
			newBullet = LeanPool.Spawn(m_BulletObject, spawnPos, Quaternion.Euler(rotations[0]), m_BulletPool).GetComponent<Bullet>();
			newBullet.PierceLevel = AbilityM.PiercingLevel;
			newBullet.BounceLevel = AbilityM.BouncingLevel;
			newBullet = LeanPool.Spawn(m_BulletObject, spawnPos, Quaternion.Euler(rotations[1]), m_BulletPool).GetComponent<Bullet>();
			newBullet.PierceLevel = AbilityM.PiercingLevel;
			newBullet.BounceLevel = AbilityM.BouncingLevel;

			if (multiplierAmount >= 3)
			{
				for (int i = 2; i < multiplierAmount; i++)
				{
					if (i % 2 == 0)
						rotations[i].y = rotations[i].y - (cutAngle + (((i - 1) / 2 + (i - 1) % 2) * anglePerBullet));
					else
						rotations[i].y = rotations[i].y + (cutAngle + (((i - 1) / 2 + (i - 1) % 2) * anglePerBullet));


					newBullet = LeanPool.Spawn(m_BulletObject, spawnPos, Quaternion.Euler(rotations[i]), m_BulletPool).GetComponent<Bullet>();
					//newBullet.IsInstantiatable = false;
					newBullet.PierceLevel = AbilityM.PiercingLevel;
					newBullet.BounceLevel = AbilityM.BouncingLevel;
				}
			}
		}

		else
		{
			for (int i = 0; i < multiplierAmount; i++)
			{
				if (i % 2 == 1)
					rotations[i].y = rotations[i].y - ((i / 2 + i % 2) * anglePerBullet);

				else
					rotations[i].y = rotations[i].y + ((i / 2 + i % 2) * anglePerBullet);

				newBullet = LeanPool.Spawn(m_BulletObject, spawnPos, Quaternion.Euler(rotations[i]), m_BulletPool).GetComponent<Bullet>();
				//newBullet.IsInstantiatable = false;
				newBullet.PierceLevel = AbilityM.PiercingLevel;
				newBullet.BounceLevel = AbilityM.BouncingLevel;
			}
		}
	}

	#endregion
}
