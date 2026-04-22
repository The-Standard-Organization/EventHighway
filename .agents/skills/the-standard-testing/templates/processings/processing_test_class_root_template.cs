// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Processing Service Test Root Class
// Replace [Entity] / [Entities] / [Namespace] with actual values.
// This is the base class for all processing service tests.

using System;
using System.Linq;
using System.Linq.Expressions;
using [Namespace].Brokers.Loggings;
using [Namespace].Models.Foundations.[Entities];
using [Namespace].Models.Foundations.[Entities].Exceptions;
using [Namespace].Services.Foundations.[Entities];
using [Namespace].Services.Processings.[Entities];
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace [Namespace].Tests.Unit.Services.Processings.[Entities]
{
    public partial class [Entity]ProcessingServiceTests
    {
        private readonly Mock<I[Entity]Service> [entity]ServiceMock = new Mock<I[Entity]Service>();
        private readonly Mock<ILoggingBroker> loggingBrokerMock = new Mock<ILoggingBroker>();
        private readonly I[Entity]ProcessingService [entity]ProcessingService;

        public [Entity]ProcessingServiceTests()
        {
            [entity]ProcessingService = new [Entity]ProcessingService(
                [entity]Service: [entity]ServiceMock.Object,
                loggingBroker: loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            innerException.Data.Add(
                key: nameof([Entity].Id),
                values: "Id is required");

            return new TheoryData<Xeption>
            {
                new [Entity]ValidationException(
                    message: "[Entity] validation errors occurred, please try again.", innerException),

                new [Entity]DependencyValidationException(
                    message: "[Entity] dependency validation occurred, please try again.", innerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new [Entity]DependencyException(
                    message: "[Entity] validation errors occurred, please try again.", innerException),

                new [Entity]ServiceException(
                    message : "[Entity] service error occurred, please contact support.", innerException)
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomString(int length) =>
            new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static [Entity] CreateRandom[Entity]() =>
            Create[Entity]Filler(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static IQueryable<[Entity]> CreateRandom[Entities]()
        {
            return Create[Entity]Filler(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<[Entity]> Create[Entity]Filler(DateTimeOffset dateTimeOffset)
        {
            string user = GetRandomString(length: 255).ToString();
            var filler = new Filler<[Entity]>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset);
                // Add .OnProperty([entity] => [entity].NavigationProperty).IgnoreIt(); for navigation properties

            return filler;
        }
    }
}
