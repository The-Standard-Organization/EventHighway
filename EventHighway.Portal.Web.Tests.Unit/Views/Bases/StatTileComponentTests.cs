// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class StatTileComponentTests : BunitContext
    {
        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        public static TheoryData<StatTileVariant, string> VariantCssClasses() =>
            new TheoryData<StatTileVariant, string>
            {
                { StatTileVariant.Green, "rag-green" },
                { StatTileVariant.Amber, "rag-amber" },
                { StatTileVariant.Red, "rag-red" },
                { StatTileVariant.Na, "rag-na" },
            };
    }
}
