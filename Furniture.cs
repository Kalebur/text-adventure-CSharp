using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TextAdventure.Actor;

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

        public void ActorToFurniture(Actor actor, string action)
        {
            string adjustedAction = string.Empty; // For adjusting the action to make more sense, (e.g. sit to sitting, lie to lying, etc.)
            Position positionAfterAction = Position.STANDING;
            switch (action)
            {
                case "stand":
                    adjustedAction = "standing";
                    positionAfterAction = Position.STANDING_OBJECT;
                    break;

                case "sit":
                    adjustedAction = "sitting";
                    positionAfterAction = Position.SITTING;
                    break;

                case "lie":
                    adjustedAction = "lying";
                    positionAfterAction = Position.LYING;
                    break;

                case "sleep":
                    adjustedAction = "sleeping";
                    positionAfterAction = Position.SLEEPING;
                    break;
            }

            // Is player already on something?
            // Can't be on two things at once!
            if (actor.OnFurniture != null)
            {
                Console.WriteLine($"You're already {actor.OnFurniture.EnterMsg} {actor.OnFurniture.ShortDescription}!");
                return;
            }

            // Does this object have room for another person?
            if (Occupants.Count + 1 <= MaxOccupants)
            {
                // Object has room, but is the person trying to go on something
                // they're already on?
                if (Occupants.Contains(actor))
                {
                    Console.WriteLine($"You're already {adjustedAction} {EnterMsg} that!");
                    return;
                }
                else
                {
                    // Everything clears and person can go on object!
                    Console.WriteLine($"You {action} {EnterMsg} {ShortDescription}.");
                    Occupants.Add(actor);
                    actor.CurrentPosition = positionAfterAction;
                    actor.OnFurniture = this;
                    return;
                }
            }
            else
            {
                // Object doesn't have room for anyone else
                Console.WriteLine($"There isn't any more room {EnterMsg} {ShortDescription}!");
                return;
            }
        }
    }
}
