// ---------------------------------------------------------------------------
// Copyright (c) Hassan Habib & Shri Humrudha Jagathisun All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------------------

using YamlDotNet.Serialization;

namespace EventHighway.Infrastructure.Models
{
    public class Run
    {
        [YamlMember(Alias = "shell")]
        public string Shell { get; set; }
    }
}
