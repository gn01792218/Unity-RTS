using UnityEngine;

// 這是一般的C#類別，沒有繼承Unity的MonoBehaviour或其他類別
//所以要將屬性暴露給編輯器，必須要加上[System.Serializable]屬性
[System.Serializable]
public class CamaraConfig {
    [field: SerializeField]public bool EnableEdgePan {get; private set;} = true; // 是否開啟邊緣平移
    [field: SerializeField]public float EdgePanSpeed{get; private set;} = 20f; // 邊緣平移的速度
    [field: SerializeField]public float EdgePanDistance{get; private set;} = 50f; // 邊緣平移的距離


    [field: SerializeField]public float CamaraMoveSpeed{get; private set;} = 20f; // 相機移動的速度
    

    [field: SerializeField]public float ZoomSpeed{get; private set;} = 1f; // 縮放速度
    [field: SerializeField]public float MinZoomDistance{get; private set;} = 7.5f; // 最小縮放距離
    [field: SerializeField]public float MaxZoomDistance{get; private set;} = 15f; // 最大縮放距離
}
