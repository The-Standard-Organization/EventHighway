// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using EventHighway.Core.Clients.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ClientTests
    {
        private readonly Mock<IListenerEventArchiveV2Service> listenerEventArchiveV2ServiceMock;
        private readonly IListenerEventArchiveV2Client listenerEventArchiveV2Client;

        public ListenerEventArchiveV2ClientTests()
        {
            this.listenerEventArchiveV2ServiceMock =
                new Mock<IListenerEventArchiveV2Service>();

            this.listenerEventArchiveV2Client =
                new ListenerEventArchiveV2Client(
                    listenerEventArchiveV2Service:
                        this.listenerEventArchiveV2ServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            return new TheoryData<Xeption>
            {
                new ListenerEventArchiveV2ValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventArchiveV2DependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static ListenerEventArchiveV2 CreateRandomListenerEventArchiveV2() =>
            CreateListenerEventArchiveV2Filler().Create();

        private static IQueryable<ListenerEventArchiveV2> CreateRandomListenerEventArchiveV2s() =>
            CreateListenerEventArchiveV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<ListenerEventArchiveV2> CreateListenerEventArchiveV2Filler()
        {
            var filler = new Filler<ListenerEventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())

                .OnProperty(listenerEventArchiveV2 => listenerEventArchiveV2.EventListenerV2)
                    .IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }
    }
}
