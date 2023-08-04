using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    internal class Furniture : WorldObject
    {
        public List<Actor> Occupants { get; } = new List<Actor>();
        public int MaxOccupants { get; set; }
        public string InteractMsg { get; set; } // Indicates WHERE you interact with it (e.g. on, in, inside, on top of, etc.)
        public float WeightCapacity { get; set; } // How much weight a piece of furniture can hold before it breaks

        public Furniture() {
            MaxOccupants = 1;
            InteractMsg = "on";
            WeightCapacity = 250f;
        }
    }
}
