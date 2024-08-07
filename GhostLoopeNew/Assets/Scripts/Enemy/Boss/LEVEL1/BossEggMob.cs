using UnityEngine;

public class BossEggMob : EnemyEgg
{
    protected override void CheckHP()
    {
        if (hp <= 0)
        {
            EventCenter.GetInstance().EventTrigger(E_Event.BossShadeStatus2Skill);
            gameObject.SetActive(false);
        }
    }
}