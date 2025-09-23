namespace WeakEvents;

public partial class Subject(int id)
{
    public int Id { get; } = id;

//    [WeakEvent]
    public partial event EventHandler<SubjectEventArgs> SomeEvent;

}
