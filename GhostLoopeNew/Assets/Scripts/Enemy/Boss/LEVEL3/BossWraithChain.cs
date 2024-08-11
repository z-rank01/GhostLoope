using UnityEngine;

public class BossWraithChain : MonoBehaviour
{
    private bool chainedPlayer = false;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other != null && other.tag == "Player")
    //    {
    //        chainedPlayer = true;
    //        EventCenter.GetInstance().EventTrigger(E_Event.BossWraithChainSuccess);
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other != null && other.tag == "Player")
        {
            EventCenter.GetInstance().EventTrigger(E_Event.BossWraithChainSuccess);
            chainedPlayer = true;
        }
        
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other != null && other.tag == "Player")
    //    {
    //        EventCenter.GetInstance().EventTrigger(E_Event.BossWraithChainSuccess);
    //        chainedPlayer = true;
    //    }
    //}

    private void OnDisable()
    {
        if (chainedPlayer)
            chainedPlayer = false;
        else 
            EventCenter.GetInstance().EventTrigger(E_Event.BossWraithChainFail);
    }
}