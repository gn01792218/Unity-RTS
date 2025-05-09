using System;
using System.Collections;
using UnityEngine;

public class BuildBuildingUI : MonoBehaviour, IUIElement<BuildingUnit>
{
    [SerializeField] private BuildingQueueButton[] cancleUnitBuildButtons;
    [SerializeField] private ProgressBar progressBar;

    private Coroutine currentBuildCoroutine;
    private BuildingUnit currentBuilding; //當前選擇的建築物
    public void Disable()
    {
        if(currentBuilding != null) currentBuilding.OnQueueUpdated-= HandleQueueUpdated;
        gameObject.SetActive(false); //關閉這個UI容器
        currentBuilding = null;
        currentBuildCoroutine = null;
    }

    public void EnableFor(BuildingUnit selectedBuilding)
    {
        //先重置進度條
        progressBar.SetProgress(0);
        gameObject.SetActive(true); //開啟這個UI容器
        currentBuilding = selectedBuilding;
        currentBuilding.OnQueueUpdated += HandleQueueUpdated; //BuildingUnit類別中會自動在Queue更新時發送事件

        //依據currentBuildingQueue的Queue來產生對應的按鈕
        SetupCancelButtons();

        //開啟更新進度條的Coroutine
        currentBuildCoroutine = StartCoroutine(UpdateBuildProgress()); //開啟當前正在build的單位程序讀條
    }

    private void SetupCancelButtons()
    {
        //疑問: 1.for的int i = 0 幹嘛寫在外面?
        //假設queue有3個待生產的單位，然後我們共有5個按鈕
        //因為是共用的i，所以跑完第一個for之後，第二個for就是剩下沒用到的案紐，可以取消
        int i = 0;
        for (; i < currentBuilding.QueueSize; i++)
        {
            //疑問: 2. 會合要用一個local的 int index = i ?
            int index = i; //catch起當前的index才不會被洗到最後一個index
            cancleUnitBuildButtons[i].EnableFor(currentBuilding.Queue[i], () => currentBuilding.CancelBuildingUnit(index));
        }
        for (; i < cancleUnitBuildButtons.Length; i++)
        {
            cancleUnitBuildButtons[i].Disable(); //把剩下的按鈕取消
        }
    }

    private void HandleQueueUpdated(UnitSO[] unitsInQueue)
    {
        if(unitsInQueue.Length == 0 ) progressBar.SetProgress(0);
        if(unitsInQueue.Length == 1 && currentBuildCoroutine == null) 
        {
            currentBuildCoroutine = StartCoroutine(UpdateBuildProgress());
        }
        SetupCancelButtons(); //更新按鈕
    }

    private IEnumerator UpdateBuildProgress()
    {
        while(currentBuilding != null && currentBuilding.QueueSize > 0)
        {
            float startTime = currentBuilding.CurrentBuildStartTime;
            float endTime = startTime + currentBuilding.CurrentBuildingUnitSO.BuildTime;
            float progress = Mathf.Clamp01((Time.time - startTime) / ( endTime - startTime ));
            // Debug.Log($"{startTime} / {endTime} -> {progress} 當前時間:{Time.time}");
            progressBar.SetProgress(progress);
            yield return null;
        }

        currentBuildCoroutine = null; //完成後記得重置coroutine
    }
}