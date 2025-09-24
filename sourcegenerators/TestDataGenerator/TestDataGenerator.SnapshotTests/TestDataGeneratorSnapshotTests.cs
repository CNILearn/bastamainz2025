using System.Threading.Tasks;
using TestDataGenerator.SnapshotTests;
using Xunit;

namespace TestDataGenerator.SnapshotTests;

public class TestDataGeneratorSnapshotTests
{
    [Fact]
    public Task GeneratesFactoryForSimpleClass()
    {
        var source = """
            using TestDataGenerator.Attributes;

            namespace TestNamespace;

            [GenerateTestData]
            public class User
            {
                public string Name { get; set; }
                public int Age { get; set; }
                public bool IsActive { get; set; }
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task GeneratesFactoryForClassWithCustomValues()
    {
        var source = """
            using TestDataGenerator.Attributes;

            namespace TestNamespace;

            [GenerateTestData(StringValue = "CustomName", IntRangeMin = 18, IntRangeMax = 65)]
            public class CustomUser
            {
                public string Name { get; set; }
                public int Age { get; set; }
                public System.DateTime RegisteredOn { get; set; }
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task GeneratesFactoryForClassWithVariousTypes()
    {
        var source = """
            using TestDataGenerator.Attributes;
            using System;

            namespace TestNamespace;

            public enum UserRole
            {
                Guest,
                User,
                Admin
            }

            [GenerateTestData]
            public class Product
            {
                public string Name { get; set; }
                public decimal Price { get; set; }
                public int StockQuantity { get; set; }
                public Guid Id { get; set; }
                public bool IsAvailable { get; set; }
                public DateTime? LastUpdated { get; set; }
                public UserRole Role { get; set; }
                public double Weight { get; set; }
                public float Rating { get; set; }
                public byte CategoryId { get; set; }
                public long SerialNumber { get; set; }
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task GeneratesFactoryForMultipleClassesInSameNamespace()
    {
        var source = """
            using TestDataGenerator.Attributes;

            namespace TestNamespace;

            [GenerateTestData]
            public class User
            {
                public string Name { get; set; }
                public int Age { get; set; }
            }

            [GenerateTestData]
            public class Product
            {
                public string Title { get; set; }
                public decimal Price { get; set; }
            }
            """;

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task GeneratesFactoryForClassWithNullableProperties()
    {
        var source = """
            using TestDataGenerator.Attributes;
            using System;

            namespace TestNamespace;

            [GenerateTestData]
            public class NullableEntity
            {
                public string Name { get; set; }
                public int? OptionalAge { get; set; }
                public DateTime? OptionalDate { get; set; }
                public bool? OptionalFlag { get; set; }
                public decimal? OptionalPrice { get; set; }
            }
            """;

        return TestHelper.Verify(source);
    }
}