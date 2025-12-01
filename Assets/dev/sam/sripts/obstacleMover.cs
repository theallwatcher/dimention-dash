using UnityEngine;

public class obstacleMover : MonoBehaviour
{
    //Rigidbody rb;
    //[SerializeField] float speed = -500;
    [SerializeField] Transform endPoint;
    [SerializeField] GameObject particalPrefab;
    public bool isCoin = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){

        //rb = GetComponent<Rigidbody>();
       // rb.AddForce(new Vector3(0, 0, speed));
    }

    // Update is called once per frame
    void Update(){
        transform.position -= new Vector3(0, 0, GameManager.Instance.roadSpeed) * Time.deltaTime;

        if (gameObject.transform.position.z < endPoint.position.z){

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other){

        if (other.gameObject.tag == "Player" || other.gameObject.tag == "obstacle") {
            Debug.Log("hit player");
            if (isCoin == false){
                GameObject particals = Instantiate(particalPrefab, transform.position, Quaternion.Euler(new Vector3(180, 0, 0)));
                audioManagerSam.Instance.Play(audioManagerSam.SoundType.Break);
            }
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Shield") && !isCoin)
        {
            PlayerInventory inv = other.gameObject.GetComponentInParent<PlayerInventory>();
            inv.DestroyShield();
            Destroy(gameObject);
        }
    }
}
