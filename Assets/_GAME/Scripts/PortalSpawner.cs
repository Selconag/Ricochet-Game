using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Lean.Pool;
using System.Threading.Tasks;
using UnityEngine.AI;
//This monobehavious class is for spawning portals in a specialised random way
public class PortalSpawner : MonoBehaviour
{
    
    [SerializeField] private int minSpawnTime, maxSpawnTime;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Transform portalPool;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private int portalMultipliers = 2;

    //[SerializeField] private NavMeshSurface levelMesh;


    private bool gameStop = false;
    private void IncreaseMultiplierAmount()
    {
        portalMultipliers++;
    }

    private void Start()
	{
        GameManager.startGame += SpawnRepeater;
        Player.playerDeathEvent += StopSpawning;
        AbilityManager.IncreasePortalMultiplier += IncreaseMultiplierAmount;
    }

	private void OnDestroy()
	{
        GameManager.startGame -= SpawnRepeater;
        Player.playerDeathEvent -= StopSpawning;
        AbilityManager.IncreasePortalMultiplier -= IncreaseMultiplierAmount;
    }

	private void SpawnRepeater()
    {
        if (gameStop) return;
        StartCoroutine(SpawnPortal());
    }

    private IEnumerator SpawnPortal()
    {
        int region = Random.Range(0, 10);
        int angle = Random.Range(0, 9) * 45;
        Quaternion spawnAngle = new Quaternion();
        spawnAngle.eulerAngles = new Vector3(0, angle, 0);
        //Defines which zone will be selected for next spawn
        Vector3 zone = spawnPoints[region].position;
        //BulletPortal BP = LeanPool.Spawn(portalPrefab, zone, spawnAngle, portalPool).GetComponent<BulletPortal>();
        //BP.MultiplierAmount = portalMultipliers;
//        levelMesh.BuildNavMesh();
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime + 1));
        SpawnRepeater();
    }

    private void StopSpawning()
	{
        StopAllCoroutines();
        gameStop = true;
	}

    


}
