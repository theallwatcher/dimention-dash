using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BombPowerup : BasePowerup
{
    Vector3 startPoint;
    Vector3 endPoint;

    [SerializeField] GameObject explosionPrefab;

    [Header("Curve config")]
    [SerializeField] float radius;
    [SerializeField] float duration;
    [SerializeField] float arcHeight;

    [Header("Explosion config")]
    [SerializeField] float explosionRange;
    [SerializeField] GameObject opponent;
    [SerializeField] float explosionForce = 5;
    bool targetReached = false;
    float explosionTime = 1f;

    [Header("Scale config")]
    [SerializeField] float endScale = 3f;
    float scaleTimer = 0f;
    public void Constructor(Transform startPos, Transform endPos, GameObject enemy) //adds player and opponent transforms
    {
        //add height to startPos
        Vector3 start = new Vector3(startPos.position.x, startPos.position.y , startPos.position.z);
        startPoint = start;

        float offSet = Random.Range(3, 8);
        Vector3 end = new Vector3(endPos.position.x, endPos.position.y + 1, endPos.position.z + offSet);
        endPoint = end;

        //save exact enemy position for distance calculations
        opponent = enemy;
    }
    private void Start()
    {
        //start moving in direction of other player
        StartCoroutine(FollowArc(transform, startPoint, endPoint, radius, duration));
    }

    private void Update()
    {
        //scale item up to 
        scaleTimer += Time.deltaTime;

        float t = scaleTimer / duration;
        if(scaleTimer < duration)
        {
            Vector3 startScale = new Vector3(1, 1, 1);
            Vector3 targetScale = new Vector3(endScale, endScale, endScale);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        }

        if (!targetReached) return;

        //start moving when target pos is reached

            transform.position -= new Vector3(0, 0, GameManager.Instance.roadSpeed) * Time.deltaTime;

            explosionTime -= Time.deltaTime;

            if(explosionTime < 0)
            {
                Explode();
            }
    }

     protected override void OnTriggerEnter(Collider other)
    {
        //if opponent runs into bomb it explodes
        if (other.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        float distance = Vector3.Distance(transform.position, opponent.transform.position);
        GameObject explosion = Instantiate(explosionPrefab,transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        //player effect
        audioManagerSam.Instance.Play(audioManagerSam.SoundType.Explosion);


        if (distance < explosionRange)
        {
            PlayerMovement moveScript = opponent.GetComponent<PlayerMovement>();
            PlayerInventory inventory = opponent.GetComponent<PlayerInventory>();

            if (moveScript != null)
            {
                if (!inventory.hasShield)
                {
                    moveScript.MovePlayerZ(explosionForce);
                }
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
        Destroy(gameObject.transform.parent.gameObject);
    }


    IEnumerator FollowArc(
        Transform mover,
        Vector3 start,
        Vector3 end,
        float radius,
        float duration)
    {
        Vector3 center = (start + end) * 0.5f;

        // Direction from start to end
        Vector3 forward = (end - start).normalized;

        // Find a perpendicular direction for arc height
        Vector3 up = Vector3.up;
        if (Mathf.Abs(Vector3.Dot(up, forward)) > 0.9f)
            up = Vector3.right;  // avoid parallel vectors

        Vector3 perpendicular = Vector3.Cross(forward, up).normalized;

        float halfDist = Vector3.Distance(start, end) * 0.5f;

        float progress = 0f;

        while (progress < 1f)
        {
            float t = progress;

            // Arc formula: parabola
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;

            Vector3 pos =
                Vector3.Lerp(start, end, t) +
                Vector3.up * height;

            mover.position = pos;

            progress += Time.deltaTime / duration;
            yield return null;
        }

        mover.position = end;
        targetReached = true;
        }
    }
