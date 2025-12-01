using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    float runSpeed = 1;
    float lastKnownSpeed;

    private void Update()
    {
        GameManager manager = GameManager.Instance;

        /*if(manager.roadSpeed > manager.startSpeed)
        {
            lastKnownSpeed = manager.roadSpeed;
            runSpeed += 0.1f;
            manager.startSpeed = manager.roadSpeed;
            animator.SetFloat("RunSpeed", runSpeed);
        }*/
    }

    public void SetIsGrounded(bool value)
    {
        animator.SetBool("IsGrounded", value);
    }

    public void SetIsSliding(bool value)
    {
        animator.SetBool("IsSliding", value);
    }
}
