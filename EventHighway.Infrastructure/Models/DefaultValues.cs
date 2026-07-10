// ---------------------------------------------------------------------------
// Copyright (c) Hassan Habib & Shri Humrudha Jagathisun All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------------------

using YamlDotNet.Serialization;

namespace EventHighway.Infrastructure.Models
{
    public class DefaultValues
    {
        [YamlMember(Alias = "run")]
        public Run Run { get; set; }
    }
}
