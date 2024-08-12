using System.Collections;
using UnityEngine;


public class BossWraithPreChain : MonoBehaviour
{
    public GameObject chainObject;

    private float chainTime;
    private float preChainTime;
    private float currChainTime = 0.0f;
    private bool hasInstantiateChain = false;

    private void OnDisable()
    {
        hasInstantiateChain = false;
        currChainTime = 0.0f;
    }

    private void Update()
    {
        if (hasInstantiateChain)
        {
            currChainTime += Time.deltaTime;
            if (currChainTime > chainTime)
            {
                gameObject.SetActive(false);
            }
        }
    }
    public void Init(float preChainSeconds, float chainEffectTime)
    {
        preChainTime = preChainSeconds;
        chainTime = chainEffectTime;
    }


    public void StartReleaseChain()
    {
        StartCoroutine(ReleaseChain());
    }

    private IEnumerator ReleaseChain()
    {
        yield return new WaitForSeconds(preChainTime);

        chainObject.SetActive(true);

        hasInstantiateChain = true;
    }
}