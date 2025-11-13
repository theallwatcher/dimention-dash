using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour
{

    [SerializeField] List<GameObject> obstacles = new List<GameObject>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] Transform endPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        

    }

    // Update is called once per frame
    void Update(){

        if (Input.GetButtonDown("space")) {

            SpawnObstacle();
        
        }

    }

    public void SpawnObstacle() { 

        int randomObstacle = Random.Range(0, obstacles.Count);
        int randomSpawn = Random.Range(0, spawnPoints.Count);

        GameObject obstacle = Instantiate(obstacles[randomObstacle], spawnPoints[randomSpawn].position, spawnPoints[randomSpawn].rotation);
    }
}
