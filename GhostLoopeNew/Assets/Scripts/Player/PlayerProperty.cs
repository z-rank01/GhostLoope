using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty : MonoBehaviour
{
    [SerializeField]
    private float san;
    [SerializeField]
    private float resilience;
    [SerializeField]
    private float speed;

    private void Awake()
    {
        
    }

    // interface
    public float GetSpeed() { return speed; }
}
