using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasurePineapple : MonoBehaviour
{
    public float san;
    public float res;
    public bool isSoul_1;
    public bool isSoul_2;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GlobalSetting.GetInstance().san += san;
            GlobalSetting.GetInstance().resilience += res;
            if (isSoul_1) Player.GetInstance().SetSoul_1(true);
            if (isSoul_2) Player.GetInstance().SetSoul_2(true);
            Destroy(gameObject);
        }
    }
}
