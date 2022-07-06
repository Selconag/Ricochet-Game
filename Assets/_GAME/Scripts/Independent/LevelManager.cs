using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Lean.Pool;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
	public static Action levelChange;

    [SerializeField] private List<GameObject> m_LevelList;
	private GameObject activeLevelObject;
	private int activeLevelIndex;
	private static LevelManager _instance;
	
	#region Getter Setter 
	private LevelManager() { }

	public static LevelManager Instance
	{
		get { return _instance; }
	}

	public int ActiveLevelInfo
	{
		get { return activeLevelIndex; }
		set { activeLevelIndex = value; }
	}
	#endregion

	#region Mono
	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		//GameManager.levelEndStatus += SpawnNextLevel;
		//SpawnLevel();
	}

	private void OnDestroy()
	{
		 //GameManager.levelEndStatus -= SpawnNextLevel;
	}

	public void SpawnNextLevel(bool status)
	{
		//Player succeeded the level, therefore continue to next level
		if (status)
		{
			//Despawn this level and spawn the next one
			Destroy(activeLevelObject);
			activeLevelIndex++;
			if (activeLevelIndex == m_LevelList.Count) activeLevelIndex = 0;
			activeLevelObject = Instantiate(m_LevelList[activeLevelIndex]);
		}
		//Player failed the level, therefore retry thee level
		else
		{
			//Respawn the level once more
			Destroy(activeLevelObject);
			activeLevelObject = Instantiate(m_LevelList[activeLevelIndex]);
		}
		GameManager.GameStartReset = false;
		LeanPool.DespawnAll();
		levelChange.Invoke();
		//Despawn all bullets, if there is any active
		LeanPool.DespawnAll();
	}

	public void SpawnNextScene()
	{
		GameManager.GameStartReset = false;
		if(SceneManager.GetActiveScene().buildIndex < 9)
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		else
			SceneManager.LoadScene(0);

	}

	public void ReSpawnScene()
	{
		GameManager.GameStartReset = false;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

	}

	public void SpawnLevel()
	{
		activeLevelObject = Instantiate(m_LevelList[activeLevelIndex]);
	}
#endregion

}
