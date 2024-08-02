using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy_Chasing : Enemy
{

    public NavMeshAgent enemyAgent;

    //public Slider ChasingHP;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("In ChasingStart");
        base.Start();

        enemyAgent = gameObject.AddComponent<NavMeshAgent>();
        enemyAgent.stoppingDistance = 2.0f;

        //EventCenter.GetInstance().AddEventListener<float>(E_Event.ReceiveDamage, this.ReceiveDamage);
        //ChasingHP = Instantiate();


    }


    //void OnTriggerEnter(Collider other)
    //{
    //    Bullet bullet = other.GetComponent<Bullet>();

    //    if (bullet != null)
    //    {
    //        PoolManager.GetInstance().ReturnObj(bullet.bulletType, bullet.gameObject);

    //        ReceiveDamage(bullet.playerDamage);

    //    }
    //}




    public void ReceiveDamage(float damage)
    {
        if (Enemy_HP == null) return;


        Debug.Log("InReceiveDamage");
        HP -= damage;
        Enemy_HP.value -= damage;
        if (HP <= 0)
        {
            GameObject.Destroy(gameObject);
            GameObject.Destroy(Enemy_HP.gameObject);
        }
    }

    public void EnemyFire()
    {
        Debug.Log("EnemyFire!");


        Vector3 playerPosition = Player.GetInstance().transform.position;


        // Set fire direction
        Vector3 fireDirection = playerPosition - transform.position;
        fireDirection = Vector3.Normalize(fireDirection);
        fireDirection.y = 0;

        // Set fire origin
        Vector3 fireOrigin = transform.position + fireDirection * 20.0f;

        SpecialBullet bullet = PoolManager.GetInstance().GetObj(EnemyBulletType).GetComponent<SpecialBullet>();
        bullet.bulletType = EnemyBulletType;


        bullet.Activate();


        bullet.FireOut(fireOrigin,
                       fireDirection,
                       GlobalSetting.GetInstance().enemyBulletSpeed);
    }


    // Update is called once per frame
    void Update()
    {
        if (player == null || enemyAgent == null) return;
        Vector3 playerPosition = player.transform.position;
        float distance = (playerPosition - transform.position).magnitude;


        if (CurFireDelay > 0) CurFireDelay -= Time.deltaTime;

        // ½øÈë¾¯½ä·¶Î§ÄÚ£¬×·×ÙÍæ¼Ò

        if (IsFollowPlayer || distance <= AlertDistance)
        {
            IsFollowPlayer = true;
            enemyAgent.speed = 10;
            enemyAgent.SetDestination(player.transform.position);
        }

        // ½øÈë¹¥»÷·¶Î§ÄÚ£¬Ö´ÐÐ¹¥»÷Âß¼­
        if (distance <= AttackDistance)
        {
            Debug.Log("Enemy_Chasing Attack");


            if (CurFireDelay <= 0)
            {
                EnemyFire();
                CurFireDelay = FireDelay;
            }
        }


        // Ëæ»úÒÆ¶¯
        //if (state == EnemyState.MovingState)
        //{
        //    if (enemyAgent.remainingDistance <= 0)
        //    {
        //        Debug.Log("Start Move");
        //        enemyAgent.SetDestination(FindRandomPosition());
        //    }
        //}
    }
}
