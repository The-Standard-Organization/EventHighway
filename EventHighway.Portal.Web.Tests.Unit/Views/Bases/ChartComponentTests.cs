// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class ChartComponentTests : BunitContext
    {
        public ChartComponentTests()
        {
            this.JSInterop.Mode = JSRuntimeMode.Loose;
        }
    }
}
