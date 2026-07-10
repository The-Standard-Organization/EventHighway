// ---------------------------------------------------------------------------
// Copyright (c) Hassan Habib & Shri Humrudha Jagathisun All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------------------

using YamlDotNet.Serialization;

namespace EventHighway.Infrastructure.Models
{
    public class Events
    {
        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public PushEvent Push { get; set; }

        [YamlMember(Alias = "pull_request", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public PullRequestEvent PullRequest { get; set; }
    }
}
