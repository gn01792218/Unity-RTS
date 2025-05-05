using UnityEngine;
[CreateAssetMenu(fileName = "MoveCommand", menuName = "Commands/Actions/MoveCommand", order = 100)]
public class MoveCommand : Command
{
    [SerializeField] private float circleRadiaOffset = 2.5f; //這一圈的半徑偏移量, 用2會和內圈黏在一起，看起來有點噁心
    private int unitsOnRing = 0; //計算當前有多少單位在這一圈上
    private int maxUnitsOnRing = 1; //這一圈最多可以放多少單位
    private float circleRadius = 0; //這一圈的半徑
    private float unitOnRingsAngle = 0; //這一圈擺放單位的平均角度
    public override bool CanHandle(CommandContext context)
    {
        return context.Unit is Unit; //檢查是否為Unit，因為這個類別包含IMoveable和AgentRadius
    }

    public override void Handle(CommandContext context)
    {
        Unit unit = context.Unit as Unit; //將單位轉換為Unit類別，為了獲取AgentRadius

        if (context.UnitIndex == 0) //當index為0的時候恢復初始值
        {
            unitsOnRing = 0; //計算當前有多少單位在這一圈上
            maxUnitsOnRing = 1; //這一圈最多可以放多少單位
            circleRadius = 0; //這一圈的半徑
        }
        //移動點位的計算
        //一開始就是hit.point，因為第一圈半徑是0
        //然後還要加上單位的半徑、及圈上的座標點(透過sin和cos計算出來的)
        //才不會重疊
        Vector3 targetPosition = new(
            context.Hit.point.x + circleRadius * Mathf.Cos(unitOnRingsAngle * unitsOnRing), //往後每一圈的半徑都要+上去
            context.Hit.point.y,  //這是高度，不需要+
            context.Hit.point.z + circleRadius * Mathf.Sin(unitOnRingsAngle * unitsOnRing) //往後每一圈的半徑都要+上去
        );

        unit.Move(targetPosition); // 移動到擊中點
        unitsOnRing++;

        //當到達一圈的上限時，開始準備下一圈的初始化作業
        if (unitsOnRing >= maxUnitsOnRing) // 如果已經達到最大單位數量
        {
            // 1.重置當前單位數量
            unitsOnRing = 0;
            //2.根據該單位的半徑，計算下一圈的半徑要多大(至少要*2以上唷)
            circleRadius += unit.AgentRadius * circleRadiaOffset;  //*2就會剛好和內圈黏在一起，因此可以考慮*3
                                                                   //3.計算下一圈可以放多少單位
            maxUnitsOnRing = Mathf.FloorToInt(2 * Mathf.PI * circleRadius / (unit.AgentRadius * 2));
            //4.計算這一圈單位之間擺放的平均角度
            unitOnRingsAngle = 2 * Mathf.PI / maxUnitsOnRing; //用360度/這一圈可以放的總單位
        }
        //完成後，切記將Unit的Agent的StopDistance設為0，這樣才會排出完美的圓圈唷!
        //否則會有些許誤差~就停下來，導致圓圈不美麗
        //當然如果想要有比較隨興的排列，那麼也可以不要歸0
    }
}