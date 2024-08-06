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
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            float dis = (playerObj.transform.position - transform.position).magnitude;
            if (dis <= explodeRadius)
            {
                Player playerScript = playerObj.GetComponent<Player>();
                playerScript.PlayerReceiveDamage(bullet as SpecialBullet);

            }
        }
    }
}