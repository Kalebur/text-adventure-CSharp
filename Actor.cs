using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    // Class used to create anything that moves, fights, speaks, etc.
    internal class Actor
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public string ShortDescription { get; set; } // This is used for when they speak or in combat
        public string LongDescription { get; set; } // This is what's displayed to other people in the room
        public string Description { get; set; } // The description of the actor themselves. Seen when you look at them.

        // Primary stats
        public int Level { get; set; }
        public int CurrentExp { get; set; }
        public int ExpToNextLevel { get; set; }
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public int CurrentMP { get; set; }
        public int MaxMP { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public int Gold { get; set; }

        public bool InCombat { get; set; }
        public bool IsPlayer { get; set; }
        public Room CurrentRoom { get; set; }
        public List<WorldObject> Inventory { get; set; } = new List<WorldObject>();
        public Dictionary<WorldObject.WearLocation, WorldObject?> Equipment { get; set; } = new Dictionary<WorldObject.WearLocation, WorldObject?>();

        public Actor()
        {
            Name = "Actor";
            ShortDescription = "the Actor";
            LongDescription = "A generic Actor is just...here...";
            Description = "This Actor couldn't possibly get more generic. They're like that one sponge fellow in the episode of that one TV show when he became 'normal.'";
            Level = 1;
            CurrentExp = 0;
            ExpToNextLevel = 1000;
            CurrentHP = 50;
            MaxHP = 50;
            CurrentMP = 50;
            MaxMP = 50;
            Strength = 10;
            Dexterity = 10;
            Constitution = 10;
            Intelligence = 10;
            Wisdom = 10;
            Charisma = 10;
            Gold = 0;
            InCombat = false;
            CurrentRoom = new Room();
            IsPlayer = false;
            ID = 0;
            Equipment.Add(WorldObject.WearLocation.HEAD, null);
            Equipment.Add(WorldObject.WearLocation.EARS, null);
            Equipment.Add(WorldObject.WearLocation.NECK, null);
            Equipment.Add(WorldObject.WearLocation.BACK, null);
            Equipment.Add(WorldObject.WearLocation.ABOUT, null);
            Equipment.Add(WorldObject.WearLocation.TORSO, null);
            Equipment.Add(WorldObject.WearLocation.WAIST, null);
            Equipment.Add(WorldObject.WearLocation.WRIST_L, null);
            Equipment.Add(WorldObject.WearLocation.WRIST_R, null);
            Equipment.Add(WorldObject.WearLocation.FINGER_L, null);
            Equipment.Add(WorldObject.WearLocation.FINGER_R, null);
            Equipment.Add(WorldObject.WearLocation.WIELD_L, null);
            Equipment.Add(WorldObject.WearLocation.WIELD_R, null);
            Equipment.Add(WorldObject.WearLocation.WIELD_DUAL, null);
            Equipment.Add(WorldObject.WearLocation.LEGS, null);
            Equipment.Add(WorldObject.WearLocation.FEET, null);
            Equipment.Add(WorldObject.WearLocation.OVER, null);
        }

        public void MoveActor(string direction)
        {
            if (InCombat)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You're too busy FIGHTING!");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }

            if (CurrentRoom.HasExit(direction))
            {
                CurrentRoom.GetExit(direction).ConnectedRoom.actorsInRoom.Add(this);
                CurrentRoom.actorsInRoom.Remove(this);
                CurrentRoom = CurrentRoom.GetExit(direction).ConnectedRoom;
                if (IsPlayer)
                {
                    CurrentRoom.DisplayRoom();
                }
            } else
            {
                Console.WriteLine("You get up close and personal with a wall!");
            }

        }

        public void MoveActor(Room room)
        {
            CurrentRoom.actorsInRoom.Remove(this);
            CurrentRoom = room;
            room.actorsInRoom.Add(this);
        }

        public void ShowEquipment()
        {
            bool allEmpty = true;
            Console.WriteLine("\n" + (IsPlayer ? "You are wearing:" : "".PadLeft(5) + ShortDescription + " is wearing:").PadLeft(25));
            foreach (var item in Equipment)
            {
                if (item.Value != null && !IsPlayer)
                {
                    allEmpty = false;
                    Console.Write(item.Key.ToString().PadLeft(30) + ":" + item.Value.ShortDescription);
                } else if (IsPlayer)
                {
                    Console.Write("".PadLeft(5) + item.Key.ToString().PadRight(15) + ":" + "".PadLeft(5));
                    Console.WriteLine((item.Value == null ? "Nothing" : item.Value.ShortDescription).PadRight(20));
                }
            }

            if (allEmpty && !IsPlayer)
            {
                Console.WriteLine("Nothing".PadLeft(20));
            }
        }

        public void ShowInventory()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + (IsPlayer ? "You are carrying:" : ShortDescription + " is carrying:").PadLeft(25));
            if (Inventory.Count == 0)
            {
                Console.WriteLine("Nothing".PadLeft(20));
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }

            foreach (var item in Inventory)
            {
                Console.WriteLine("".PadLeft(12) + item.ShortDescription);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public WorldObject? IsCarrying(string targetObject)
        {
            foreach (var item in Inventory)
            {
                if (item.ShortDescription.ToLower().Contains(targetObject.ToLower()) || item.Keywords.Contains(targetObject.ToLower())) {
                    return item;
                }
            }

            return null;
        }
    }
}
