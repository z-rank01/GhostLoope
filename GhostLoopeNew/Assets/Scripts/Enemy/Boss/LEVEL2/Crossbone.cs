using System.Collections;
using UnityEngine;

public class Crossbone : MonoBehaviour
{
    public float boomerangDamage = 35.0f;


    protected void Start()
    {
        
    }

    protected void Update()
    {
        
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player.GetInstance().PlayerReceiveDamage(boomerangDamage);
        }
    }
}