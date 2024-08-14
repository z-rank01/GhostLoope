using Unity.VisualScripting;
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
        AddExplodeAnimationEvent();
    }

    protected override void CheckHP()
    {
        if (enemyHp != null)
        {
            enemyHp.value = hp;
        }
        // �жϹ����Ƿ�����
        if (hp <= 0)
        {
            //MusicManager.GetInstance().PlayFireSound("ը���ֱ�ը��Ч");

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
        explodeEvent.objectReferenceParameter = this.gameObject;

        // add event
        animator.AddEvent("Die", explodeEvent);
    }

    // interface
    public void ExplodeAfterDie(GameObject targetObj)
    {
        // explode effect
        if (targetObj != null && targetObj.GetComponent<Enemy>().GetEnemyHP() <= 0)
        {
            //Debug.LogWarning("Bomb enemy exploding!");
            GameObject bulletObj = PoolManager.GetInstance().GetObj(E_PoolType.ExplodeBullet);
            targetObj.GetComponent<EnemyBomb>().EnemyReceiveDamage(bulletObj.GetComponent<Bullet>());
            PoolManager.GetInstance().ReturnObj(E_PoolType.ExplodeBullet, bulletObj);
        }
    }
}