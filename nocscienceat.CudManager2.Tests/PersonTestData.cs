namespace nocscienceat.CudManager2.Tests;

public record Person(int Id, string Surname, string GivenName);

public static class PersonTestData
{
    public static List<Person> SourcePersons = new()
    {
        new Person(19, "Hoffmann", "Sarah"),        // To Create (only in source)
        new Person(3, "Schmidt", "Max"),            // In Sync
        new Person(8, "Fischer", "Thomas"),         // To Update (exist in both, but data differs) - Surname changed
        new Person(21, "Weber", "Lisa"),            // In Sync
        new Person(5, "Bauer", "Michael"),          // To Update (exist in both, but data differs) - Both changed
        new Person(23, "Klein", "Stefan"),          // To Create (only in source)
        new Person(14, "Wagner", "Julia"),          // To Update (exist in both, but data differs) - GivenName changed
        new Person(17, "Müller", "Anna"),           // In Sync
        new Person(7, "Koch", "Daniel"),            // To Update (exist in both, but data differs) - Surname and GivenName changed
        new Person(12, "Richter", "Laura"),         // To Update (exist in both, but data differs) - Surname and GivenName changed
    };

    public static List<Person> DestinationPersons = new()
    {
        new Person(17, "Müller", "Anna"),           // In Sync
        new Person(8, "Meier", "Thomas"),           // To Update (exist in both, but data differs) - Surname differs
        new Person(25, "Neumann", "Christian"),     // To Delete (only in destination)
        new Person(3, "Schmidt", "Max"),            // In Sync
        new Person(12, "Schwarz", "Katrin"),        // To Update (exist in both, but data differs) - Surname and GivenName differ
        new Person(21, "Weber", "Lisa"),            // In Sync
        new Person(7, "Braun", "Sabine"),           // To Update (exist in both, but data differs) - Surname and GivenName differ
        new Person(5, "Huber", "Markus"),           // To Update (exist in both, but data differs) - Both differ
        new Person(14, "Wagner", "Juliane"),        // To Update (exist in both, but data differs) - GivenName differs
        new Person(9, "Schneider", "Peter")         // To Delete (only in destination)
    };

    public static List<Person> AlternativeSourcePersons = new()
    {
        new Person(19, "Hoffmann", "Sarah"),        // To Create (only in source)
        new Person(3, "Schmidt", "Max"),            // In Sync
        new Person(8, "Fischer", "Thomas"),         // To Update (exist in both, but data differs) - Surname changed
        new Person(21, "Weber", "Lisa"),            // In Sync
        new Person(5, "Bauer", "Michael"),          // To Update (exist in both, but data differs) - Both changed
        new Person(23, "Klein", "Stefan"),          // To Create (only in source)
        new Person(14, "Wagner", "Julia"),          // To Update (exist in both, but data differs) - GivenName changed
        new Person(17, "Müller", "Anna"),           // In Sync
        new Person(7, "Koch", "Daniel"),            // To Update (exist in both, but data differs) - Surname and GivenName changed
        new Person(12, "Richter", "Laura"),         // To Update (exist in both, but data differs) - Surname and GivenName changed
        new Person(5, "Dupont", "Marie"),           // Duplicate ID (5) with different data
    };

    public static List<Person> AlternativeDestinationPersons = new()
    {
        new Person(17, "Müller", "Anna"),           // In Sync
        new Person(8, "Meier", "Thomas"),           // To Update (exist in both, but data differs) - Surname differs
        new Person(25, "Neumann", "Christian"),     // To Delete (only in destination)
        new Person(3, "Schmidt", "Max"),            // In Sync
        new Person(12, "Schwarz", "Katrin"),        // To Update (exist in both, but data differs) - Surname and GivenName differ
        new Person(21, "Weber", "Lisa"),            // In Sync
        new Person(7, "Braun", "Sabine"),           // To Update (exist in both, but data differs) - Surname and GivenName differ
        new Person(5, "Huber", "Markus"),           // To Update (exist in both, but data differs) - Both differ
        new Person(14, "Wagner", "Juliane"),        // To Update (exist in both, but data differs) - GivenName differs
        new Person(9, "Schneider", "Peter"),        // To Delete (only in destination)
        new Person(5, "Dupont", "Marie"),           // Duplicate ID (5) with different data
    };
}
