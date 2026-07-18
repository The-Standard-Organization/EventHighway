// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV4s;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV5s;

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
            var githubPipeline = new Models.GithubPipeline
            {
                Name = projectName,

                OnEvents = new Models.Events
                {
                    Push = new Models.PushEvent { Branches = new[] { branchName } },

                    PullRequest = new Models.PullRequestEvent
                    {
                        Types = new[] { "opened", "synchronize", "reopened", "closed" },
                        Branches = new[] { branchName }
                    }
                },

                Jobs = new Dictionary<string, Models.Job>
                {
                    {
                        "build",
                        new Models.Job
                        {
                            Name = "Build & Unit Tests",
                            RunsOn = BuildMachines.UbuntuLatest,

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV5
                                {
                                    Name = "Check out"
                                },

                                new SetupDotNetTaskV5
                                {
                                    Name = "Setup Dot Net Version",

                                    With = new TargetDotNetVersionV5
                                    {
                                        DotNetVersion = dotNetVersion
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restore"
                                },

                                new DotNetBuildTask
                                {
                                    Name = "Build"
                                },

                                new TestTask
                                {
                                    Name = "Test - Core Unit",
                                    Run = "dotnet test EventHighway.Core.Tests.Unit --no-build --verbosity normal"
                                },

                                new TestTask
                                {
                                    Name = "Test - EventHandlers Unit",
                                    Run = "dotnet test EventHighway.EventHandlers.Tests.Unit --no-build --verbosity normal"
                                },

                                new TestTask
                                {
                                    Name = "Test - EventHandlers Acceptance",
                                    Run = "dotnet test EventHighway.EventHandlers.Tests.Acceptance --no-build --verbosity normal"
                                },

                                new TestTask
                                {
                                    Name = "Test - Portal Unit",
                                    Run = "dotnet test EventHighway.Portal.Web.Tests.Unit --no-build --verbosity normal"
                                }
                            }
                        }
                    },
                    {
                        "build-integration",
                        new Models.Job
                        {
                            Name = "Build & Acceptance Tests (DB matrix)",
                            RunsOn = BuildMachines.UbuntuLatest,

                            Strategy = new Models.Strategy
                            {
                                FailFast = false,

                                Matrix = new Dictionary<string, object>
                                {
                                    ["provider"] = new List<string> { "sqlserver", "postgres" }
                                },

                                Include = new List<Dictionary<string, string>>
                                {
                                    new Dictionary<string, string>
                                    {
                                        ["provider"] = "sqlserver",

                                        ["connection_string"] =
                                            "Server=localhost;Database=EventHighwayDb;User Id=sa;" +
                                            "Password=Your_password123!;TrustServerCertificate=True;" +
                                            "MultipleActiveResultSets=true;Pooling=false"
                                    },
                                    new Dictionary<string, string>
                                    {
                                        ["provider"] = "postgres",

                                        ["connection_string"] =
                                            "Host=localhost;Database=EventHighwayDb;" +
                                            "Username=postgres;Password=postgres;Pooling=false"
                                    }
                                }
                            },

                            EnvironmentVariables = new Dictionary<string, string>
                            {
                                ["PROVIDER"] = "${{ matrix.provider }}",
                                ["CONNECTION_STRING"] = "${{ matrix.connection_string }}"
                            },

                            Services = new Dictionary<string, Models.Service>
                            {
                                {
                                    "sqlserver",
                                    new Models.Service
                                    {
                                        Image = "mcr.microsoft.com/mssql/server:2019-latest",

                                        Environment = new Dictionary<string, string>
                                        {
                                            ["ACCEPT_EULA"] = "Y",
                                            ["SA_PASSWORD"] = "Your_password123!"
                                        },

                                        Ports = new List<string> { "1433:1433" },

                                        Options =
                                    "--health-cmd \"/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa " +
                                    "-P Your_password123! -N -C -Q 'SELECT 1' || exit 1\" " +
                                    "--health-interval 10s --health-timeout 5s --health-retries 10"
                                    }
                                },
                                {
                                    "postgres",
                                    new Models.Service
                                    {
                                        Image = "postgres:17",

                                        Environment = new Dictionary<string, string>
                                        {
                                            ["POSTGRES_DB"] = "EventHighwayDb",
                                            ["POSTGRES_USER"] = "postgres",
                                            ["POSTGRES_PASSWORD"] = "postgres"
                                        },

                                        Ports = new List<string> { "5432:5432" },

                                        Options =
                                    "--health-cmd pg_isready --health-interval 10s " +
                                    "--health-timeout 5s --health-retries 5"
                                    }
                                }
                            },

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV5
                                {
                                    Name = "Check out"
                                },

                                new SetupDotNetTaskV5
                                {
                                    Name = "Setup Dot Net Version",

                                    With = new TargetDotNetVersionV5
                                    {
                                        DotNetVersion = "10.0.100"
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restore"
                                },

                                new DotNetBuildTask
                                {
                                    Name = "Build"
                                },

                                new GithubTask
                                {
                                    Name = "Test",
                                    Run = "dotnet test EventHighway.Core.Tests.Acceptance --no-build --verbosity normal"
                                }
                            }
                        }
                    },
                    {
                        "add_tag",
                        new Models.Job
                        {
                            Name = "Tag and Release",
                            RunsOn = BuildMachines.UbuntuLatest,
                            Needs = new[] { "build", "build-integration" },
                            If =
                                "needs.build.result == 'success' && " +
                                "needs.build-integration.result == 'success' && " +
                                "github.event.pull_request.merged && " +
                                $"github.event.pull_request.base.ref == '{branchName}' && " +
                                "startsWith(github.event.pull_request.title, 'RELEASES:') && " +
                                "contains(github.event.pull_request.labels.*.name, 'RELEASES')",

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV4
                                {
                                    Name = "Checkout code",

                                    With = new Dictionary<string, string>
                                    {
                                        ["token"] = "${{ secrets.PAT_FOR_TAGGING }}"
                                    }
                                },

                                new ConfigureGitTask
                                {
                                    Name = "Configure Git"
                                },

                                new ExtractProjectPropertyTask(
                                    name: "Extract Version",
                                    id: "extract_version",
                                    projectRelativePath: $"{projectName}/{projectName}.csproj",
                                    propertyName: "Version",
                                    stepVariableName: "version_number",
                                    runsOn: BuildMachines.UbuntuLatest),

                                new GithubTask
                                {
                                    Name = "Display Version",
                                    Run = "echo \"Version number: ${{ steps.extract_version.outputs.version_number }}\""
                                },

                                new ExtractProjectPropertyTask(
                                    name: "Extract Package Release Notes",
                                    id: "extract_package_release_notes",
                                    projectRelativePath: $"{projectName}/{projectName}.csproj",
                                    propertyName: "PackageReleaseNotes",
                                    stepVariableName: "package_release_notes",
                                    runsOn: BuildMachines.UbuntuLatest),

                                new GithubTask
                                {
                                    Name = "Display Package Release Notes",
                                    Run = "echo \"Package Release Notes: " +
                                        "${{ steps.extract_package_release_notes.outputs.package_release_notes }}\""
                                },

                                new CreateGitHubTagTask(
                                    tagName: "v${{ steps.extract_version.outputs.version_number }}",
                                    tagMessage: "Release - v${{ steps.extract_version.outputs.version_number }}")
                                {
                                    Name = "Create GitHub Tag"
                                },

                                new CreateGitHubReleaseTask(
                                    releaseName: "Release - v${{ steps.extract_version.outputs.version_number }}",
                                    tagName: "v${{ steps.extract_version.outputs.version_number }}",
                                    releaseNotes: "${{ steps.extract_package_release_notes.outputs.package_release_notes }}",
                                    githubToken: "${{ secrets.PAT_FOR_TAGGING }}")
                                {
                                    Name = "Create GitHub Release",
                                    Uses = "actions/create-release@v1"
                                }
                            }
                        }
                    },
                    {
                        "publish",
                        new Models.Job
                        {
                            Name = "Publish to NuGet",
                            RunsOn = BuildMachines.UbuntuLatest,
                            Needs = new[] { "add_tag" },
                            If = "needs.add_tag.result == 'success'",

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV5
                                {
                                    Name = "Check out"
                                },

                                new SetupDotNetTaskV5
                                {
                                    Name = "Setup Dot Net Version",

                                    With = new TargetDotNetVersionV5
                                    {
                                        DotNetVersion = dotNetVersion
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restore"
                                },

                                new DotNetBuildReleaseTask
                                {
                                    Name = "Build"
                                },

                                new PackNugetTaskWithSymbols
                                {
                                    Name = "Pack NuGet Package"
                                },

                                new NugetPushTask(nugetApiKey: "${{ secrets.NUGET_ACCESS }}")
                                {
                                    Name = "Push NuGet Package"
                                }
                            }
                        }
                    }
                }
            };
        }

        public void GeneratePrLintScript(string branchName)
        {
            var labelJob = new LabelJobV3(runsOn: BuildMachines.UbuntuLatest)
            {
                Name = "Label"
            };

            labelJob.Steps[0].With["script"] = LabelScript;

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
                        labelJob
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

        // PR title prefixes recognised by the labeler. Canonical Standard list
        // (ADotNet LabelJobV3 master) extended with prefixes used in this repo:
        // PORTAL:, POC: and CONFIGURATIONS:.
        private const string LabelScript =
            """
            const prefixes = [
              'INFRA:',
              'MINOR INFRA:',
              'MEDIUM INFRA:',
              'MAJOR INFRA:',
              'PROVISIONS:',
              'RELEASES:',
              'DATA:',
              'MINOR DATA:',
              'MEDIUM DATA:',
              'MAJOR DATA:',
              'MIGRATIONS:',
              'BROKERS:',
              'MINOR BROKERS:',
              'MEDIUM BROKERS:',
              'MAJOR BROKERS:',
              'PROVIDERS:',
              'FOUNDATIONS:',
              'MINOR FOUNDATIONS:',
              'MEDIUM FOUNDATIONS:',
              'MAJOR FOUNDATIONS:',
              'PROCESSINGS:',
              'MINOR PROCESSINGS:',
              'MEDIUM PROCESSINGS:',
              'MAJOR PROCESSINGS:',
              'ORCHESTRATIONS:',
              'MINOR ORCHESTRATIONS:',
              'MEDIUM ORCHESTRATIONS:',
              'MAJOR ORCHESTRATIONS:',
              'COORDINATIONS:',
              'MINOR COORDINATIONS:',
              'MEDIUM COORDINATIONS:',
              'MAJOR COORDINATIONS:',
              'MANAGEMENTS:',
              'MINOR MANAGEMENTS:',
              'MEDIUM MANAGEMENTS:',
              'MAJOR MANAGEMENTS:',
              'AGGREGATIONS:',
              'MINOR AGGREGATIONS:',
              'MEDIUM AGGREGATIONS:',
              'MAJOR AGGREGATIONS:',
              'CONTROLLERS:',
              'MINOR CONTROLLERS:',
              'MEDIUM CONTROLLERS:',
              'MAJOR CONTROLLERS:',
              'CLIENTS:',
              'MINOR CLIENTS:',
              'MEDIUM CLIENTS:',
              'MAJOR CLIENTS:',
              'EXPOSERS:',
              'MINOR EXPOSERS:',
              'MEDIUM EXPOSERS:',
              'MAJOR EXPOSERS:',
              'BASE:',
              'MINOR BASE:',
              'MEDIUM BASE:',
              'MAJOR BASE:',
              'COMPONENTS:',
              'MINOR COMPONENTS:',
              'MEDIUM COMPONENTS:',
              'MAJOR COMPONENTS:',
              'VIEWS:',
              'MINOR VIEWS:',
              'MEDIUM VIEWS:',
              'MAJOR VIEWS:',
              'PAGES:',
              'MINOR PAGES:',
              'MEDIUM PAGES:',
              'MAJOR PAGES:',
              'PORTAL:',
              'ACCEPTANCE:',
              'MINOR ACCEPTANCE:',
              'MEDIUM ACCEPTANCE:',
              'MAJOR ACCEPTANCE:',
              'INTEGRATIONS:',
              'MINOR INTEGRATIONS:',
              'MEDIUM INTEGRATIONS:',
              'MAJOR INTEGRATIONS:',
              'CODE RUB:',
              'MINOR CODE RUB:',
              'MEDIUM CODE RUB:',
              'MAJOR CODE RUB:',
              'MINOR FIX:',
              'MEDIUM FIX:',
              'MAJOR FIX:',
              'DOCUMENTATION:',
              'CONFIG:',
              'CONFIGURATIONS:',
              'STANDARD:',
              'DESIGN:',
              'MINOR DESIGN:',
              'MEDIUM DESIGN:',
              'MAJOR DESIGN:',
              'BUSINESS:',
              'POC:',
              'PLANNING:',
              'MINOR PLANNING:',
              'MAJOR PLANNING:',
              'MENTORSHIP:',
              'MINOR MENTORSHIP:',
              'MAJOR MENTORSHIP:',
              'DISCUSSION:',
              'MINOR DISCUSSION:',
              'MAJOR DISCUSSION:',
              'IMPORTS:',
              'REVIEWS:',
              'STATUS:'
            ];

            const pullRequest = context.payload.pull_request;

            if (!pullRequest) {
              console.log('No pull request context available.');
              return;
            }

            const title = context.payload.pull_request.title;
            const existingLabels = context.payload.pull_request.labels.map(label => label.name);

            for (const prefix of prefixes) {
              if (title.startsWith(prefix)) {
                const label = prefix.slice(0, -1);
                if (!existingLabels.includes(label)) {
                  console.log(`Applying label: ${label}`);
                  await github.rest.issues.addLabels({
                    owner: context.repo.owner,
                    repo: context.repo.repo,
                    issue_number: context.payload.pull_request.number,
                    labels: [label]
                  });
                }
                break;
              }
            }
            """;
    }
}
