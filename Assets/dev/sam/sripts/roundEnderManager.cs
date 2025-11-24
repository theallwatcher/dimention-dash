using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roundEnderManager : MonoBehaviour
{

    [SerializeField] private GameObject portals;
    [SerializeField] List<GameObject> spawners = new List<GameObject>();
    [SerializeField] private float roundDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){

        StartCoroutine(RoundCountDouwn(roundDuration));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RoundCountDouwn(float time) { 
    
        yield return new WaitForSeconds(time);
        EndRound();
    }

    private void EndRound() { 

        portals.SetActive(true);
        for (int i = 0; i < spawners.Count; i++){

            spawners[i].SetActive(false);
        }
    }
}
