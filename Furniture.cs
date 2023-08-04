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
        public string EnterMsg { get; set; } // Indicates WHERE you interact with it (e.g. on, in, inside, on top of, etc.)
        public string ExitMsg { get; set; }
        public float WeightCapacity { get; set; } // How much weight a piece of furniture can hold before it breaks
        public float CurrentLoad { get; set; }

        public Furniture() {
            MaxOccupants = 1;
            EnterMsg = "on";
            ExitMsg = "get off of";
            WeightCapacity = 250f;
            CurrentLoad = 0;
        }
    }
}
