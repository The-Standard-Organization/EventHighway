// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Controller Acceptance Tests
// Replace [Entity] / [Entities] / [Namespace] with actual values.
// Demonstrates: Acceptance test for controller endpoints.

// ---------------------------------------------------------------
// File: [Entity]ApiTests.GetAll.cs
// test-073: Acceptance-test every endpoint
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using [Namespace].Tests.Acceptance.Models.[Entities];

namespace [Namespace].Tests.Acceptance.Apis.[Entities]
{
    public partial class [Entity]ApiTests
    {
        // test-073: Acceptance-test every endpoint
        // test-100: GWT pattern with inline comments
        [Fact]
        public async Task ShouldGetAll[Entities]Async()
        {
            // given
            List<[Entity]> random[Entities] = await PostRandom[Entities]Async();
            List<[Entity]> expected[Entities] = random[Entities];

            // when
            List<[Entity]> actual[Entities] = await this.apiBroker.GetAll[Entities]Async();

            // then
            foreach ([Entity] expected[Entity] in expected[Entities])
            {
                [Entity] actual[Entity] = 
                    actual[Entities].Single(approval => approval.Id == expected[Entity].Id);

                // test-102: Use FluentAssertions for readable assertions
                actual[Entity].Should().BeEquivalentTo(expected[Entity], options => options
                    .Excluding(property => property.CreatedBy)
                    .Excluding(property => property.CreatedDate)
                    .Excluding(property => property.UpdatedBy)
                    .Excluding(property => property.UpdatedDate));

                // test-074: Clean up all test data after acceptance tests
                await this.apiBroker.Delete[Entity]ByIdAsync(actual[Entity].Id);
            }
        }
    }
}
