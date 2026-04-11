// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Root test class for a Controller
// File: [Entities]ControllerTests.cs
// Replace [Entity] / [Entities] / [Namespace] with actual values.

using System;
using System.Linq;
using [Namespace].Models.Foundations.[Entities];
using [Namespace].Models.Foundations.[Entities].Exceptions;
using [Namespace].Services.Foundations.[Entities];
using [Namespace].Controllers;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace [Namespace].Tests.Unit.Controllers.[Entities]
{
    // test-090, test-091: Root file — setup, mocks, helpers
    public partial class [Entities]ControllerTests : RESTFulController
    {
        // test-101: Mock all dependencies with Moq
        private readonly Mock<I[Entity]Service> [entity]ServiceMock;
        private readonly [Entities]Controller [entities]Controller;

        public [Entities]ControllerTests()
        {
            [entity]ServiceMock = new Mock<I[Entity]Service>();
            [entities]Controller = new [Entities]Controller([entity]ServiceMock.Object);
        }

        // test-071: Unit-test every error mapping
        // Validation → 400, DependencyValidation → 400
        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new [Entity]ValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new [Entity]DependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        // test-071: Unit-test every error mapping
        // CriticalDependency → 500, Dependency → 500, Service → 500
        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new [Entity]DependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new [Entity]ServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        // test-035: Randomized data helper using ObjectFiller
        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static [Entity] CreateRandom[Entity]() =>
            Create[Entity]Filler().Create();

        private static IQueryable<[Entity]> CreateRandom[Entities]()
        {
            return Create[Entity]Filler()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<[Entity]> Create[Entity]Filler()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<[Entity]>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset);

            return filler;
        }
    }
}
