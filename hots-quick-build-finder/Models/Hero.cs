using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace hots_quick_build_finder.Models
{
    /// <summary>
    ///     A model representing the build orders of a Heroes of the Storm "Hero".
    /// </summary>
    [Serializable]
    public class Hero: IEqualityComparer<Hero>, IComparable<Hero>
    {
        /// <summary>
        ///     The proper title name of the hero.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     The name used internally by the connected service.
        /// </summary>
        [JsonProperty("internal")]
        public string InternalName { get; set; }

        /// <summary>
        ///     Every build that the hero has available retrieved from the service.
        /// </summary>
        [JsonProperty("builds")]
        public List<Build> Builds { get; set; } = new List<Build>();

        /// <summary>
        ///     The date of the last time that the builds for this hero were updated.
        /// </summary>
        [JsonProperty("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public int CompareTo(Hero other) => string.CompareOrdinal(Name, other.Name);

        public bool Equals(Hero x, Hero y) => x.Name.Equals(y.Name);

        public int GetHashCode(Hero obj) => Name.GetHashCode();
    }
}
