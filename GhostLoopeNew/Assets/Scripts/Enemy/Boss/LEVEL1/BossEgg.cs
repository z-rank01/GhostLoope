using UnityEngine;
using UnityEngine.UI;

public class BossEgg : EnemyEgg
{
    new protected void OnEnable()
    {
        base.OnEnable();

        // UI
        enemySan = GameObject.Find("Enemy_San").GetComponent<Slider>();
        enemySan.value = enemySan.maxValue = maxHp;
    }

    new protected void Update()
    {
        CheckHP();

        // animate when receiving damage
        if (receiveDamage)
        {
            animator.SetBool("Shake", true);
            receiveDamage = false;
        }
        else
            animator.SetBool("Shake", false);


    }

    protected override void CheckHP()
    {
        // spawn when dead
        if (hp <= 0)
        {
            readyToSpawn = true;
            animator.SetTrigger("Spawn");
        }
    }


    // interface

    public override void Spawn(GameObject targetObj)
    {
        BossEgg bossEgg = targetObj.GetComponent<BossEgg>();
        if (bossEgg != null)
        {
            if (bossEgg.CheckReadyToSpawn())
            {
                MusicManager.GetInstance().PlayFireSound("µ∞ø«ÀÈ¡—");
                Enemy bossShade = Instantiate(spawnObject, transform.position, transform.rotation).GetComponent<Enemy>();
                this.gameObject.SetActive(false);

                //bossShade.SetSlider(bossShade.enemySan, bossShade.enemyRes);


                Player.GetInstance().SetIsFightingBoss(true);
            }
        }
        
    }
}