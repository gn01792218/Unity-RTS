//使用struct可以增進效能
using Unity.VisualScripting;
using UnityEngine;

public struct CommandContext
{
   public CommandableUnit Unit { get; private set; } //單位
   public RaycastHit Hit { get; private set; } //射線擊中點 
   public int UnitIndex { get; private set; } //單位的索引
   public MouseButton MouseButton { get; private set; } //按下了滑鼠的什麼按鍵
   public CommandContext(
      CommandableUnit unit,
      RaycastHit hit,
      int unitIndex = 0,
      MouseButton mouseButton = MouseButton.Left
      )
   {
      Unit = unit;
      Hit = hit;
      UnitIndex = unitIndex;
      MouseButton = mouseButton;
   }
}