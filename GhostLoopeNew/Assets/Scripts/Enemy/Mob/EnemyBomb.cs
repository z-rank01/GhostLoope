using UnityEngine;
using UnityEngine.Rendering;

public class EnemyBomb : EnemyMob
{
    new protected void OnEnable()
    {
        base.OnEnable();
        // die animation event
        AddDieAnimationEvent();
    }

    protected override void CheckHP()
    {
        // ≈–∂œπ÷ŒÔ «∑ÒÀ¿Õˆ
        if (hp <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("Ú˘Úπ÷±¨’®“Ù–ß");

            // animation
            animator.SetTrigger("GoingToExplode");
        }
    }

    protected override void AddDieAnimationEvent()
    {
        // event: explode
        AnimationEvent explodeEvent = new AnimationEvent();
        explodeEvent.functionName = "ExplodeAfterDie";
        explodeEvent.time = animator.GetClipLength("Die") / 2;


        // event: disable object
        AnimationEvent dieEvent = new AnimationEvent();
        dieEvent.functionName = "DisableAfterDie";
        dieEvent.time = animator.GetClipLength("Die");

        // add event
        animator.AddEvent("Die", explodeEvent);
        animator.AddEvent("Die", dieEvent);
    }

    // interface
    public void ExplodeAfterDie()
    {
        // explode effect
        GameObject bulletObj = PoolManager.GetInstance().GetObj(E_PoolType.ExplodeBullet);
        EnemyReceiveDamage(bulletObj.GetComponent<Bullet>());
        PoolManager.GetInstance().ReturnObj(E_PoolType.ExplodeBullet, bulletObj);
    }
}