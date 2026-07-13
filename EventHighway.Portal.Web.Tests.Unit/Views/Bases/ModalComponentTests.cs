// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class ModalComponentTests : BunitContext
    {
        private static string GetRandomString() =>
            new MnemonicString().GetValue();
    }
}
