using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Save
{
    public string sceneName;
    public double x, y, z;

    public List<int> GraveStoneId = new List<int>(); // 目前仍然存在的可交互的物体的ID

    public List<int> SpawnEnemyId = new List<int>();

    public List<int> ComputerId = new List<int>();

    public List<int> EnemyId = new List<int>();
    //public List<bool> EnemyActive = new List<bool>();

    //public List<bool> EnemyHpActive = new List<bool>();
}
