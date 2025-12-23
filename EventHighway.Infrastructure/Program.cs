// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Infrastructure.Services;

namespace EventHighway.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scriptGenerationService = new ScriptGenerationService();

            scriptGenerationService.GenerateBuildScript(
                branchName: "main",
                projectName: "EventHighway.Core",
                dotNetVersion: "10.0.100");

            scriptGenerationService.GeneratePrLintScript(branchName: "main");
        }
    }
}
