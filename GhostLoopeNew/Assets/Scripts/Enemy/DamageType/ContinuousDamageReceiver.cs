using System.Collections;
using UnityEngine;

public class ContinuousDamageReceiver : MonoBehaviour
{
    public void ReceiveDamage(float deltaTime, float extraDamage, int count, Enemy enemy)
    {
        StartCoroutine(ReceiveExtraDamage(deltaTime, extraDamage, count, enemy));
    }

    // ÿdeltaTime�ܵ�һ���˺���ÿ���˺�ΪextraDamage�����ܵ�count���˺�
    IEnumerator ReceiveExtraDamage(float deltaTime, float extraDamage, int count, Enemy enemy)
    {
        for (int i = 0; i < count; i++)
        {
            enemy.SetEnemyHP(enemy.GetEnemyHP() - extraDamage);

            // �������ܵ�һ�ζ����˺���Ȼ��ȴ�deltaTime
            yield return new WaitForSeconds(deltaTime);
        }
    }
}