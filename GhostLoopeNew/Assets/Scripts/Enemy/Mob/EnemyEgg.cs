using Unity.VisualScripting;
using UnityEngine;

public class EnemyEgg : Enemy
{
    [Header("Egg Spawn")]
    public GameObject spawnObject;
    public int spawnCounter = 5;

    protected bool readyToSpawn = false;
    protected float timer;

    new protected void OnEnable()
    {
        base.OnEnable();
        AddSpawnEvent();
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
            readyToSpawn = true;
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
        spawnEvent.functionName = "Spawn";
        spawnEvent.time = animator.GetClipLength("Spawn");
        spawnEvent.objectReferenceParameter = this.gameObject;
        
        animator.AddEvent("Spawn", spawnEvent);
    }


    // interface
    public bool CheckReadyToSpawn()
    {
        return readyToSpawn;
    }

    public virtual void Spawn(GameObject targetObj)
    {
        EnemyEgg enemyEgg = targetObj.GetComponent<EnemyEgg>();
        if (enemyEgg != null)
        {
            if (enemyEgg.CheckReadyToSpawn())
            {
                Instantiate(spawnObject, transform.position, transform.rotation);
                gameObject.SetActive(false);
            }
        }
    }
}