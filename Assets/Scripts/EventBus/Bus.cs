using UnityEngine;

public static class Bus<T> where T : IEvent
{
    public delegate void Event(T args);
    public static event Event OnEvent; //訂閱事件的方法
    public static void Publish(T args) =>OnEvent?.Invoke(args); //如果有訂閱者則執行
    public static void Unsubscribe(Event method)=> OnEvent -= method; //從事件中移除訂閱者
}