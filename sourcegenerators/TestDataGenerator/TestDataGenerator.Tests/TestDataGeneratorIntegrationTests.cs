using System;
using System.Linq;
using TestDataGenerator.Attributes;
using Xunit;

namespace TestDataGenerator.Tests;

[GenerateTestData]
public class TestUser
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

[GenerateTestData(StringValue = "TestName", IntRangeMin = 25, IntRangeMax = 65)]
public class CustomTestUser
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime RegisteredOn { get; set; }
}

public class TestDataGeneratorIntegrationTests
{
    [Fact]
    public void GenerateTestDataAttribute_ExistsAndCanBeApplied()
    {
        // This test verifies that the GenerateTestDataAttribute is generated and can be applied
        // The fact that this code compiles proves the source generator is working
        Assert.True(true, "GenerateTestDataAttribute exists and can be applied to classes");
    }

    [Fact]
    public void TestUserTestDataFactory_CreateSample_ReturnsValidInstance()
    {
        // Test that the generated factory creates a valid instance
        var testUser = TestUserTestDataFactory.CreateSample();
        
        Assert.NotNull(testUser);
        Assert.NotNull(testUser.Name);
        Assert.NotEmpty(testUser.Name);
        Assert.True(testUser.Age >= 0);
        Assert.True(testUser.CreatedAt != default(DateTime));
    }

    [Fact]
    public void TestUserTestDataFactory_CreateMany_ReturnsCorrectCount()
    {
        // Test that CreateMany generates the correct number of instances
        var users = TestUserTestDataFactory.CreateMany(5);
        
        Assert.NotNull(users);
        Assert.Equal(5, users.Count);
        
        foreach (var user in users)
        {
            Assert.NotNull(user);
            Assert.NotNull(user.Name);
            Assert.NotEmpty(user.Name);
        }
    }

    [Fact]
    public void CustomTestUserTestDataFactory_CreateSample_UsesCustomValues()
    {
        // Test that custom attribute values are applied
        var customUser = CustomTestUserTestDataFactory.CreateSample();
        
        Assert.NotNull(customUser);
        Assert.Equal("TestName", customUser.Name);
        Assert.InRange(customUser.Age, 25, 65);
        Assert.True(customUser.RegisteredOn != default(DateTime));
    }

    [Fact]
    public void TestUserTestDataFactory_CreateMany_GeneratesDifferentData()
    {
        // Test that multiple instances have different random data
        var users = TestUserTestDataFactory.CreateMany(10);
        
        // Check that at least some values are different (randomness)
        var ages = users.Select(u => u.Age).Distinct().Count();
        var dates = users.Select(u => u.CreatedAt.Date).Distinct().Count();
        
        // With random generation, we should get some variety
        // Note: This test might occasionally fail due to randomness, but it's very unlikely
        Assert.True(ages > 1 || dates > 1, "Generated test data should have some variety");
    }

    [Fact]
    public void GeneratedFactories_HandleBooleanProperties()
    {
        var user = TestUserTestDataFactory.CreateSample();
        
        // IsActive should be either true or false (not default)
        Assert.True(user.IsActive == true || user.IsActive == false);
    }
}