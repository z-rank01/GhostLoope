using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Diagnostics;

public class Save
{
    public string sceneName;
    public double x, y, z;
    public bool is_Soul1 = false;
    public bool is_Soul2 = false;
    public float san, res; // ���ǵ�����ֵ������ֵ
    public List<int> GraveStoneId = new List<int>(); // Ŀǰ��Ȼ���ڵĿɽ����������ID

    public List<int> SpawnEnemyId = new List<int>();

    public List<int> ComputerId = new List<int>();

    public List<int> EnemyId = new List<int>();
    //public List<bool> EnemyActive = new List<bool>();

    //public List<bool> EnemyHpActive = new List<bool>();
}
