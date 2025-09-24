namespace UnsafeAccessorGenerator.Sample;

// Partial JsonContext class - the other part will be generated
public partial class JsonContext
{
    public partial IEnumerable<Book> GetBooks(string jsonFile);
    public partial IEnumerable<Person> GetPeople(string jsonFile);
    public partial IEnumerable<BookWithPrivateSetters> GetBooksWithPrivateSetters(string jsonFile);
    public partial IEnumerable<BookRecord> GetBookRecords(string jsonFile);
}
