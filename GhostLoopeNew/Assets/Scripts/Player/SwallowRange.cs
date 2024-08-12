using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwallowRange : MonoBehaviour
{
    public Bullet swallowedBullet;
    private List<SpecialBullet> specialBullets = new List<SpecialBullet>(); // 进入吞噬范围内的特殊子弹列表
    private bool readyToFire = false;


    public Transform swallowedBulletPosition; // 所吞噬的子弹放的位置


    // 进入吞噬范围，将进入玩家吞噬范围的子弹放入列表
    public void OnTriggerEnter(Collider other)
    {
        SpecialBullet currentBullet = other.GetComponent<SpecialBullet>();
        if (currentBullet != null)
        {
            //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!In Player OnTriggerEnter: " + other.name);


            //Debug.Log(other.gameObject.GetComponent<SpecialBullet>().bulletType);
            specialBullets.Add(other.GetComponent<SpecialBullet>());
        }
    }

    // 退出吞噬范围，将退出吞噬范围内的子弹移除列表
    public void OnTriggerExit(Collider other)
    {
        // Debug.Log("In Player OnTriggerExit: " + other.name);
        SpecialBullet currentBullet = other.GetComponent<SpecialBullet>();
        if (currentBullet != null && specialBullets.Contains(currentBullet))
        {
            specialBullets.Remove(currentBullet);
        }
    }
    public void Update()
    {
        GetComponent<SphereCollider>().radius = Player.GetInstance().GetSwallowRadius();
    }
    public void SwallowBullet()
    {
        MusicManager.GetInstance().PlayFireSound("洛普-吸收"); 

        if (specialBullets.Count > 0)
        {
            MusicManager.GetInstance().PlayFireSound("洛普-吸收成功提示"); 

            // 吞噬成功减去SAN值
            float SAN = Player.GetInstance().GetProperty(E_Property.san);
            float RES = Player.GetInstance().GetProperty(E_Property.resilience);

            float DecreaseSan = Player.GetInstance().GetSwallowDecreaseSan();
            float IncreaseRes = Player.GetInstance().GetSwallowIncreaseRes();

            Player.GetInstance().SetProperty(E_Property.san, SAN - DecreaseSan);
            Player.GetInstance().SetProperty(E_Property.resilience, RES + IncreaseRes);

            StartCoroutine(Player.GetInstance().GettingHurt()); // 受到伤害，开始计时


            Debug.Log("Swallowing");
            swallowedBullet = specialBullets[0];
            swallowedBullet.Deactivate();

            swallowedBullet.isSwallowed = true; // 设为已吞噬的子弹

            swallowedBullet.transform.SetParent(swallowedBulletPosition, true);
            swallowedBullet.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);


            // 让吞噬的子弹的尾迹能向鼠标的相反方向发射
            swallowedBullet.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            swallowedBullet.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            specialBullets.RemoveAt(0);

            readyToFire = true;
        }
    }

    // 发射已经吞噬的特殊子弹
    public void FireSpecial()
    {
        Debug.Log("FireSpecial Before");
        if (swallowedBullet == null) return;

        swallowedBullet.transform.SetParent(null); // 发射子弹，将parent设为null
        swallowedBullet.isSwallowed = false;
        swallowedBullet.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;


        Debug.Log("FireSpecial End");

        // Get mouse world direction
        Vector3 screenWorldPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseScreenPostion = Mouse.current.position.ReadValue();
        mouseScreenPostion.z = screenWorldPos.z;
        Vector3 mouseWorldPostion = Camera.main.ScreenToWorldPoint(mouseScreenPostion);

        // Set fire direction
        Vector3 fireDirection = mouseWorldPostion - transform.position;
        fireDirection = Vector3.Normalize(fireDirection);
        fireDirection.y = 0;

        // Set fire origin
        Vector3 fireOrigin = transform.position + fireDirection * 1.0f;



        fireOrigin += new Vector3(0.0f, 1.0f, 0.0f);


        swallowedBullet.Activate();


        swallowedBullet.SetIsFromPlayer(true); // 标记为玩家打出的子弹
        swallowedBullet.FireOut(fireOrigin,
                                fireDirection,
                                GlobalSetting.GetInstance().specialBulletSpeed);


        // 如果含有第二关Boss的灵魂，则可以发射三枚特殊子弹
        if (Player.GetInstance().GetSoul_2())
        {
            Debug.Log("Fire Triple Bullet!");

            Bullet bullet1 = PoolManager.GetInstance().GetObj(swallowedBullet.bulletType).GetComponent<Bullet>();
            Bullet bullet2 = PoolManager.GetInstance().GetObj(swallowedBullet.bulletType).GetComponent<Bullet>();
            bullet1.SetIsFromPlayer(true); // 标记为玩家打出的子弹
            bullet2.SetIsFromPlayer(true); // 标记为玩家打出的子弹


            bullet1.FireOut(fireOrigin + fireDirection * 1,
                            fireDirection,
                            GlobalSetting.GetInstance().specialBulletSpeed);

            bullet2.FireOut(fireOrigin + fireDirection * 2,
                            fireDirection,
                            GlobalSetting.GetInstance().specialBulletSpeed);
        }


        swallowedBullet = null;
        readyToFire = false;



        MusicManager.GetInstance().PlayFireSound("洛普-普攻-射出"); // 添加子弹音效
        MusicManager.GetInstance().PlayFireSound("洛普-普攻-飞行"); // 添加子弹音效
    }

    public bool ReadyToFire()
    {
        return readyToFire;
    }

    public void RemoveBulleet(SpecialBullet bullet)
    {
        if (specialBullets.Contains(bullet))
        {
            specialBullets.Remove(bullet);
        }
    }
}
