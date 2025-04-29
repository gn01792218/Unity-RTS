public struct SelectedEvent:IEvent
{
    public ISelectable SelectdObject {get; private set;}
    public SelectedEvent(ISelectable selectdObject)
    {
        SelectdObject = selectdObject;
    }
}