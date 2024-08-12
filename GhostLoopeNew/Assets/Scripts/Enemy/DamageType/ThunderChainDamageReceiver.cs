using UnityEngine;

public class ThunderChainDamageReceiver : MonoBehaviour
{
    [SerializeField]
    private float chainColdDown;
    private float currChainTime;
    private bool isAlreadyChained = false; // ��ǰ�����Ƿ��Ѿ��յ������������˺�

    private void Update()
    {
        // enemy's inner colddown of damage receiving
        if (isAlreadyChained)
        {
            currChainTime -= Time.deltaTime;
        }
        if (currChainTime <= 0)
        {
            currChainTime = chainColdDown;
            isAlreadyChained = false;
        }
    }

    // interface

    /// <summary>
    /// Thunder chain damage: could reflect to other enemy in the same type
    /// </summary>
    /// <param name="count">chained enemy counter</param>
    /// <param name="damage">chain damage</param>
    /// <param name="thunderRadius">chain influence radius</param>
    /// <param name="enemy">next chained enemy</param>
    public void ReceiveDamage(int count, float damage, float thunderRadius, Enemy enemy)
    {
        if (count <= 0) return;

        // set status
        isAlreadyChained = true; // ���ϱ�ǣ���ֹ�ظ��ܵ��˺�

        // get damage
        float hp = enemy.GetEnemyHP();
        hp -= hp - damage >= 0 ? damage : hp;
        enemy.SetEnemyHP(hp);

        // exert chain damage
        GameObject[] EnemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistance = 100000000.0f;
        int id = -1;
        for (int i = 0; i < EnemyArray.Length; i++)
        {
            ThunderChainDamageReceiver thunderChainDamageReceiver = EnemyArray[i].GetComponent<ThunderChainDamageReceiver>();
            if (EnemyArray[i] == null || thunderChainDamageReceiver.IsAlreadyChained()) continue;

            //Debug.Log("EnemyArray[i]: " + EnemyArray[i].name);
            float dis = (EnemyArray[i].transform.position - transform.position).magnitude;
            //Debug.Log("distance: " + dis);

            // �ҵ����뵱ǰ���������һ������
            if (dis <= thunderRadius && dis < minDistance)
            {
                minDistance = dis;
                id = i;
            }
        }

        if (id != -1)
        {
            //Debug.Log("Next Thunder Enemy:  " + EnemyArray[id].name);
            ThunderChainDamageReceiver thunderChainDamageReceiver = EnemyArray[id].GetComponent<ThunderChainDamageReceiver>();
            if (thunderChainDamageReceiver != null)
                thunderChainDamageReceiver.ReceiveDamage(count - 1, damage, thunderRadius, EnemyArray[id].GetComponent<Enemy>());
        }

    }

    public bool IsAlreadyChained()
    {
        return isAlreadyChained;
    }


    
}