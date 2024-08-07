using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    protected Animator animator;
    private List<AnimationClip> clips;

    // Start is called before the first frame update
    void OnEnable()
    {
        // animator
        animator = GetComponent<Animator>();

        // animation clip
        clips = new List<AnimationClip>();
        AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in animationClips)
        {
            clips.Add(clip);
        }
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

    public void AddEvent(string clipName, AnimationEvent animationEvent)
    {
        int idx = 0;
        for (int i = 0; i < clips.Count; i++)
        {
            if (clips[i].name == clipName) idx = i;
        }
        clips[idx].AddEvent(animationEvent);
    }

    public float GetClipLength(int idx)
    {
        return clips[idx].length;
    }

    public float GetClipLength(string clipName)
    {
        for (int i = 0; i < clips.Count; i++)
        {
            if (clips[i].name == clipName) return clips[i].length;
        }
        return 0;
    }
}
