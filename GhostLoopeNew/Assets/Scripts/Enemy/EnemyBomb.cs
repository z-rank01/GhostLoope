using UnityEngine;
using UnityEngine.Rendering;

public class EnemyBomb : EnemyMob
{

    protected override void CheckHP()
    {
        // ≈–∂œπ÷ŒÔ «∑ÒÀ¿Õˆ
        if (hp <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("Ú˘Úπ÷±¨’®“Ù–ß");

            // die animation event
            AddDieAnimationEvent();

            // animation
            animator.SetTrigger("GoingToExplode");
        }
    }

    private bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    private bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    private void AddDieAnimationEvent()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        int idx = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == "Die") idx = i;
        }

        // event: explode
        AnimationEvent explodeEvent = new AnimationEvent();
        explodeEvent.functionName = "ExplodeAfterDie";
        explodeEvent.time = clips[idx].length / 2;
        explodeEvent.objectReferenceParameter = this.gameObject;


        // event: disable object
        AnimationEvent dieEvent = new AnimationEvent();
        dieEvent.functionName = "DisableAfterDie";
        dieEvent.time = clips[idx].length;
        dieEvent.objectReferenceParameter = this.gameObject;

        // add event
        clips[idx].AddEvent(explodeEvent);
        clips[idx].AddEvent(dieEvent);
    }

    // interface
    public void DisableAfterDie(GameObject targetObj)
    {
        targetObj.SetActive(false);
    }

    public void ExplodeAfterDie(GameObject targetObj)
    {
        // explode effect
        EnemyBomb enemyBomb = targetObj.GetComponent<EnemyBomb>();
        GameObject bulletObj = PoolManager.GetInstance().GetObj(E_PoolType.ExplodeBullet);
        enemyBomb.EnemyReceiveDamage(bulletObj.GetComponent<Bullet>());
        PoolManager.GetInstance().ReturnObj(E_PoolType.ExplodeBullet, bulletObj);
    }
}