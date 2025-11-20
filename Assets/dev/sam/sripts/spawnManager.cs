using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class spawnManager : MonoBehaviour
{

    [SerializeField] List<GameObject> obstacles = new List<GameObject>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] float spawnDelay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        
        StartCoroutine(spawnTimer(spawnDelay));
    }

    // Update is called once per frame
    void Update(){

        if (Keyboard.current.spaceKey.wasPressedThisFrame) {

            SpawnObstacle();
        
        }

    }

    public void SpawnObstacle() { 

        int randomObstacle = Random.Range(0, obstacles.Count);
        int randomSpawn = Random.Range(0, spawnPoints.Count);

        GameObject obstacle = Instantiate(obstacles[randomObstacle], spawnPoints[randomSpawn].position, spawnPoints[randomSpawn].rotation);
    }

    IEnumerator spawnTimer(float seconds) { 


        yield return new WaitForSeconds(seconds);
        SpawnObstacle();
        StartCoroutine(spawnTimer(spawnDelay));
    
    }
}
