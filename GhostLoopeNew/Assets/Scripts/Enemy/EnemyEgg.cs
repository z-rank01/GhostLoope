using Unity.VisualScripting;
using UnityEngine;

public class EnemyEgg : Enemy
{
    [Header("Egg Spawn")]
    public GameObject egglet;
    public int spawnCounter = 30;

    private float timer;

    protected void Update()
    {
        CheckHP();

        // spawning counter
        Debug.Log("Egg Counter: " +  spawnCounter);
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

        // animator when receiving damage
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
        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void AddSpawnEvent()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        int idx = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == "Spawn") idx = i;
        }
        AnimationEvent spawnEvent = new AnimationEvent();
        spawnEvent.functionName = "SpawnEgglet";
        spawnEvent.time = clips[idx].length;
        spawnEvent.objectReferenceParameter = this.gameObject;
        clips[idx].AddEvent(spawnEvent);
    }


    // interface
    public void SpawnEgglet(GameObject egg)
    {
        EnemyEgg enemyEgg = egg.GetComponent<EnemyEgg>();
        Instantiate(enemyEgg.egglet, egg.transform.position, egg.transform.rotation);
        egg.SetActive(false);
    }
}