using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUnit : CommandableUnit
{
    //getter只讀屬性，供外部獲取使用
    public int QueueSize => buildingQueue.Count;
    public UnitSO[] Queue => buildingQueue.ToArray();

    //public屬性，可供外部獲取；並且只有本類能修改
    public float CurrentBuildStartTime { get; private set; } //當前正在生產的開始時間
    public float CurrentBuildEndTime => CurrentBuildStartTime + CurrentBuildingUnitSO.BuildTime;
    public UnitSO CurrentBuildingUnitSO { get; private set; } //當前正生產的單位資料
    
    //事件的委派類別與其方法定義
    public delegate void QueueUpdateEvent(UnitSO[] unitsInQueue);
    public event QueueUpdateEvent OnQueueUpdated; // evet關鍵字表示只有這個類別可以raise這個event

    //私有屬性
    private const int MAX_QUEUE_SIZE = 5; //佇列的最大限制
    private List<UnitSO> buildingQueue = new(MAX_QUEUE_SIZE); //生產佇列
    public void BuildUnit(UnitSO unit)
    {
        if (buildingQueue.Count == MAX_QUEUE_SIZE)
        {
            Debug.LogError("生產佇列已經滿了!");
            return;
        }
        buildingQueue.Add(unit); //把要生產的單位加入佇列
        if (buildingQueue.Count == 1)
        {
            //開始一個Coroutine
            StartCoroutine(DoBuildUnits());
        }
        else //0不會進這裡，因為按下BuildUnit肯定有東西在Queue中
        {
            EmitOnQueueUpdated(); // 發出QueueUpdate的通知
        }
    }

    public void CancelBuildingUnit(int index)
    {
        if(index < 0 || index >= buildingQueue.Count)
        {
            Debug.LogError($"輸入的{index}不在buildingQueue的Size : {buildingQueue.Count}範圍內");
        }
        buildingQueue.RemoveAt(index);
        if(index == 0)
        {
            //只有一個時，直接取消該coroutine就可以
            StopAllCoroutines(); //coroutines也只會有一個，所以直接全取消即可
            if(buildingQueue.Count>0) //如果queue中還有東西
            {
                //開始另一個製作過程
                StartCoroutine(DoBuildUnits());
            }
            else //如果陣列中沒東西
            {
                EmitOnQueueUpdated();
            }
        }
        else{
            EmitOnQueueUpdated();
        }
    }

    //製做一個Coroutine方法
    private IEnumerator DoBuildUnits()
    {
        //當佇列中有東西時
        while (buildingQueue.Count > 0)
        {
            CurrentBuildingUnitSO = buildingQueue[0];
            CurrentBuildStartTime = Time.time;
            EmitOnQueueUpdated(); // 發出QueueUpdate的通知, 讓進度條物件可以正確讀取
            //看到yield就會告訴unity，你先執行return 那段的事情
            yield return new WaitForSeconds(CurrentBuildingUnitSO.BuildTime);
            //完成後再回來繼續執行以下的東西
            Instantiate(CurrentBuildingUnitSO.Prefab, transform.position, Quaternion.identity);
            //永遠只剔除第一個
            buildingQueue.RemoveAt(0);
        }
        EmitOnQueueUpdated(); //最後沒東西的時候也要發送通知
    }

    private void EmitOnQueueUpdated()
    {
        OnQueueUpdated?.Invoke(buildingQueue.ToArray());
    }
}
