using UnityEngine;

public class RodAnimator : MonoBehaviour
{

    // grab the animator component
    private Animator animator;
    public string currentAnimation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
      void Awake()
    {
        animator = GetComponent<Animator>();

        // Explicitly reset triggers at start to prevent auto-play
        animator.ResetTrigger("Cast");
        animator.ResetTrigger("Reel");

        animator.Play("Idle");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerCast()
    {
        animator.SetTrigger("Cast");
    }

    public void TriggerReel()
    {
        animator.SetTrigger("Reel");
    }

}
