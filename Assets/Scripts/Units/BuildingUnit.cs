using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnit : CommandableUnit
{
    private const int MAX_QUEUE_SIZE = 5; //佇列的最大限制
    private Queue<UnitSO> buildingQueue = new(MAX_QUEUE_SIZE); //生產佇列
    public void BuildUnit(UnitSO unit)
    {
        if (buildingQueue.Count == MAX_QUEUE_SIZE)
        {
            Debug.LogError("生產佇列已經滿了!");
            return;
        }
        buildingQueue.Enqueue(unit); //把要生產的單位加入佇列
        if (buildingQueue.Count == 1)
        {
            //開始一個Coroutine
            StartCoroutine(DoBuildUnits());
        }
    }

    //製做一個Coroutine方法
    private IEnumerator DoBuildUnits()
    {
        //當佇列中有東西時
        while (buildingQueue.Count > 0)
        {
            //使用queue中的Peek()方法，返回queue中的第一個元素
            UnitSO unit = buildingQueue.Peek();
            //看到yield就會告訴unity，你先執行return 那段的事情
            yield return new WaitForSeconds(unit.BuildTime);
            //完成後再回來繼續執行以下的東西
            Instantiate(unit.Prefab, transform.position, Quaternion.identity);
            //剔除queu
            buildingQueue.Dequeue();
        }
    }
}
