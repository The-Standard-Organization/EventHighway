// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Services.Foundations.VolatilePaths;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.VolatilePaths
{
    public partial class VolatilePathServiceTests
    {
        private readonly Mock<IJsonBroker> jsonBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IVolatilePathService volatilePathService;

        public VolatilePathServiceTests()
        {
            this.jsonBrokerMock = new Mock<IJsonBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.volatilePathService = new VolatilePathService(
                jsonBroker: this.jsonBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();
    }
}
