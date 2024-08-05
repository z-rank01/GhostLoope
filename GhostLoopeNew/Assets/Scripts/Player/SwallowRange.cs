using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwallowRange : MonoBehaviour
{
    private Bullet swallowedBullet;
    private List<SpecialBullet> specialBullets = new List<SpecialBullet>(); // 进入吞噬范围内的特殊子弹列表
    private bool readyToFire = false;


    public Transform swallowedBulletPosition; // 所吞噬的子弹放的位置


    // 进入吞噬范围，将进入玩家吞噬范围的子弹放入列表
    public void OnTriggerEnter(Collider other)
    {
        SpecialBullet currentBullet = other.GetComponent<SpecialBullet>();
        if (currentBullet != null)
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!In Player OnTriggerEnter: " + other.name);


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

    public void SwallowBullet()
    {
        MusicManager.GetInstance().PlayFireSound("洛普-吸收"); 




        if (specialBullets.Count > 0)
        {
            MusicManager.GetInstance().PlayFireSound("洛普-吸收成功提示"); 

            // 吞噬成功减去SAN值
            float SAN = Player.GetInstance().GetProperty(E_Property.san);
            float RES = Player.GetInstance().GetProperty(E_Property.resilience);

            Player.GetInstance().SetProperty(E_Property.san, SAN - 10);
            Player.GetInstance().SetProperty(E_Property.resilience, RES + 10);

            Debug.Log("Swallowing");
            swallowedBullet = specialBullets[0];
            swallowedBullet.Deactivate();

            swallowedBullet.isSwallowed = true; // 设为已吞噬的子弹

            swallowedBullet.transform.SetParent(swallowedBulletPosition, true);
            swallowedBullet.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

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
        Vector3 fireOrigin = transform.position + fireDirection * 20.0f;



        fireOrigin += new Vector3(0.0f, 5.0f, 0.0f);


        swallowedBullet.Activate();
        swallowedBullet.FireOut(fireOrigin,
                                fireDirection,
                                GlobalSetting.GetInstance().specialBulletSpeed);


        


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
