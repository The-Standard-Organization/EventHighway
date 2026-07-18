// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using EventHighway.Core.Clients.Events.V2;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Coordinations.Events.V2;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        private readonly Mock<IEventV2CoordinationService> eventV2CoordinationServiceMock;
        private int coordinationServiceResolutionCount;
        private readonly IEventV2Client eventV2Client;

        public EventV2ClientTests()
        {
            this.eventV2CoordinationServiceMock =
                new Mock<IEventV2CoordinationService>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped(_ =>
            {
                this.coordinationServiceResolutionCount++;

                return this.eventV2CoordinationServiceMock.Object;
            });

            this.eventV2Client =
                new EventV2Client(
                    serviceProvider: serviceCollection.BuildServiceProvider());
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new EventV2CoordinationValidationException(
                    someMessage,
                    someInnerException),

                new EventV2CoordinationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static IQueryable<EventV2> CreateRandomEventV2s() =>
            CreateEventV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static EventV2 CreateRandomEventV2() =>
            CreateEventV2Filler().Create();

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(eventV2 =>
                    eventV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.ListenerEventV2s).IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.EventParticipantV2).IgnoreIt();

            return filler;
        }
    }
}
