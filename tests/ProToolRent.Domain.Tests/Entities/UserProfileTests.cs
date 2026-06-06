using System.ComponentModel;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;

namespace ProToolRent.Domain.tests;

public class UserProfileTests
{
    public UserProfile CreateTestProfile()
    {
        return new UserProfile(
            firstName: "name",
            lastName: "last", 
            city: "city", 
            organization: "org", 
            phone: "123"
            );
    }
    [Fact]
    public void Constructor_WhenDataIsValid_CreatesUserProfileObject()
    {
        var userProfile = CreateTestProfile();

        Assert.Equal("name", userProfile.FirstName);
        Assert.Equal("last", userProfile.LastName);
        Assert.Equal("city", userProfile.City);
        Assert.Equal("org", userProfile.Organization);
        Assert.Equal("123", userProfile.Phone);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenFirstNameIsInvalid_ThrowsException(string? invalidFirstName)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new UserProfile(
                firstName: invalidFirstName!,
                lastName: "last",
                city: "city",
                organization: "org",
                phone: "123"
            ));

        Assert.Equal("firstName", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenLastNameIsInvalid_ThrowsException(string? invalidLastName)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new UserProfile(
                firstName: "name",
                lastName: invalidLastName!,
                city: "city",
                organization: "org",
                phone: "123"
            ));

        Assert.Equal("lastName", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenCityIsInvalid_ThrowsException(string? invalidCity)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new UserProfile(
                firstName: "name",
                lastName: "last",
                city: invalidCity!,
                organization: "org",
                phone: "123"
            ));

        Assert.Equal("city", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenOrganizationIsInvalid_ThrowsException(string? invalidOrganization)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new UserProfile(
                firstName: "name",
                lastName: "last",
                city: "city",
                organization: invalidOrganization!,
                phone: "123"
            ));

        Assert.Equal("organization", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenPhoneIsInvalid_ThrowsException(string? invalidPhone)
    {
        var ex = Assert.Throws<ArgumentException>(() => 
            new UserProfile(
                firstName: "name",
                lastName: "last",
                city: "city",
                organization: "org",
                phone: invalidPhone!
            ));

        Assert.Equal("phone", ex.ParamName);
    }

    [Fact]
    public void CreateEmpty_ReturnEmptyProfile()
    {
        var emptyProfile = UserProfile.CreateEmpty();

        Assert.Equal("", emptyProfile.FirstName);
        Assert.Equal("", emptyProfile.LastName);
        Assert.Equal("", emptyProfile.City);
        Assert.Equal("", emptyProfile.Organization);
        Assert.Equal("", emptyProfile.Phone);
        Assert.Null(emptyProfile.User);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateProfile_WhenFirstNameIsInvalid_ThrowsException(string? invalidFirstName)
    {
        var profile = CreateTestProfile();
    
        var ex = Assert.Throws<ArgumentException>(() => 
            profile.UpdateProfile(
                firstName: invalidFirstName!,
                lastName: "last",
                city: "city",
                organization: "org",
                phone: "123"
            ));
    
        Assert.Equal("firstName", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateProfile_WhenLastNameIsInvalid_ThrowsException(string? invalidLastName)
    {
        var profile = CreateTestProfile();
    
        var ex = Assert.Throws<ArgumentException>(() => 
            profile.UpdateProfile(
                firstName: "first",
                lastName: invalidLastName!,
                city: "city",
                organization: "org",
                phone: "123"
            ));
    
        Assert.Equal("lastName", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateProfile_WhenCityIsInvalid_ThrowsException(string? invalidCity)
    {
        var profile = CreateTestProfile();
    
        var ex = Assert.Throws<ArgumentException>(() => 
            profile.UpdateProfile(
                firstName: "first",
                lastName: "last",
                city: invalidCity!,
                organization: "org",
                phone: "123"
            ));
    
        Assert.Equal("city", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateProfile_WhenOrganizationIsInvalid_ThrowsException(string? invalidOrganization)
    {
        var profile = CreateTestProfile();
    
        var ex = Assert.Throws<ArgumentException>(() => 
            profile.UpdateProfile(
                firstName: "first",
                lastName: "last",
                city: "city",
                organization: invalidOrganization!,
                phone: "123"
            ));
    
        Assert.Equal("organization", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateProfile_WhenPhoneIsInvalid_ThrowsException(string? invalidPhone)
    {
        var profile = CreateTestProfile();
    
        var ex = Assert.Throws<ArgumentException>(() => 
            profile.UpdateProfile(
                firstName: "first",
                lastName: "last",
                city: "city",
                organization: "org",
                phone: invalidPhone!
            ));
    
        Assert.Equal("phone", ex.ParamName);
    }

    [Fact]
    public void UpdateProfile_WhenDataIsValid_UpdatesAllFields()
    {
        var profile = new UserProfile(
            firstName: "old",
            lastName: "old",
            city: "old",
            organization: "old",
            phone: "111"
        );
        
        profile.UpdateProfile(
            firstName: "newfirst",
            lastName: "newlast",
            city: "newcity",
            organization: "neworg",
            phone: "999"
        );
        
        Assert.Equal("newfirst", profile.FirstName);
        Assert.Equal("newlast", profile.LastName);
        Assert.Equal("newcity", profile.City);
        Assert.Equal("neworg", profile.Organization);
        Assert.Equal("999", profile.Phone);
    }

    [Fact]
    public void UpdateProfile_WithSameData_ShouldNotChangeAnyValues()
    {
        var profile = new UserProfile("Petr", "Ivanov", "Moscow", "Company", "123");
        
        var expectedFirstName = profile.FirstName;
        var expectedLastName = profile.LastName;
        var expectedCity = profile.City;
        var expectedOrganization = profile.Organization;
        var expectedPhone = profile.Phone;
        
        profile.UpdateProfile("Petr", "Ivanov", "Moscow", "Company", "123");
        
        Assert.Equal(expectedFirstName, profile.FirstName);
        Assert.Equal(expectedLastName, profile.LastName);
        Assert.Equal(expectedCity, profile.City);
        Assert.Equal(expectedOrganization, profile.Organization);
        Assert.Equal(expectedPhone, profile.Phone);
    }

    [Fact]
    public void SetUser_WhenDataIsValid_SetsUserInUserProfile()
    {
        var user = new User("email", "pass", UserRole.Admin);
        var userProfile = new UserProfile(
            firstName: "name",
            lastName: "lastname",
            city: "city",
            organization: "organization",
            phone: "123456789"
        );

        userProfile.SetUser(user);

        Assert.NotNull(userProfile.User);
        Assert.Equal(user, userProfile.User);
    }
}
