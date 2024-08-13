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

            string text = "";
            if (san != 0) text += "san + " + san + "\n";
            if (res != 0) text += "res + " + res + "\n";
            if (isSoul_1) text += "damage * 1.25\n";
            if (isSoul_2) text += "damage * 3\nbullet * 3\n";

            Player.GetInstance().isNeedToShowText = true;
            Player.GetInstance().showText = text;
            Destroy(gameObject);
        }
    }
}
