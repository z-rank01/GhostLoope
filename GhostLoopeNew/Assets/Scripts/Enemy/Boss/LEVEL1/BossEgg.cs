using UnityEngine;

public class BossEgg : Enemy
{
    [Header("Egg Spawn")]
    public GameObject bossShadeObject;
    public int spawnCounter = 30;

    private float timer;

    new protected void OnEnable()
    {
        base.OnEnable();
        AddSpawnEvent();
    }

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
            animator.SetTrigger("Spawn");
        }
    }

    private void AddSpawnEvent()
    {
        AnimationEvent spawnEvent = new AnimationEvent();
        spawnEvent.functionName = "SpawnEgglet";
        spawnEvent.time = animator.GetClipLength("Spawn");

        animator.AddEvent("Spawn", spawnEvent);
    }


    // interface
    public void SpawnEgglet()
    {
        Enemy bossShade = Instantiate(bossShadeObject, transform.position, transform.rotation).GetComponent<Enemy>();
        bossShade.SetSlider(this.enemyHp, this.enemyRes);
        gameObject.SetActive(false);
    }
}