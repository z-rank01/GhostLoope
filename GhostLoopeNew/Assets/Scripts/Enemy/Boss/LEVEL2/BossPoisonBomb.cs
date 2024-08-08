using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum E_PoisonBombStatus
{
    normal, 
    skill1, 
    skill2, 
    skill3,
    LOCK
}

public class BossPoisonBomb : Enemy
{
    [Header("Boss PoisonBomb Setting")]
    public float enemyWalkSpeed = 4.0f;
    public float enemyRunSpeed = 8.0f;
    public NavMeshAgent agent;
    public float skillCoolDown = 5.0f;

    //private E_ShadowStatus shadowStatus;
    private HashSet<E_ShadowStatus> shadowsStatusContainer;
    private float currSkillTime;

    [Header("Skill 1 Setting")]


    [Header("Skill 2 Setting")]


    [Header("Skill 3 Setting")]



    [Header("Tenacity")]
    public GameObject tenacityObj;
    private Tenacity tenacity;
}