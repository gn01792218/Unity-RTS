public interface IUIElement
{
    //給不需要傳遞任何東西的UI，像是資源顯示
    void EnableFor();
    void Disable();
}
public interface IUIElement<T>  
{
    //主要是給container元件使用的,例如指令容器
    void EnableFor(T item); //要開啟我，得跟我說是哪種單位，我才能根據單位進行渲染
    void Disable();
}

public interface IUIElement<T1, T2>  
{
    //主要是給像是指令按鈕這類有call back function的元件使用
    void EnableFor(T1 item, T2 callback); //得告訴我是哪種類別，又需要的call back是什麼
    void Disable();
}