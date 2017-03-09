using System;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace hots_quick_build_finder.Models
{
    /// <summary>
    ///     An individual talent, usually alongside others in a Hero's Build
    /// </summary>
    [Serializable]
    public class Talent
    {
        /// <summary>
        ///     The header of the talent, usually the level requirement.
        /// </summary>
        [JsonProperty("header")]
        public string Header { get; set; }

        /// <summary>
        ///     A path to the talent's icon image.
        /// </summary>
        [JsonProperty("image")]
        public string Image { get; set; }

        /// <summary>
        ///     The position this talent is in on the talent choices for this tier.
        /// </summary>
        [JsonProperty("position")]
        public int Position { get; set; }

        /// <summary>
        ///     The total number of talent choices a hero has at this talent tier.
        /// </summary>
        [JsonProperty("slots")]
        public int Slots { get; set; }

        [JsonIgnore]
        public ObservableCollection<Square> Squares
        {
            get
            {
                var squares = new ObservableCollection<Square>();
                foreach (var number in Enumerable.Range(0, Slots))
                    squares.Add(new Square());
                squares[Position].Selected = true;
                return squares;
            }
        }
    }
}
