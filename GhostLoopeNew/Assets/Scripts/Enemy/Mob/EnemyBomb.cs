using UnityEngine;
using UnityEngine.Rendering;

public class EnemyBomb : EnemyMob
{
    new protected void OnEnable()
    {
        base.OnEnable();
        // die animation event
    }

    protected void Start()
    {
        //AddExplodeAnimationEvent();
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

    protected void AddExplodeAnimationEvent()
    {
        // event: explode
        AnimationEvent explodeEvent = new AnimationEvent();
        explodeEvent.functionName = "ExplodeAfterDie";
        explodeEvent.time = animator.GetClipLength("Die") / 2;

        // add event
        animator.AddEvent("Die", explodeEvent);
    }

    // interface
    public void ExplodeAfterDie()
    {
        // explode effect
        Debug.LogWarning("Bomb enemy exploding!");
        GameObject bulletObj = PoolManager.GetInstance().GetObj(E_PoolType.ExplodeBullet);
        EnemyReceiveDamage(bulletObj.GetComponent<Bullet>());
        PoolManager.GetInstance().ReturnObj(E_PoolType.ExplodeBullet, bulletObj);
    }
}