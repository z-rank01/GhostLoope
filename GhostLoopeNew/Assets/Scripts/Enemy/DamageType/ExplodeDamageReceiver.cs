using UnityEngine;

public class ExplodeDamageReceiver : MonoBehaviour
{
    public void ReceiveDamage(Bullet bullet)
    {
        // exert exploded damage to enemy
        float explodeRadius = bullet.explodeRadius;
        float explodeDamage = bullet.playerDamage;
        GameObject[] EnemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < EnemyArray.Length; i++)
        {
            if (EnemyArray[i] == null || EnemyArray[i].GetComponent<Enemy>() == this) continue;

            float dis = (EnemyArray[i].transform.position - transform.position).magnitude;
            if (dis <= explodeRadius)
            {
                Enemy enemy = EnemyArray[i].GetComponent<Enemy>();
                enemy.SetEnemyHP(enemy.GetEnemyHP() - explodeDamage);

            }
        }

        // exert exploded damage to player
        if (Vector3.Distance(Player.GetInstance().transform.position, gameObject.transform.position) <= explodeRadius)
        {
            Player.GetInstance().PlayerReceiveDamage(explodeDamage);
        }
    }
}