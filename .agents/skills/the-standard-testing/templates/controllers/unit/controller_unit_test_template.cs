// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Controller Unit Tests — Logic and Security
// Replace [Entity] / [Entities] / [Namespace] with actual values.
// Demonstrates: Logic (success code mapping) and Security tests.

// ---------------------------------------------------------------
// File: [Entities]ControllerTests.Logic.Get.cs
// test-070: Unit-test every success code mapping
// ---------------------------------------------------------------

namespace [Namespace].Tests.Unit.Controllers.[Entities]
{
    public partial class [Entities]ControllerTests
    {
        // test-070: Unit-test every success code mapping (200 for GET/PUT/DELETE, 201 for POST)
        // test-100: GWT pattern with inline comments
        // test-030: Always verify exact service calls
        [Fact]
        public async Task ShouldReturnRecordOnGetByIdsAsync()
        {
            // given
            [Entity] random[Entity] = CreateRandom[Entity]();
            Guid inputId = random[Entity].Id;
            [Entity] storage[Entity] = random[Entity];
            [Entity] expected[Entity] = storage[Entity].DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expected[Entity]);

            var expectedActionResult =
                new ActionResult<[Entity]>(expectedObjectResult);

            [entity]ServiceMock
                .Setup(service => service.Retrieve[Entity]ByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storage[Entity]);

            // when
            ActionResult<[Entity]> actualActionResult = await [entities]Controller.Get[Entity]ByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            // test-030: Verify exact service calls (Times.Once)
            [entity]ServiceMock
                .Verify(service => service.Retrieve[Entity]ByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            // test-031: Always end with VerifyNoOtherCalls()
            [entity]ServiceMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: [Entities]ControllerTests.Security.Get.cs
// test-072: Unit-test authorization/authentication failure mappings
// ---------------------------------------------------------------

namespace [Namespace].Tests.Unit.Controllers.[Entities]
{
    public partial class [Entities]ControllerTests
    {
        // test-072: Unit-test auth failure mappings
        // test-100: GWT pattern with inline comments
        [Fact]
        public void GetShouldHaveRoleAttributeWithRoles()
        {
            // Given
            var controllerType = typeof([Entities]Controller);
            var methodInfo = controllerType.GetMethod("Get[Entity]ByIdAsync");
            Type attributeType = typeof(AuthorizeAttribute);
            string attributeProperty = "Roles";

            List<string> expectedAttributeValues = new List<string>
            {
                "Administrators,[Entity]Write"
            };

            // When
            var methodAttribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var controllerAttribute = controllerType
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var attribute = methodAttribute ?? controllerAttribute;

            // Then
            attribute.Should().NotBeNull();

            var actualAttributeValue = attributeType
                .GetProperty(attributeProperty)?
                .GetValue(attribute) as string ?? string.Empty;

            var actualAttributeValues = actualAttributeValue?
                .Split(',')
                .Select(role => role.Trim())
                .Where(role => !string.IsNullOrEmpty(role))
                .ToList();

            // test-102: Use FluentAssertions for readable assertions
            actualAttributeValues.Should().BeEquivalentTo(expectedAttributeValues);
        }

        // test-072: Unit-test auth failure mappings
        [Fact]
        public void GetShouldNotHaveInvisibleApiAttribute()
        {
            // Given
            var controllerType = typeof([Entities]Controller);
            var methodInfo = controllerType.GetMethod("Get[Entity]ByIdAsync");
            Type attributeType = typeof(InvisibleApiAttribute);

            // When
            var methodAttribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var controllerAttribute = controllerType
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var attribute = methodAttribute ?? controllerAttribute;

            // Then
            // test-102: Use FluentAssertions for readable assertions
            attribute.Should().BeNull();
        }
    }
}
