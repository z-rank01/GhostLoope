using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwallowRange : MonoBehaviour
{
    private Bullet swallowedBullet;
    private List<SpecialBullet> specialBullets = new List<SpecialBullet>(); // �������ɷ�Χ�ڵ������ӵ��б�
    private bool readyToFire = false;


    public Transform swallowedBulletPosition; // �����ɵ��ӵ��ŵ�λ��


    // �������ɷ�Χ��������������ɷ�Χ���ӵ������б�
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

    // �˳����ɷ�Χ�����˳����ɷ�Χ�ڵ��ӵ��Ƴ��б�
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
        MusicManager.GetInstance().PlayFireSound("����-����"); 




        if (specialBullets.Count > 0)
        {
            MusicManager.GetInstance().PlayFireSound("����-���ճɹ���ʾ"); 

            // ���ɳɹ���ȥSANֵ
            float SAN = Player.GetInstance().GetProperty(E_Property.san);
            float RES = Player.GetInstance().GetProperty(E_Property.resilience);

            Player.GetInstance().SetProperty(E_Property.san, SAN - 10);
            Player.GetInstance().SetProperty(E_Property.resilience, RES + 10);

            Debug.Log("Swallowing");
            swallowedBullet = specialBullets[0];
            swallowedBullet.Deactivate();

            swallowedBullet.isSwallowed = true; // ��Ϊ�����ɵ��ӵ�

            swallowedBullet.transform.SetParent(swallowedBulletPosition, true);
            swallowedBullet.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            swallowedBullet.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            specialBullets.RemoveAt(0);

            readyToFire = true;
        }
    }

    // �����Ѿ����ɵ������ӵ�
    public void FireSpecial()
    {
        Debug.Log("FireSpecial Before");
        if (swallowedBullet == null) return;

        swallowedBullet.transform.SetParent(null); // �����ӵ�����parent��Ϊnull
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



        MusicManager.GetInstance().PlayFireSound("����-�չ�-���"); // ����ӵ���Ч
        MusicManager.GetInstance().PlayFireSound("����-�չ�-����"); // ����ӵ���Ч
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
