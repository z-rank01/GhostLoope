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

    protected override void AddDieAnimationEvent()
    {
        // event: explode
        AnimationEvent explodeEvent = new AnimationEvent();
        explodeEvent.functionName = "ExplodeAfterDie";
        explodeEvent.time = animator.GetClipLength("Die") / 2;
        explodeEvent.objectReferenceParameter = this.gameObject;


        // event: disable object
        AnimationEvent dieEvent = new AnimationEvent();
        dieEvent.functionName = "DisableAfterDie";
        dieEvent.time = animator.GetClipLength("Die");
        dieEvent.objectReferenceParameter = this.gameObject;

        // add event
        animator.AddEvent("Die", explodeEvent);
        animator.AddEvent("Die", dieEvent);
    }

    // interface
    public void ExplodeAfterDie(GameObject targetObj)
    {
        // explode effect
        EnemyBomb enemyBomb = targetObj.GetComponent<EnemyBomb>();
        GameObject bulletObj = PoolManager.GetInstance().GetObj(E_PoolType.ExplodeBullet);
        enemyBomb.EnemyReceiveDamage(bulletObj.GetComponent<Bullet>());
        PoolManager.GetInstance().ReturnObj(E_PoolType.ExplodeBullet, bulletObj);
    }
}