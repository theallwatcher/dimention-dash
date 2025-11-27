using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Update()
    {
        
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
