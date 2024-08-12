using UnityEngine;

public class BossEggMob : EnemyEgg
{
    new private void OnEnable()
    {
        base.OnEnable();
        EventCenter.GetInstance().EventTrigger(E_Event.BossShadeIncreaseMobOnScene);
    }

    private void OnDisable()
    {
        EventCenter.GetInstance().EventTrigger(E_Event.BossShadeDecreaseMobOnScene);
    }
}