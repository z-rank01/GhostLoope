using System.Collections;
using UnityEngine;

public class ContinuousDamageReceiver : MonoBehaviour
{
    public void ReceiveDamage(float deltaTime, float extraDamage, int count, Enemy enemy)
    {
        StartCoroutine(ReceiveExtraDamage(deltaTime, extraDamage, count, enemy));
    }

    // 每deltaTime受到一次伤害，每次伤害为extraDamage，共受到count次伤害
    IEnumerator ReceiveExtraDamage(float deltaTime, float extraDamage, int count, Enemy enemy)
    {
        for (int i = 0; i < count; i++)
        {
            enemy.SetEnemyHP(enemy.GetEnemyHP() - extraDamage);

            // 先立即受到一次额外伤害，然后等待deltaTime
            yield return new WaitForSeconds(deltaTime);
        }
    }
}