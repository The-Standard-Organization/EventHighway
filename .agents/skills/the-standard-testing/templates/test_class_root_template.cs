// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Root test class for a Foundation Service
// File: [Entity]ServiceTests.cs
// Replace [Entity] / [Entities] / [Namespace] with actual values.

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace [Namespace].Tests.Unit.Services.Foundations.[Entities]
{
    // test-090, test-091: Root file — setup, mocks, helpers
    public partial class [Entity]ServiceTests
    {
        // test-101: Mock all dependencies with Moq
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly I[Entity]Service [entity]Service;

        public [Entity]ServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.[entity]Service = new [Entity]Service(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        // test-035: Randomized data helper using ObjectFiller
        private static [Entity] CreateRandom[Entity]() =>
            Create[Entity]Filler(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        // test-036: Exception equality expression helper
        private static Expression<Func<Exception, bool>> SameExceptionAs(Exception expectedException)
        {
            return actualException =>
                actualException.Message == expectedException.Message
                && actualException.InnerException.Message == expectedException.InnerException.Message;
        }

        private static Filler<[Entity]> Create[Entity]Filler(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<[Entity]>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset);

            return filler;
        }
    }
}
