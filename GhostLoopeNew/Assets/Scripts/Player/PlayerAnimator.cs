using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    protected Animator animator;

    // Start is called before the first frame update
    void OnEnable()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBool(string animationName, bool animatorState)
    {
        animator.SetBool(animationName, animatorState);
    }

    public void SetFloat(string animationName, float animatorValue)
    {
        animator.SetFloat(animationName, animatorValue);
    }

    public void SetInt(string animationName, int animatorValue)
    {
        animator.SetInteger(animationName, animatorValue);
    }

    public void SetTrigger(string animatioName)
    {
        animator.SetTrigger(animatioName);
    }
}
