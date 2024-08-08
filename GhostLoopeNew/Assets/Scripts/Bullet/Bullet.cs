using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{

    public E_PoolType bulletType;

    public float damage = 5.0f; // ���﷢���ӵ��Ļ����˺�
    public float extraDamage = 0.0f; // ���﷢���ӵ��Ķ����˺�

    public float extraDamageTime = 0.0f; // �����˺�����ʱ��

    public float playerDamage = 25.0f; // ��ҷ����ӵ����˺�

    public float thunderRadius = 100.0f; // ���������׷�ٵ����Χ

    public float explodeRadius = 100.0f; // ��ը��Χ

    public bool isSwallowed = false; // ���ӵ��Ƿ�����


    public ParticleSystem BulletParticleSystem; // �ӵ���Ч

    public ParticleSystem[] testParticles = new ParticleSystem[5];

    private bool activated = true;
    private float bulletSpeed;
    private Vector3 fireDirection;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        BulletParticleSystem.Play();


        //for (int i = 0; i < testParticles.Length; i++)
        //{
        //    testParticles[i].Play();
        //}


        if (Time.timeScale == 0.0f) return; //��ͣ�ӵ����˶�


        if (activated) Flying();
        if (!CheckWithinScreen())
        {
            Debug.Log("OUT OF SCREEN");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isSwallowed == true) return; // ���ɵ��ӵ�����Ҫ����ײ����

        if (other.gameObject.CompareTag("EnvironmentObject"))
        {
            Debug.Log("Collider Boooommm!!");
            PoolManager.GetInstance().ReturnObj(bulletType, gameObject);
        }

        // if hit enemy
        if (other.gameObject.CompareTag("Enemy") && isSwallowed == false)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                // �����յ������ɵĸ��ӵ����˺�
                Debug.Log("�����յ������ɵĸ��ӵ����˺�: " + playerDamage + " Bullet.IsSwallowed: " + isSwallowed);
                //enemyChasing.ReceiveDamage(playerDamage);

                enemy.EnemyReceiveDamage(this);
                PoolManager.GetInstance().ReturnObj(bulletType, this.gameObject);

                //EventCenter.GetInstance().EventTrigger<float>(E_Event.ReceiveDamage, playerDamage);
            }

        }

        // if hit tenacity
        if (other.gameObject.CompareTag("Tenacirt"))
        {
            Tenacity tenacityShield = other.gameObject.GetComponent<Tenacity>();
            tenacityShield.ReceiveDamage(this);
            PoolManager.GetInstance().ReturnObj(bulletType, this.gameObject);
        }

        ////// ������ӵ��������
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    //PoolManager.GetInstance().ReturnObj(E_PoolType.SimpleBullet, gameObject);
        //    // EventCenter.GetInstance().EventTrigger<SpecialBullet>(E_Event.PlayerReceiveDamage, this);
        //}


        // if hit wall
        if (other.gameObject.CompareTag("Boundary"))
        {
            //Debug.Log("OUT OF BOUNDARY");
            PoolManager.GetInstance().ReturnObj(bulletType, gameObject);
        }
    }


    // interface
    public void FireOut(Vector3 position, Vector3 fireDirection, float bulletSpeed)
    {
        transform.position = position;
        this.fireDirection = fireDirection;
        this.bulletSpeed = bulletSpeed;
    }

    public void Flying()
    {
        transform.position += fireDirection * bulletSpeed;
    }


    public bool CheckWithinScreen()
    {
        Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(transform.position);

        if (viewPortPosition.x >= 0 && viewPortPosition.x <= 1
            &&
            viewPortPosition.y >= 0 && viewPortPosition.y <= 1
            &&
            viewPortPosition.z > 0)
            return true;
        else return false;
    }

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }
}
