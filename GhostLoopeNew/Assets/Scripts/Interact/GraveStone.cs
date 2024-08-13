using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveStone : MonoBehaviour
{
    public Transform ConditionDestroy; // ��Ҫ�ȴݻٸ����壬���ܴݻٱ�����

    public ParticleSystem fallenSmoke; // Ĺ�����º󲥷ŵ���Ч

    bool fallen = false; // Ĺ���Ƿ�������ʧ

    public bool isTriggerBossFight = false;
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
            // �����������, Ĺ��ֻ������ҵ��ӵ�����
            if (ConditionDestroy == null && bullet.GetIsFromPlayer() == true)
            {
                Debug.Log(bullet.bulletType + " Hit GraveStone!");


                if (isTriggerBossFight)
                {
                    Player.GetInstance().SetIsFightingBoss(true);
                }


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


            //// ������������
            //GlobalSetting.GetInstance().san += GlobalSetting.GetInstance().treasureSan;
            //GlobalSetting.GetInstance().resilience += GlobalSetting.GetInstance().treasureRes;
        }
    }
}
