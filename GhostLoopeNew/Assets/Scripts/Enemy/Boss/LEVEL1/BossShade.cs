using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.UI;
public enum E_BossShadeStatus
{
    Status1, 
    Status2
}

public class BossShade : Enemy
{
    [Header("Boss Shade Setting")]
    // general variable
    public float enemyWalkSpeed = 4.0f;
    public float enemyRunSpeed = 8.0f;
    public NavMeshAgent agent;

    private float currFireDelay = 0.0f;
    private E_BossShadeStatus enemyStatus = E_BossShadeStatus.Status1;

    [Header("Boss Shade Status1")]
    // status1 variable
    public GameObject status1Destination;

    [Header("Boss Shade Status2")]
    // status2 variable
    public GameObject mobGameObject;
    public GameObject nextStageBossObject;
    public int mobNumber = 3;
    public float directionNoise = 1.0f;
    public float forceMagnitude = 1.0f;
    public float skillCoolDown = 5.0f;

    private int mobOnScene;
    private float currSkillTime;

    new protected void OnEnable()
    {
        //Debug.Log("In ChasingStart");
        base.OnEnable();

        // ai
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.stoppingDistance = 2.0f;
        status1Destination = GameObject.FindGameObjectWithTag("Destination");
        agent.SetDestination(status1Destination.transform.position);
        agent.speed = enemyRunSpeed;

        // skill event
        EventCenter.GetInstance().AddEventListener(E_Event.BossShadeDecreaseMobOnScene, DecreaseMobOnScene);
        EventCenter.GetInstance().AddEventListener(E_Event.BossShadeIncreaseMobOnScene, IncreaseMobOnScene);
        currSkillTime = skillCoolDown;

        // animation event
        AddDieAnimationEvent();

        // UI
        enemySan = GameObject.Find("Enemy_San").GetComponent<Slider>();
        enemySan.value = enemySan.maxValue = maxHp;
        enemyRes = GameObject.Find("Enemy_Res").GetComponent<Slider>();
        enemyRes.value = enemyRes.maxValue = 0;

        // music
        MusicManager.GetInstance().PlayFireSound("BOSS1-1ʩ����Ч");
    }


    protected void Update()
    {
        switch (enemyStatus)
        {
            case E_BossShadeStatus.Status1:
                Status1Update();
                break;
            case E_BossShadeStatus.Status2:
                Status2Update();
                break;
        }

    }


    // private function

    // general function

    private void CheckReachDestination()
    {
        if (Vector3.Distance(status1Destination.transform.position, transform.position) <= agent.stoppingDistance)
        {
            // change enemy agent's status
            enemyStatus = E_BossShadeStatus.Status2;

            // animate
            moveFrame -= Time.deltaTime * 10;  // run stop faster than walk
            moveFrame = moveFrame < 0 ? 0 : moveFrame;
            animator.SetFloat("Move", moveFrame);
        }
        else
        {
            // animate
            moveFrame += Time.deltaTime;
            moveFrame = moveFrame > 1 ? 1 : moveFrame;
            animator.SetFloat("Move", moveFrame);
        }
           
    }

    private float GetPlayerDistance()
    {
        if (agent == null) Debug.LogError("Could not find AI agent!");
        Vector3 playerPosition = Player.GetInstance().GetPlayerTransform().position;
        float distance = (playerPosition - transform.position).magnitude;
        return distance;
    }

    private void CheckHP()
    {
        if (enemySan) enemySan.value = hp;

        // �жϹ����Ƿ�����
        if (hp <= 0)
        {
            // MusicManager.GetInstance().PlayFireSound("��ը��Ч");

            // animation
            animator.SetTrigger("Die");
        }
    }

    private void AddDieAnimationEvent()
    {
        // event: disable object
        AnimationEvent dieEvent = new AnimationEvent();
        dieEvent.functionName = "DisableAfterDie";
        dieEvent.time = animator.GetClipLength("Die");
        dieEvent.objectReferenceParameter = this.gameObject;

        // event: spawn next stage of Boss
        AnimationEvent spawnEvent = new AnimationEvent();
        spawnEvent.functionName = "SpawnNextStageBoss";
        spawnEvent.time = animator.GetClipLength("Die") / 2;
        spawnEvent.objectReferenceParameter = this.gameObject;

        // add event
        animator.AddEvent("Die", dieEvent);
        animator.AddEvent("Die", spawnEvent);
    }


    // skill 1 function
    private List<Vector3> FindFluctuateDirection(Vector3 direction, float noise)
    {
        Vector3 fluctuation = new Vector3(Random.Range(-noise, noise),
                                          Random.Range(0, noise), 
                                          Random.Range(-noise, noise));
        Vector3 direction1 = direction + fluctuation;
        Vector3 direction2 = direction;
        Vector3 direction3 = direction - fluctuation;

        return new List<Vector3> { direction1, direction2, direction3 };
    }

    private void SkillSpawnMob()
    {
        MusicManager.GetInstance().PlayFireSound("boss�ٻ��������������Ч");
        Vector3 playerDirection = Player.GetInstance().GetPlayerTransform().position - transform.position;
        var directions = FindFluctuateDirection(playerDirection.normalized, directionNoise);
        for (int i = 0; i < mobNumber; i++)
        {
            GameObject mobObj = Instantiate(mobGameObject, transform.position, Quaternion.identity);
            Rigidbody mobRB = mobObj.GetComponent<Rigidbody>();
            mobRB.AddForce(directions[i] * forceMagnitude);
        }
    }

    private bool CheckMobOnScene()
    {
        return mobOnScene != 0;
    }


    // update function
    private void Status1Update()
    {
        CheckReachDestination();
    }

    private void Status2Update()
    {
        if (currFireDelay > 0) currFireDelay -= Time.deltaTime;
        if (!CheckMobOnScene())
        {
            if (currSkillTime > 0) currSkillTime -= Time.deltaTime;
        }
        
        //Debug.Log("In Enemy_Chasing Update");
        float currDistance = GetPlayerDistance();

        // chasing
        if (currDistance <= alertDistance)
        {
            agent.SetDestination(Player.GetInstance().transform.position);
            agent.speed = enemyWalkSpeed;

            // animate
            moveFrame += Time.deltaTime;
            moveFrame = moveFrame > 0.5f ? 0.5f : moveFrame;
            animator.SetFloat("Move", moveFrame);
        }
        else
        {
            // animate
            moveFrame -= Time.deltaTime;
            moveFrame = moveFrame < 0 ? 0 : moveFrame;
            animator.SetFloat("Move", moveFrame);
        }

        // simple attack
        if (currDistance <= attackDistance)
        {
            if (currFireDelay <= 0)
            {
                animator.SetBool("Attack", true);
                SimpleFire();
                currFireDelay = fireCooldown;
            }
            else animator.SetBool("Attack", false);
        }
        else
        {
            animator.SetBool("Attack", false);
        }

        // animator when receiving damage
        if (receiveDamage)
        {
            animator.SetBool("TakeDamage", true);
            receiveDamage = false;
        }
        else
            animator.SetBool("TakeDamage", false);

        // skill
        if (currSkillTime <= 0)
        {
            if (!CheckMobOnScene())
            {
                SkillSpawnMob();
                currSkillTime = skillCoolDown;
            }
        }
        

        CheckHP();
    }

    // interface
    public void DecreaseMobOnScene()
    {
        mobOnScene--;
        if (mobOnScene < 0) mobOnScene = 0;
    }

    public void IncreaseMobOnScene()
    {
        mobOnScene++;
    }


    public void DisableAfterDie(GameObject targetObj)
    {
        if (targetObj.GetComponent<Enemy>().GetEnemyHP() <= 0)
        {
            targetObj.SetActive(false);
            agent.enabled = false;
        }
    }

    public void SpawnNextStageBoss(GameObject targetObj)
    {
        if (targetObj.GetComponent<Enemy>().GetEnemyHP() <= 0)
        {
            BossShade bossShade = targetObj.GetComponent<BossShade>();
            targetObj.SetActive(false);
            Enemy bossShadow = Instantiate(bossShade.nextStageBossObject, targetObj.transform.position, targetObj.transform.rotation).GetComponent<Enemy>();

            //bossShadow.SetSlider(bossShadow.enemySan, bossShadow.enemyRes);
        }
    }
}