using UnityEngine;

public class BossEgg : Enemy
{
    [Header("Egg Spawn")]
    public GameObject bossShade;
    public int spawnCounter = 30;

    private float timer;



    protected void Update()
    {
        CheckHP();

        // spawning counter
        // Debug.Log("Egg Counter: " + spawnCounter);
        if (timer < spawnCounter)
        {
            timer += Time.deltaTime;
            int seconds = (int)(timer % 60);
        }
        else
        {
            AddSpawnEvent();
            animator.SetTrigger("Spawn");
        }

        // animate when receiving damage
        if (receiveDamage)
        {
            animator.SetBool("Shake", true);
            receiveDamage = false;
        }
        else
            animator.SetBool("Shake", false);
    }

    private void CheckHP()
    {
        // spawn when dead
        if (hp <= 0)
        {
            AddSpawnEvent();
            animator.SetTrigger("Spawn");
        }
    }

    private void AddSpawnEvent()
    {
        AnimationEvent spawnEvent = new AnimationEvent();
        spawnEvent.functionName = "SpawnEgglet";
        spawnEvent.time = animator.GetClipLength("Spawn");
        spawnEvent.objectReferenceParameter = this.gameObject;

        animator.AddEvent("Spawn", spawnEvent);
    }


    // interface
    public void SpawnEgglet(GameObject egg)
    {
        BossEgg enemyEgg = egg.GetComponent<BossEgg>();
        Enemy bossShade = Instantiate(enemyEgg.bossShade, egg.transform.position, egg.transform.rotation).GetComponent<Enemy>();
        bossShade.SetSlider(this.enemyHp, this.enemyRes);
        egg.SetActive(false);
    }
}