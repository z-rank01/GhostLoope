using Unity.VisualScripting;
using UnityEngine;

public class EnemyEgg : Enemy
{
    [Header("Egg Spawn")]
    public GameObject eggletObject;
    public int spawnCounter = 30;

    private float timer;

    new protected void OnEnable()
    {
        base.OnEnable();
    }

    protected void Update()
    {
        CheckHP();

        // spawning counter
        // Debug.Log("Egg Counter: " +  spawnCounter);
        if (timer < spawnCounter)
        {
            timer += Time.deltaTime;
            int seconds = (int)(timer % 60);
        }
        else
        {
            animator.SetTrigger("Spawn");
        }

        // animator when receiving damage
        if (receiveDamage)
        {
            animator.SetBool("Shake", true);
            receiveDamage = false;
        }
        else
            animator.SetBool("Shake", false);
    }

    protected virtual void CheckHP()
    {
        if (hp <= 0)
        {
            gameObject.SetActive(false);
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
        Instantiate(eggletObject, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}