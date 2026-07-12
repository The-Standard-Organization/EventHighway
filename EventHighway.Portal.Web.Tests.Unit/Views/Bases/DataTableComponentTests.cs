// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Views.Bases;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Views.Bases
{
    public partial class DataTableComponentTests : BunitContext
    {
        public sealed record Sample(string Name, int Value);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static List<Sample> CreateSamples(int count) =>
            Enumerable.Range(0, count)
                .Select(index => new Sample(Name: GetRandomString(), Value: index))
                .ToList();

        private static List<DataTableColumn<Sample>> CreateColumns() =>
            new List<DataTableColumn<Sample>>
            {
                new DataTableColumn<Sample> { Title = "Name", Value = sample => sample.Name },
                new DataTableColumn<Sample> { Title = "Value", Value = sample => sample.Value },
            };
    }
}
