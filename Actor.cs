﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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

        // Battle / status effect flags
        public bool InCombat { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsAsleep { get; set; }
        public bool IsConscious { get; set; } = true;
        public bool CanWake { get; set; }

        // Location, inventory and equipment fields
        public Room CurrentRoom { get; set; }
        public List<WorldObject> Inventory { get; set; } = new List<WorldObject>();
        public Dictionary<WorldObject.WearLocation, WorldObject?> Equipment { get; set; } = new Dictionary<WorldObject.WearLocation, WorldObject?>();
        public List<Actor> Opponents { get; } = new List<Actor>();

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
            CurrentRoom = Room.nullRoom;
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

        // TODO: Implement status screen
        public void ShowStats()
        {

        }

        private static Actor CloneActor(Actor actorToClone)
        {
            return new Actor()
            {
                ID = actorToClone.ID,
                Name = actorToClone.Name,
                ShortDescription = actorToClone.ShortDescription,
                LongDescription = actorToClone.LongDescription,
                Description = actorToClone.Description,
                Level = actorToClone.Level,
                CurrentExp = actorToClone.CurrentExp,
                CurrentHP = actorToClone.CurrentHP,
                MaxHP = actorToClone.MaxHP,
                CurrentMP = actorToClone.CurrentMP,
                MaxMP = actorToClone.MaxMP,
                Strength = actorToClone.Strength,
                Dexterity = actorToClone.Dexterity,
                Constitution = actorToClone.Constitution,
                Intelligence = actorToClone.Intelligence,
                Wisdom = actorToClone.Wisdom,
                Charisma = actorToClone.Charisma,
                Gold = actorToClone.Gold,
            };
        }

        public static void SpawnActor(Actor actor, Room spawnLocation)
        {
            CloneActor(actor).MoveActor(spawnLocation);
        }

        private void DisplayActorInfo()
        {
            Console.WriteLine("****************************************");
            Console.Write($"*{Game.CenterText("", 38)}*\n");
            Console.Write($"*{Game.CenterText(ShortDescription, 38)}*\n");
            Console.Write($"*{Game.CenterText("", 38)}*\n");
            Console.WriteLine("****************************************");
            Console.WriteLine($"Level: {Level.ToString().PadRight(10)} ID: {ID.ToString().PadRight(10)}");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Short Desc: {ShortDescription}");
            Console.WriteLine($"Long Desc: {LongDescription}");
            Console.WriteLine("Description:");
            Console.WriteLine($"{Game.UniversalPadding}{Description}\n");
            Console.WriteLine($"HP: {CurrentHP}/{MaxHP}  MP: {CurrentMP}/{MaxMP} Gold: {Gold}");
            ShowEquipment();
            ShowInventory();
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

        /*****************************************************************
         *                                                               *
         *                                                               *
         *                     PLAYER ACTION COMMANDS                    *
         *                                                               *
         *                                                               *
         *****************************************************************/

        /// <summary>
        /// Handles player picking up things or getting them out of containers.
        /// </summary>
        /// <param name="args">
        /// A string which will be split into individual arguments, which are then handled based on the number of args.
        /// Arg 1: The item to pick up or take.
        /// Arg 2: A container to search for either in the player's inventory or within the room itself if not carried by player.
        /// </param>
        public void DoGet(string args)
        {
            if (args == "get")
            {
                Console.WriteLine("Get what, exactly?");
                return;
            }
            string[] splitArgs = args.Split(' ');
            

            // If args is only two values, get object from current room
            if (splitArgs.Length == 1)
            {
                string objToGet = splitArgs[0];
                WorldObject? targetObject = CurrentRoom.ObjectInRoom(objToGet);

                if (targetObject != WorldObject.nullObject && targetObject.ObjectFlags["canTake"])
                {
                    targetObject.ObjectToActor(this);
                    targetObject.ObjectFromRoom(this.CurrentRoom);
                    Console.WriteLine($"You get {targetObject.ShortDescription}.");
                }
                else if (targetObject != WorldObject.nullObject && !targetObject.ObjectFlags["canTake"])
                {
                    Console.WriteLine("Sorry, you can't take that. Not EVERYTHING is just free for the taking, you know! Ugh, greedy adventurers...");
                }
                else
                {
                    Console.WriteLine("You don't see anything like that here.");
                }
            }
            // If args is two+ values, try to take something from a container
            else
            {
                string objToTake = splitArgs[0].ToLower();
                string targetContainer = splitArgs[1].ToLower();
                string itemNotFoundMessage = "There's nothing like that in ";
                // First, check if the container is something the player is carrying
                
                if (IsCarrying(targetContainer, out WorldObject item)) {
                    Container c = (Container)item;
                    c.ObjectFromContainer(objToTake, itemNotFoundMessage, this);
                } else if (IsCarrying(targetContainer) == -1)
                {
                    // Player isn't carrying the container, so look for it in the room they're in
                    var roomContainer = (Container)CurrentRoom.ObjectInRoom(targetContainer);
                    if (roomContainer != WorldObject.nullObject)
                    {
                        roomContainer.ObjectFromContainer(objToTake, itemNotFoundMessage, this);
                    } else if (roomContainer != WorldObject.nullObject && 
                        roomContainer is Container)
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


        public void DoLook(string args)
        {
            if (args == "look")
            {
                CurrentRoom.DisplayRoom();
            }
            else
            {
                string target = args.Split(' ')[0];
                if (target == "self")
                {
                    Console.WriteLine("You look yourself over.");
                    Console.WriteLine(Description);
                    ShowEquipment();
                } else
                {
                    foreach (Actor actor in CurrentRoom.actorsInRoom)
                    {
                        if (actor.Name.ToLower().Contains(target) || actor.ShortDescription.ToLower().Contains(target))
                        {
                            Console.WriteLine(actor.Description);
                            actor.ShowEquipment();
                            return;
                        }
                    }
                    Console.WriteLine("You don't see anyone like that here.");
                }
                
            }
        }

        public void DoDrop(string args)
        {
            if (args == "drop")
            {
                Console.WriteLine("Drop what?");
                return;
            }
            else if (IsCarrying(args, out WorldObject targetItem))
            {
                targetItem.ObjectToRoom(CurrentRoom);
                Console.WriteLine($"You drop {targetItem.ShortDescription}.");
                Inventory.Remove(targetItem);
                return;
            }
            else
            {
                Console.WriteLine("You aren't carrying that.");
                return;
            }
        }

        public void DoExamine(string args)
        {
            if (args == "exa" ||  args == "examine")
            {
                Console.WriteLine("Examining imaginary objects?");
                return;
            } else
            {
                string[] splitArgs = args.Split(' ');
                string targetObject = splitArgs[0];
                if (IsCarrying(targetObject, out WorldObject carriedItem))
                {
                    carriedItem.DisplayObjectInfo();
                } else
                {
                    Console.WriteLine("You aren't carrying that.");

                }
            }
        }

        public static void DoOstat(string args)
        {
            string[] splitArgs = args.Split(' ');
            try
            {
                int areaID = int.Parse(splitArgs[0]);
                int objectID = int.Parse(splitArgs[1]);
                try
                {
                    var targetObj = Game.areas[areaID].Objects[objectID];
                    targetObj.DisplayFullObjectInfo();
                }
                catch
                {
                    Console.WriteLine("Unable to locate specified item.");
                }
            }
            catch
            {
                Console.WriteLine("Get stats for which object?");
                Console.WriteLine("Usage: ostat AREA_ID OBJECT_ID");
                Console.WriteLine("Ex: ostat 0 2");
                return;
            }
        }

        public void DoRemove(string args)
        {
            if (args == "rem" || args == "remove")
            {
                Console.WriteLine("Remove WHAT?!");
                return;
            }
            else
            {
                foreach (var wornItem in Equipment)
                {
                    if (wornItem.Value != null &&
                        (wornItem.Value.Keywords.Contains(args) || wornItem.Value.ShortDescription.Contains(args)))
                    {
                        wornItem.Value.ObjectToActor(this);
                        Equipment[wornItem.Key] = null;
                        Console.WriteLine($"You remove {wornItem.Value.ShortDescription}.");
                        return;
                    }
                }
                Console.WriteLine("You're not wearing that.");
            }
        }

        public void DoSay(string args)
        {
            if (args == "say")
            {
                Console.WriteLine("You open your mouth to speak but change your mind.");
                return;
            }

            Game.PrintColoredText($"You say, '{args}'", ConsoleColor.Cyan, true);
        }

        public void DoKill(string args)
        {
            if (args == "self")
            {
                Console.WriteLine("Do...do I need to schedule you a therapist appointment?");
                return;
            } else if (args == "kill" || args == "attack")
            {
                Console.WriteLine("You take a swing at an imaginary target. CRITICAL HIT!");
            } else
            {
                foreach (Actor actor in this.CurrentRoom.actorsInRoom)
                {
                    if (actor.Name.ToLower().Contains(args) || actor.ShortDescription.ToLower().Contains(args))
                    {
                        Thread battleManager = new(() => Game.StartCombat(this, actor));
                        battleManager.Start();
                        return;
                    }
                }
                Console.WriteLine("You don't see anyone like that here.");
            }
        }

        public void DoMstat(string args)
        {
            static void DisplayUsage()
            {
                Console.WriteLine("Get stats on which mob?");
                Console.WriteLine("Usage: mstat [AREA ID] [ACTOR ID]");
                Console.WriteLine("Alt Usage: mstat [TARGET 'e.g; skeleton']");
            }

            if (args == "mstat")
            {
                DisplayUsage();
            } else
            {
                string[] splitArgs = args.Split(' ');

                // If the length of args is only one, act as if looking for
                // target by name
                if (splitArgs.Length == 1) {
                    string target = splitArgs[0];

                    foreach (Actor mob in CurrentRoom.actorsInRoom)
                    {
                        if (mob.Name.ToLower().Contains(target) ||
                            mob.ShortDescription.ToLower().Contains(target))
                        {
                            mob.DisplayActorInfo();
                            return;
                        }
                    }

                    Console.WriteLine("You don't see anyone like that here.");
                } else
                {
                    try
                    {
                        int areaID = int.Parse(splitArgs[0]);
                        int actorID = int.Parse(splitArgs[1]);
                        Game.areas[areaID].Actors[actorID].DisplayActorInfo();
                    }
                    catch
                    {
                        Console.WriteLine("Unable to find specified actor.");
                    }
                }
            }
        }
    }
}
