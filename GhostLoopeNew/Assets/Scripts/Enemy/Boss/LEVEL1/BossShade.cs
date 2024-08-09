using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;
using UnityEngine.AI;


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
    public NavMeshAgent enemyAgent;

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
        enemyAgent = gameObject.AddComponent<NavMeshAgent>();
        enemyAgent.stoppingDistance = 2.0f;
        status1Destination = GameObject.FindGameObjectWithTag("Destination");
        enemyAgent.SetDestination(status1Destination.transform.position);
        enemyAgent.speed = enemyRunSpeed;

        // skill event
        EventCenter.GetInstance().AddEventListener(E_Event.BossShadeStatus2Skill, DecreaseMobOnScene);
        currSkillTime = skillCoolDown;

        // animation event
        AddDieAnimationEvent();
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
    private void SetMoveTo(GameObject destination, float speed)
    {
        enemyAgent.SetDestination(destination.transform.position);
        enemyAgent.speed = speed;
    }

    private void CheckReachDestination()
    {
        if (Vector3.Distance(status1Destination.transform.position, transform.position) <= enemyAgent.stoppingDistance)
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
        if (enemyAgent == null) Debug.LogError("Could not find AI agent!");
        Vector3 playerPosition = Player.GetInstance().GetPlayerTransform().position;
        float distance = (playerPosition - transform.position).magnitude;
        return distance;
    }

    private void CheckHP()
    {
        // ≈–∂œπ÷ŒÔ «∑ÒÀ¿Õˆ
        if (hp <= 0)
        {
            MusicManager.GetInstance().PlayFireSound("Ú˘Úπ÷±¨’®“Ù–ß");

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
        Vector3 direction2 = direction.normalized;
        Vector3 direction3 = direction - fluctuation;

        return new List<Vector3> { direction1, direction2, direction3 };
    }

    private void SkillSpawnMob()
    {
        Vector3 playerDirection = Player.GetInstance().GetPlayerTransform().position - transform.position;
        var directions = FindFluctuateDirection(playerDirection.normalized, directionNoise);
        for (int i = 0; i < mobNumber; i++)
        {
            GameObject mobObj = Instantiate(mobGameObject);
            Rigidbody mobRB = mobObj.GetComponent<Rigidbody>();
            mobRB.AddForce(directions[i] * forceMagnitude);
        }
        mobOnScene = mobNumber;
    }

    private bool CheckMobOnScene()
    {
        return mobOnScene == 0 ? false : true;
    }


    // update function
    private void Status1Update()
    {
        CheckReachDestination();
    }

    private void Status2Update()
    {
        if (currFireDelay > 0) currFireDelay -= Time.deltaTime;
        if (currSkillTime > 0) currSkillTime -= Time.deltaTime;
        
        //Debug.Log("In Enemy_Chasing Update");
        float currDistance = GetPlayerDistance();

        // chasing
        if (currDistance > enemyAgent.stoppingDistance)
        {
            enemyAgent.SetDestination(Player.GetInstance().transform.position);
            enemyAgent.speed = enemyWalkSpeed;

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
                SkillSpawnMob();
            currSkillTime = skillCoolDown;
        }
        

        CheckHP();
    }

    // interface
    public void DecreaseMobOnScene()
    {
        mobOnScene -= mobOnScene-- < 0 ? 0 : 1;
    }


    public void DisableAfterDie(GameObject targetObj)
    {
        targetObj.SetActive(false);
        enemyAgent.enabled = false;
    }

    public void SpawnNextStageBoss(GameObject targetObj)
    {
        BossShade bossShade = targetObj.GetComponent<BossShade>();
        Enemy bossShadow = Instantiate(bossShade.nextStageBossObject, targetObj.transform.position, targetObj.transform.rotation).GetComponent<Enemy>();
        bossShadow.SetSlider(this.enemyHp, this.enemyRes);
        targetObj.SetActive(false);
    }
}