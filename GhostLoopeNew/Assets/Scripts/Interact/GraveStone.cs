using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveStone : MonoBehaviour
{
    public Transform ConditionDestroy; // 需要先摧毁该物体，才能摧毁本物体

    public ParticleSystem fallenSmoke; // 墓碑倒下后播放的特效

    bool fallen = false; // 墓碑是否正在消失

    //public bool isTriggerBossFight = false;
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fallen)
        {
            Debug.Log("GraveStene Position: " + transform.position);
            transform.position -= new Vector3(0, 0.1f, 0);
        }
    }


    IEnumerator PlaySmokeParticle()
    {
        fallenSmoke.Play();
        yield return new WaitForSeconds(1.0f);
        fallenSmoke.Stop();
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        
        if (bullet != null)
        {
            Debug.Log("In GraveStone OnTriggerEnter: " );
            PoolManager.GetInstance().ReturnObj(bullet.bulletType, bullet.gameObject);
            // 满足击破条件, 墓碑只能由玩家的子弹打破
            if (ConditionDestroy == null && bullet.GetIsFromPlayer() == true)
            {
                Debug.Log(bullet.bulletType + " Hit GraveStone!");


                //if (isTriggerBossFight)
                //{
                //    Player.GetInstance().SetIsFightingBoss(true);
                //}


                fallen = true;

                if (fallenSmoke != null)
                {
                    StartCoroutine(PlaySmokeParticle());
                }
                else
                {
                    Destroy(gameObject);
                }
            }


            //// 增加属性上限
            //GlobalSetting.GetInstance().san += GlobalSetting.GetInstance().treasureSan;
            //GlobalSetting.GetInstance().resilience += GlobalSetting.GetInstance().treasureRes;
        }
    }
}
