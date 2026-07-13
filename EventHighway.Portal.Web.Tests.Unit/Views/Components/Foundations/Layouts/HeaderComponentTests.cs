// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Components.Foundations.Layouts
{
    public partial class HeaderComponentTests : BunitContext
    {
        public HeaderComponentTests()
        {
            // The header hosts ColorModeSwitcher, which calls JS interop on render.
            JSInterop.Mode = JSRuntimeMode.Loose;
        }
    }
}
