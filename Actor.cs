using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public bool IsAsleep { get; set; }
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

        /// <summary>
        /// Shows the list of everything the player currently has equipped.
        /// Empty slots will show up as "Nothing."
        /// </summary>
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

        public int IsCarrying(string targetObject)
        {
            for (int i = 0; i < Inventory.Count; i++)
            {
                if (Inventory[i].ShortDescription.ToLower().Contains(targetObject.ToLower())
                    || Inventory[i].Keywords.Contains(targetObject.ToLower())) {
                    return i;
                }
            }

            return -1;
        }

        public bool IsCarrying(string targetObject, out WorldObject carriedItem)
        {
            foreach (var item in Inventory)
            {
                if (item.ShortDescription.ToLower().Contains(targetObject.ToLower()) ||
                    item.Keywords.Contains(targetObject.ToLower()))
                {
                    carriedItem = item;
                    return true;
                }
            }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            carriedItem = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            return false;
        }

        public void ExamineObject(string targetObject)
        {
            int itemIndex = IsCarrying(targetObject);
            if (itemIndex != -1)
            {
                Inventory[itemIndex].DisplayObjectInfo();
            } else
            {
                Console.WriteLine("You aren't carrying that.");
            }
        }

        public void DoGet(string args)
        {
            if (args == "get")
            {
                Console.WriteLine("Get what, exactly?");
                return;
            }
            string[] splitArgs = args.Split(' ');
            

            // If args is only two items, get object from current room
            if (splitArgs.Length == 1)
            {
                string objToGet = splitArgs[0];
                WorldObject? targetObject = CurrentRoom.ObjectInRoom(objToGet);

                if (targetObject != null && targetObject.ObjectFlags["canTake"])
                {
                    targetObject.ObjectToActor(this);
                    targetObject.ObjectFromRoom(this.CurrentRoom);
                    Console.WriteLine($"You get {targetObject.ShortDescription}.");
                }
                else if (targetObject != null && !targetObject.ObjectFlags["canTake"])
                {
                    Console.WriteLine("Sorry, you can't take that. Not EVERYTHING is just free for the taking, you know! Ugh, greedy adventurers...");
                }
                else
                {
                    Console.WriteLine("You don't see anything like that here.");
                }
            }
            // If args is two+ items, try to take something from a container
            else
            {
                string objToTake = splitArgs[0].ToLower();
                string targetContainer = splitArgs[1].ToLower();
                string itemNotFoundMessage = "There's nothing like that in ";
                // First, check if the container is something the player is carrying
                int objIndex = IsCarrying(targetContainer);
                if (objIndex != -1) {
                    Container c = (Container)Inventory[objIndex];
                    c.ObjectFromContainer(objToTake, itemNotFoundMessage, this);
                } else if (objIndex == -1)
                {
                    // Player isn't carrying the container, so look for it in the room they're in
                    var roomContainer = (Container)CurrentRoom.ObjectInRoom(targetContainer);
                    if (roomContainer != null)
                    {
                        roomContainer.ObjectFromContainer(objToTake, itemNotFoundMessage, this);
                    } else if (roomContainer != null && !Container.IsContainer(roomContainer))
                    {
                        Console.WriteLine("That's not a container!");
                    } else
                    {
                        Console.WriteLine("There's nothing like that here.");
                        return;
                    }
                    
                }
            }
        }
    }
}
