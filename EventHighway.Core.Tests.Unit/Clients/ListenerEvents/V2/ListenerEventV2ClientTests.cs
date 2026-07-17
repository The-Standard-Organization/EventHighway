// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using EventHighway.Core.Clients.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.RetryingListenerEvents.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.RetryingListenerEvents.V2;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        private readonly Mock<IListenerEventV2OrchestrationService> listenerEventV2OrchestrationServiceMock;

        private readonly Mock<IRetryingListenerEventV2OrchestrationService>
            retryingListenerEventV2OrchestrationServiceMock;

        private int orchestrationServiceResolutionCount;
        private readonly IListenerEventV2Client listenerEventV2Client;

        public ListenerEventV2ClientTests()
        {
            this.listenerEventV2OrchestrationServiceMock =
                new Mock<IListenerEventV2OrchestrationService>();

            this.retryingListenerEventV2OrchestrationServiceMock =
                new Mock<IRetryingListenerEventV2OrchestrationService>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped(_ =>
            {
                this.orchestrationServiceResolutionCount++;

                return this.listenerEventV2OrchestrationServiceMock.Object;
            });

            serviceCollection.AddScoped(_ =>
                this.retryingListenerEventV2OrchestrationServiceMock.Object);

            this.listenerEventV2Client =
                new ListenerEventV2Client(
                    serviceProvider: serviceCollection.BuildServiceProvider());
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new ListenerEventV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> RetryValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new RetryingListenerEventV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new RetryingListenerEventV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static ListenerEventV2 CreateRandomListenerEventV2() =>
            CreateListenerEventV2Filler().Create();

        private static IQueryable<ListenerEventV2> CreateRandomListenerEventV2s() =>
            CreateListenerEventV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<ListenerEventV2> CreateListenerEventV2Filler()
        {
            var filler = new Filler<ListenerEventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())

                .OnType<EventParticipantV2>().IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventV2)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventListenerV2)
                    .IgnoreIt();

            return filler;
        }
    }
}
