// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using EventHighway.Infrastructure.Models;

namespace EventHighway.Infrastructure.Services
{
    internal class ScriptGenerationService
    {
        private readonly ADotNetClient adotNetClient;

        public ScriptGenerationService() =>
            adotNetClient = new ADotNetClient();

        public void GenerateBuildScript(
            string branchName,
            string projectName,
            string dotNetVersion)
        {
            GitHubPipelineBuilder.CreateNewPipeline()
              .SetName(projectName)
              .OnPush(branchName)
              .OnPullRequest(branchName)

              .AddJob("build-windows", job => job
                  .WithName("Build (Windows)")
                  .RunsOn(BuildMachines.WindowsLatest)
                  .AddCheckoutStep("Check out")
                  .AddSetupDotNetStep(dotNetVersion)
                  .AddRestoreStep()
                  .AddBuildStep()
                  .AddTestStep())

              .AddJob("build-integration", job => job
                  .WithName("Build & Test (DB matrix)")
                  .RunsOn(BuildMachines.UbuntuLatest)
                  .WithFailFast(false)
                  .AddMatrix("provider", "sqlserver", "postgres")
                    .AddMatrixInclude(new()
                    {
                        ["provider"] = "sqlserver",
                        ["connection_string"] =
                            $"Server=localhost;Database=EventHighwayDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
                    })
                    .AddMatrixInclude(new()
                    {
                        ["provider"] = "postgres",
                        ["connection_string"] =
                            $"Host=localhost;Database=EventHighwayDb;Username=postgres;Password=postgres"
                    })
                    .AddService("sqlserver", new Models.Service
                    {
                        Image = "mcr.microsoft.com/mssql/server:2019-latest",
                        Environment = new()
                        {
                            ["ACCEPT_EULA"] = "Y",
                            ["SA_PASSWORD"] = "Your_password123!"
                        },
                        Ports = new() { "1433:1433" },
                        Options =
                        "--health-cmd \"/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Your_password123! -N -C -Q 'SELECT 1' || exit 1\" " +
                        "--health-interval 10s --health-timeout 5s --health-retries 10"
                    })
                    .AddService("postgres", new Models.Service
                    {
                        Image = "postgres:17",
                        Environment = new()
                        {
                            ["POSTGRES_DB"] = "EventHighwayDb",
                            ["POSTGRES_USER"] = "postgres",
                            ["POSTGRES_PASSWORD"] = "postgres"
                        },
                        Ports = new() { "5432:5432" },
                        Options = "--health-cmd pg_isready --health-interval 10s --health-timeout 5s --health-retries 5"
                    })
                  .AddEnvironmentVariables(new Dictionary<string, string>
                  {
                      ["PROVIDER"] =
                          "${{ matrix.provider }}",

                      ["CONNECTION_STRING"] =
                          "${{ matrix.connection_string }}"
                  })
                  .AddCheckoutStep()
                  .AddSetupDotNetStep("10.0.100")
                  .AddRestoreStep()
                  .AddBuildStep()
                  .AddGenericStep(
                      name: "Install EF Core Tools",
                      runCommand: "dotnet tool install --global dotnet-ef")
                  .AddGenericStep(
                      name: "Apply Migrations",
                      runCommand: "dotnet ef database update")
                  .AddTestStep())

              .AddJob("add_tag", job => job
                  .WithName("Tag and Release")
                  .RunsOn(BuildMachines.UbuntuLatest)
                  .DependsOn("build-windows", "build-integration")
                  .WithCondition(
                      "needs.build-windows.result == 'success' && " +
                      "needs.build-integration.result == 'success' && " +
                      "github.event.pull_request.merged && " +
                      "github.event.pull_request.base.ref == 'main' && " +
                      "startsWith(github.event.pull_request.title, 'RELEASES:') && " +
                      "contains(github.event.pull_request.labels.*.name, 'RELEASES')")
                  .AddActionStep(
                      name: "Checkout code",
                      uses: "actions/checkout@v5",
                      with: new Dictionary<string, string>
                      {
                          ["token"] = "${{ secrets.PAT_FOR_TAGGING }}"
                      })
                  .AddGenericStep(
                      name: "Configure Git",
                      runCommand:
                          "git config user.name \"GitHub Action\"\n" +
                          "git config user.email \"action@github.com\"")
                  .AddGenericStep(
                      id: "extract_version",
                      name: "Extract Version",
                      shell: "bash",
                      runCommand:
                          "sudo apt-get update\n" +
                          "sudo apt-get install -y xmlstarlet\n" +
                          "version_number=$(xmlstarlet sel -t -v \"//Version\" " +
                          "-n EventHighway.Core/EventHighway.Core.csproj)\n" +
                          "echo \"$version_number\"\n" +
                          "echo \"version_number<<EOF\" >> $GITHUB_OUTPUT\n" +
                          "echo \"$version_number\" >> $GITHUB_OUTPUT\n" +
                          "echo \"EOF\" >> $GITHUB_OUTPUT")
                  .AddGenericStep(
                      name: "Display Version",
                      runCommand: "echo \"Version number: ${{ steps.extract_version.outputs.version_number }}\"")
                  .AddGenericStep(
                      id: "extract_package_release_notes",
                      name: "Extract Package Release Notes",
                      shell: "bash",
                      runCommand:
                          "sudo apt-get update\n" +
                          "sudo apt-get install -y xmlstarlet\n" +
                          "package_release_notes=$(xmlstarlet sel -t -v \"//PackageReleaseNotes\" " +
                          "-n EventHighway.Core/EventHighway.Core.csproj)\n" +
                          "echo \"$package_release_notes\"\n" +
                          "echo \"package_release_notes<<EOF\" >> $GITHUB_OUTPUT\n" +
                          "echo \"$package_release_notes\" >> $GITHUB_OUTPUT\n" +
                          "echo \"EOF\" >> $GITHUB_OUTPUT")
                  .AddGenericStep(
                      name: "Display Package Release Notes",
                      runCommand: "echo \"Package Release Notes:" +
                      " ${{ steps.extract_package_release_notes.outputs.package_release_notes }}\"")
                  .AddGenericStep(
                      name: "Create GitHub Tag",
                      runCommand:
                          "git tag -a \"v${{ steps.extract_version.outputs.version_number }}\" -m \"Release -" +
                          " v${{ steps.extract_version.outputs.version_number }}\"\n" +
                          "git push origin --tags")
                  .AddActionStep(
                      name: "Create GitHub Release",
                      uses: "actions/create-release@v1",
                      with: new Dictionary<string, string>
                      {
                          ["tag_name"] = "v${{ steps.extract_version.outputs.version_number }}",
                          ["release_name"] = "Release - v${{ steps.extract_version.outputs.version_number }}",
                          ["body"] =
                              "## Release - v${{ steps.extract_version.outputs.version_number }}\n\n" +
                              "### Release Notes\n" +
                              "${{ steps.extract_package_release_notes.outputs.package_release_notes }}"
                      },
                      environmentVariables: new Dictionary<string, string>
                      {
                          ["GITHUB_TOKEN"] = "${{ secrets.PAT_FOR_TAGGING }}"
                      }))

              .AddJob("publish", job => job
                  .WithName("Publish to NuGet")
                  .RunsOn(BuildMachines.UbuntuLatest)
                  .DependsOn("add_tag")
                  .WithCondition("needs.add_tag.result == 'success'")
                  .AddCheckoutStep("Check out")
                  .AddSetupDotNetStep(dotNetVersion)
                  .AddRestoreStep()
                  .AddGenericStep(
                      name: "Build",
                      runCommand: "dotnet build --no-restore --configuration Release")
                  .AddGenericStep(
                      name: "Pack NuGet Package",
                      runCommand: "dotnet pack --configuration Release --include-symbols")
                  .AddGenericStep(
                      name: "Push NuGet Package",
                      runCommand:
                          "dotnet nuget push **/bin/Release/**/*.nupkg " +
                          "--source https://api.nuget.org/v3/index.json " +
                          "--api-key ${{ secrets.NUGET_ACCESS }} --skip-duplicate"))

              .SaveToFile("../../../../.github/workflows/build.yml");
        }

        public void GeneratePrLintScript(string branchName)
        {
            var githubPipeline = new ADotNet.Models.Pipelines.GithubPipelines.DotNets.GithubPipeline
            {
                Name = "PR Linter",

                OnEvents = new ADotNet.Models.Pipelines.GithubPipelines.DotNets.Events
                {
                    PullRequest = new ADotNet.Models.Pipelines.GithubPipelines.DotNets.PullRequestEvent
                    {
                        Types = ["opened", "edited", "synchronize", "reopened", "closed"]
                    }
                },

                Jobs = new Dictionary<string, ADotNet.Models.Pipelines.GithubPipelines.DotNets.Job>
                {
                    {
                        "label",
                        new LabelJobV3(runsOn: BuildMachines.UbuntuLatest)
                        {
                            Name = "Label",
                            Permissions = new Dictionary<string, string>
                            {
                                { "contents", "read" },
                                { "pull-requests", "write" },
                                { "issues", "write" }
                            }
                        }
                    },
                    {
                        "requireIssueOrTask",
                        new RequireIssueOrTaskJobV2("")
                        {
                            Name = "Require Issue Or Task Association",
                        }
                    },
                }
            };

            string buildScriptPath = "../../../../.github/workflows/prLinter.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adotNetClient.SerializeAndWriteToFile(
                adoPipeline: githubPipeline,
                path: buildScriptPath);
        }
    }
}
