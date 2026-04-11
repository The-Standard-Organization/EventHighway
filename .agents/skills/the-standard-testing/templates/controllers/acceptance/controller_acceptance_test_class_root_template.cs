// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Root test class for Controller Acceptance Tests
// File: [Entity]ApiTests.cs
// Replace [Entity] / [Entities] / [Namespace] with actual values.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using [Namespace].Tests.Acceptance.Brokers;
using [Namespace].Tests.Acceptance.Models.[Entities];
using Tynamix.ObjectFiller;

namespace [Namespace].Tests.Acceptance.Apis.[Entities]
{
    // test-073: Acceptance-test every endpoint
    [Collection(nameof(ApiTestCollection))]
    public partial class [Entity]ApiTests
    {
        private readonly ApiBroker apiBroker;

        public [Entity]ApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        // test-035: Randomized data helper using ObjectFiller
        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static [Entity] Update[Entity]WithRandomValues([Entity] input[Entity])
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updated[Entity] = CreateRandom[Entity]();
            updated[Entity].Id = input[Entity].Id;
            updated[Entity].EntraId = input[Entity].EntraId;
            updated[Entity].CreatedDate = input[Entity].CreatedDate;
            updated[Entity].CreatedBy = input[Entity].CreatedBy;
            updated[Entity].UpdatedDate = now;

            return updated[Entity];
        }

        // test-074: Clean up all test data after acceptance tests
        private async ValueTask<[Entity]> PostRandom[Entity]Async()
        {
            [Entity] random[Entity] = CreateRandom[Entity]();
            [Entity] created[Entity] = await this.apiBroker.Post[Entity]Async(random[Entity]);

            return created[Entity];
        }

        private async ValueTask<List<[Entity]>> PostRandom[Entities]Async()
        {
            int randomNumber = GetRandomNumber();
            var random[Entities] = new List<[Entity]>();

            for (int i = 0; i < randomNumber; i++)
            {
                random[Entities].Add(await PostRandom[Entity]Async());
            }

            return random[Entities];
        }

        private static [Entity] CreateRandom[Entity]() =>
            CreateRandom[Entity]Filler().Create();

        private static Filler<[Entity]> CreateRandom[Entity]Filler()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<[Entity]>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now);

            return filler;
        }
    }
}
