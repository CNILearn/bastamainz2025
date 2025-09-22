namespace WeakEvents;

public partial class Subject
{
    // this can be generated from a source-generator

    private readonly WeakEvent<SubjectEventArgs> _weakEvent = new();
    public partial event EventHandler<SubjectEventArgs> SomeEvent
    {
        add
        {
            _weakEvent.AddHandler(value);
        }
        remove
        {
            _weakEvent.RemoveHandler(value);
        }
    }

    public void RaiseEvent()
    {
        _weakEvent.RaiseEvent(this, new SubjectEventArgs(Id));
    }
}
