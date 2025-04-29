public struct UnselectedEvent:IEvent
{
    public ISelectable SelectdObject {get; private set;}
    public UnselectedEvent(ISelectable selectdObject)
    {
        SelectdObject = selectdObject;
    }
}