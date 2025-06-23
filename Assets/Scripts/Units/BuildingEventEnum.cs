using System;
using Unity.Behavior;

[BlackboardEnum]
public enum BuildingEventEnum
{
    ArrivedAt, //到達建築的地基位置
    Begin, //開始蓋房子
    Cancel, //取消該建築(按下取消指令，可以拿回$$)
    Abort, //因任何緣故，蓋到一半無法繼續蓋時
    Completed, //完成
}
