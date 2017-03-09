using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace hots_quick_build_finder.Models
{
    /// <summary>
    ///     A Hero talent build.
    /// </summary>
    [Serializable]
    public class Build
    {
        /// <summary>
        ///     The given name for the build usually representing it's purpose (damage, healing, etc).
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     The collection of talents that this build is composed of.
        /// </summary>
        [JsonProperty("talents")]
        public List<Talent> Talents { get; set; } = new List<Talent>(7);
    }
}