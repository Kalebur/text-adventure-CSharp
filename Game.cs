using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TextAdventure
{
    internal static class Game
    {
        private static bool isPlaying = true;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static Actor _player;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public static readonly string UniversalPadding = "".PadLeft(5);
        public static readonly string GlobalLineTerminator = "\n";
        public static Random diceRoller = new();

        public static Actor Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public static readonly List<Area> areas = new();

        private static void QuitGame()
        {
            if (Player.InCombat)
            {
                PrintColoredText("You can't quit while in combat!", ConsoleColor.Red, true);
                return;
            }
            isPlaying = false;
        }

        public static string CenterText(string text, int totalLength = 40)
        {
            int startIndex = (totalLength - text.Length) / 2;
            int leftPadding = startIndex - 1;
            int rightPadding = totalLength - leftPadding;

            return $"{"".PadLeft(leftPadding)}{text.PadRight(rightPadding)}";
        }


        public static void ProcessCommand(string command, Actor performingActor = null)
        {
            string[] splitCommand = command.ToLower().Split(' ');
            int targetIndex;
            string args = command[(command.IndexOf(' ') + 1)..];
            if (performingActor == null)
            {
                performingActor = Player;
            }
            switch (splitCommand[0]) {
                case "quit":
                    QuitGame();
                    break;

                case "look":
                    performingActor.DoLook(args.ToLower());
                    break;

                case "ne":
                case "northeast":
                    performingActor.MoveActor("Northeast");
                    break;

                case "n":
                case "north":
                    performingActor.MoveActor("North");
                    break;

                case "s":
                case "south":
                    performingActor.MoveActor("South");
                    break;

                case "se":
                case "southeast":
                    performingActor.MoveActor("Southeast");
                    break;

                case "sw":
                case "southwest":
                    performingActor.MoveActor("Southwest");
                    break;

                case "e":
                case "east":
                    performingActor.MoveActor("East");
                    break;

                case "w":
                case "west":
                    performingActor.MoveActor("West");
                    break;

                case "u":
                case "up":
                    performingActor.MoveActor("Up");
                    break;

                case "d":
                case "down":
                    performingActor.MoveActor("Down");
                    break;

                case "astat":
                    areas[performingActor.CurrentRoom.AreaID].DisplayArea();
                    break;

                case "inv":
                case "inventory":
                case "i":
                    performingActor.ShowInventory();
                    break;

                case "eq":
                    performingActor.ShowEquipment();
                    break;

                case "get":
                    performingActor.DoGet(args.ToLower());
                    break;

                case "drop":
                    performingActor.DoDrop(args.ToLower());
                    break;

                case "wear":
                    performingActor.DoWear(args.ToLower());
                    break;

                case "remove":
                case "rem":
                    performingActor.DoRemove(args.ToLower());
                    break;

                case "alist":
                    DisplayAreaList();
                    break;

                case "ostat":
                    Actor.DoOstat(args.ToLower());
                    break;

                case "exa":
                case "examine":
                    performingActor.DoExamine(args.ToLower());
                    break;

                case "say":
                    performingActor.DoSay(args);
                    break;

                case "put":
                    if (splitCommand.Length < 3)
                    {
                        Console.WriteLine("Put WHAT WHERE?!");
                        return;
                    } else
                    {
                        string itemToStore = splitCommand[1];
                        string targetContainer = splitCommand[2];
                        targetIndex = performingActor.IsCarrying(itemToStore);
                        int containerIndex;
                        Container container;

                        // If player isn't carrying target item
                        if (targetIndex == -1)
                        {
                            Console.WriteLine("You aren't carrying that.");
                        } else if (targetIndex != -1) // If player IS carrying target item
                        {
                            // Check if player is carrying target container
                            containerIndex = performingActor.IsCarrying(targetContainer);
                            if (containerIndex == -1)
                            {
                                // If player isn't carrying target container, check to see if
                                // the container is an object in the room
                                container = (Container)performingActor.CurrentRoom.ObjectInRoom(targetContainer);
                                if (container == null)
                                {
                                    Console.WriteLine("There's nothing like that here.");
                                    return;
                                } else if (container is not Container)
                                {
                                    Console.WriteLine("That's not a container.");
                                    return;
                                } else
                                {
                                    container.ObjectToContainer(performingActor.Inventory[targetIndex]);
                                }
                            } 

                            else
                            {
                                // performingActor is carrying both target item and target container
                                if (itemToStore == targetContainer)
                                {
                                    // Prevent player from putting a container inside itself.
                                    Console.WriteLine("You can't put containers in themselves! Are you TRYING to destroy the universe?!");
                                    return;
                                }
                                else if (!(performingActor.Inventory[containerIndex] is Container))
                                {
                                    Console.WriteLine("That's not a container.");
                                    return;
                                }
                                {
                                    // performingActor has both items and isn't trying to create a paradox
                                    // Store item in container and stored item from player's inventory
                                    container = (Container)performingActor.Inventory[containerIndex];
                                    container.ObjectToContainer(performingActor.Inventory[targetIndex]);
                                }
                            }
                        }
                    }
                    break;

                case "oinvoke":
                    SpawnObject(performingActor, args.ToLower());
                    break;

                case "kill":
                case "attack":
                case "k":
                    performingActor.DoKill(args.ToLower());
                    break;

                case "mstat":
                    performingActor.DoMstat(args.ToLower());
                    break;

                case "mpforce":
                    performingActor.DoMPForce(args.ToLower());
                    break;

                default:
                    Console.WriteLine("WHAT?!");
                    break;
            }
        }


        public static void DisplayPrompt()
        {
            Console.WriteLine($"\nHP: {Player.CurrentHP}/{Player.MaxHP} MP:{Player.CurrentMP}/{Player.MaxMP} Gold: {Player.Gold}\n");
            //Console.Write("> ");
        }

        public static void InitializeGame()
        {
            LoadAreas();
            LoadPlayer();
        }

        private static void LoadPlayer()
        {
            Player = new Actor
            {
                IsPlayer = true,
                Name = "Gort",
                ShortDescription = "Gort",
                Description = "This adventurer has an aura of such daring that it's illegal in three countries! (Seriously, they're wanted in two of those already!)",
                CurrentHP = 500,
                MaxHP = 500
            };
            Player.MoveActor(areas[0].Rooms[0]);
        }

        public static void Play()
        {
            string? userInput;
            Player.CurrentRoom.DisplayRoom();

            while (Game.isPlaying)
            {
                DisplayPrompt();
                userInput = Console.ReadLine();
#pragma warning disable CS8604 // Possible null reference argument.
                Game.ProcessCommand(userInput);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        public static void LoadAreas()
        {
            var fileList = Directory.EnumerateFiles(@"D:\Creative Stuff\Programming Projects\CSharp\TextAdventure\Areas\");
            foreach (var file in fileList)
            {
                ParseAreaFile(file);
            }

            string areaData;
            foreach (var file in fileList)
            {
                areaData = File.ReadAllText(file);
                int areaID = ParseID(ref areaData);

                ParseObjects(areas[areaID], ref areaData);
                ParseActors(areas[areaID], ref areaData);
                ParseRooms(areas[areaID], ref areaData);
            }
        }

        public static void TrimCurrentLine(ref string stringToTrim)
        {
            stringToTrim = stringToTrim[(stringToTrim.IndexOf("\n") + 1)..];
        }

        // Returns the value substring between identifier and terminator
        // Calling function will be responsible for any type conversion required
        private static string ParseValue(ref string data, string identifier, string terminator)
        {
            int stopIndex = data.IndexOf(terminator);
            int dataLength = stopIndex - identifier.Length;
            return data.Substring(identifier.Length, dataLength).Trim();
        }

        public static int ParseID(ref string areaData)
        {
            _ = int.TryParse(ParseValue(ref areaData, "ID: ", "\n"), out int id);
            areaData = areaData[(areaData.IndexOf("\n") + 1)..];
            return id;
        }

        public static string ParseDescription(ref string data)
        {
            return ParseValue(ref data, "Description: ", "\n");
        }

        public static string ParseName(ref string data)
        {
            return ParseValue(ref data, "Name: ", "\n");
        }

        public static int ParseEntityCount(string data, string entity)
        {
            _ = int.TryParse(ParseValue(ref data, entity + " Count: ", "\n"), out int count);
            return count;
        }

        public static void ParseRooms(Area area, ref string areaData)
        {
            for (int i = 0; i < area.Rooms.Count; i++)
            {
            ParseRoom(area, ref areaData);
            }
        }

        public static void ParseRoom(Area area, ref string areaData)
        {
            string dividerText = "--ROOM--";
            string terminator = "--END_ROOM--";
            string trimmedData = areaData.Substring(areaData.IndexOf(dividerText));
            trimmedData = trimmedData.Substring(trimmedData.IndexOf("\n") + 1);
            int dataSize = trimmedData.IndexOf(terminator) + terminator.Length;
            string roomData = trimmedData[..dataSize];

            int roomID = ParseID(ref roomData);
            string roomName = ParseName(ref roomData);
            roomData = roomData.Substring(roomData.IndexOf("\n") + 1);
            string roomDescription = ParseDescription(ref roomData);
            roomData = roomData.Substring(roomData.IndexOf("\n") + 1);
            ParseExits(area.Rooms[roomID], roomData);
            areaData = areaData.Substring(areaData.IndexOf(terminator) + (terminator.Length + 2));

            area.Rooms[roomID].ID = roomID;
            area.Rooms[roomID].Name = roomName;
            area.Rooms[roomID].Description = roomDescription;
        }

        public static void ParseActors(Area area, ref string areaData)
        {
            string actorList = areaData[areaData.IndexOf("**ACTOR_LIST**")..];
            string actorInitializer = "--ACTOR--";

            while (actorList.IndexOf(actorInitializer) != -1)
            {
                string currentActor = actorList[actorList.IndexOf(actorInitializer)..];
                TrimCurrentLine(ref currentActor);
                Actor actor = new Actor();
                actor.ID = ParseID(ref currentActor);
                actor.Name = ParseValue(ref currentActor, "Name: ", GlobalLineTerminator);
                TrimCurrentLine(ref currentActor);
                actor.ShortDescription = ParseValue(ref currentActor, "Short Description: ", GlobalLineTerminator);
                TrimCurrentLine(ref currentActor);
                actor.LongDescription = ParseValue(ref currentActor, "Long Description: ", GlobalLineTerminator);
                TrimCurrentLine(ref currentActor);
                actor.Description = ParseValue(ref currentActor, "Description: ", GlobalLineTerminator);
                TrimCurrentLine(ref currentActor);
                actor.Level = int.Parse(ParseValue(ref currentActor, "Level: ", GlobalLineTerminator));
                TrimCurrentLine(ref currentActor);
                actor.CurrentExp = int.Parse(ParseValue(ref currentActor, "Current Exp: ", GlobalLineTerminator));
                TrimCurrentLine(ref currentActor);
                int HP = int.Parse(ParseValue(ref currentActor, "Max HP: ", GlobalLineTerminator));
                actor.CurrentHP = HP;
                actor.MaxHP = HP;
                TrimCurrentLine(ref currentActor);
                int MP = int.Parse(ParseValue(ref currentActor, "Max MP: ", GlobalLineTerminator));
                actor.CurrentMP = MP;
                actor.MaxMP = MP;
                TrimCurrentLine(ref currentActor);
                actor.Strength = int.Parse(ParseValue(ref currentActor, "Strength: ", GlobalLineTerminator));
                TrimCurrentLine(ref currentActor);
                actor.Dexterity = int.Parse(ParseValue(ref currentActor, "Dexterity: ", GlobalLineTerminator));
                TrimCurrentLine(ref currentActor);
                actor.Constitution = int.Parse(ParseValue(ref currentActor, "Constitution: ", GlobalLineTerminator));
                TrimCurrentLine(ref currentActor);
                actor.Intelligence = int.Parse(ParseValue(ref currentActor, "Intelligence: ", GlobalLineTerminator));
                TrimCurrentLine(ref currentActor);
                actor.Wisdom = int.Parse(ParseValue(ref currentActor, "Wisdom: ", GlobalLineTerminator));
                TrimCurrentLine(ref currentActor);
                actor.Charisma = int.Parse(ParseValue(ref currentActor, "Charisma: ", GlobalLineTerminator));
                TrimCurrentLine(ref currentActor);
                actor.Gold = int.Parse(ParseValue(ref currentActor, "Gold: ", GlobalLineTerminator));
                TrimCurrentLine(ref currentActor);
                area.Actors[actor.ID] = actor;
                Actor.SpawnActor(actor, areas[area.ID].Rooms[1]);

                actorList = currentActor;
            }
        }

        public static void ParseExits(Room room, string roomData)
        {
            while (roomData.IndexOf("--EXIT--") != -1)
            {
                string customName = "Somewhere";
                string terminator = "\n";
                roomData = roomData.Substring(roomData.IndexOf("--EXIT--"));
                roomData = roomData.Substring(roomData.IndexOf(terminator) +  1);
                string exitDirection = ParseValue(ref roomData, "Direction: ", terminator).Trim();
                roomData = roomData.Substring(roomData.IndexOf(terminator) + 1);
                int areaID = int.Parse(ParseValue(ref roomData, "Connected Area ID: ", terminator));
                roomData = roomData.Substring(roomData.IndexOf(terminator) + 1);
                int roomID = int.Parse(ParseValue(ref roomData, "Connected Room ID: ", terminator));
                roomData = roomData.Substring(roomData.IndexOf(terminator) + 1);
                if (roomData.IndexOf("Custom Name") != -1)
                {
                    customName = ParseValue(ref roomData, "Custom Name: ", terminator).Trim();
                }
                room.AddExit(exitDirection, Game.areas[areaID].Rooms[roomID]);
                room.Exits[room.Exits.Count - 1].Direction = customName;
            }
        }

        public static WorldObject ParseStandardObject(ref string objectData, string terminator)
        {
            WorldObject obj = new WorldObject();
            int id = ParseID(ref objectData);
            string[] keywords = ParseValue(ref objectData, "Keywords: ", terminator).Split(',');
            TrimCurrentLine(ref objectData);
            obj.Keywords = keywords;
            string shortDescription = ParseValue(ref objectData, "Short Description: ", terminator);
            TrimCurrentLine(ref objectData);
            string longDescription = ParseValue(ref objectData, "Long Description: ", terminator);
            TrimCurrentLine(ref objectData);
            string description = ParseValue(ref objectData, "Description: ", terminator);
            TrimCurrentLine(ref objectData);
            float weight = float.Parse(ParseValue(ref objectData, "Weight: ", terminator));
            TrimCurrentLine(ref objectData);

            obj.ID = id;
            obj.ShortDescription = shortDescription;
            obj.LongDescription = longDescription;
            obj.Description = description;
            obj.Weight = weight;
            return obj;
        }

        public static Container ParseContainer(WorldObject obj, ref string objectData, string terminator)
        {
            Container container = new Container();
            container.ID = obj.ID;
            container.Keywords = obj.Keywords;
            container.ShortDescription = obj.ShortDescription;
            container.LongDescription = obj.LongDescription;
            container.Description = obj.Description;
            container.WearLocations = obj.WearLocations;
            container.ObjectFlags = obj.ObjectFlags;
            container.Weight = obj.Weight;
            container.MaxWeight = float.Parse(ParseValue(ref objectData, "Max Weight: ", terminator));
            TrimCurrentLine(ref objectData);
            container.IsClosable = bool.Parse(ParseValue(ref objectData, "Is Closable: ", terminator));
            TrimCurrentLine(ref objectData);
            container.IsClosed = bool.Parse(ParseValue(ref objectData, "Is Closed: ", terminator));
            TrimCurrentLine(ref objectData);
            container.IsLockable = bool.Parse(ParseValue(ref objectData, "Is Lockable: ", terminator));
            TrimCurrentLine(ref objectData);
            container.IsLocked = bool.Parse(ParseValue(ref objectData, "Is Locked: ", terminator));
            TrimCurrentLine(ref objectData);


            return container;
        }

        public static void ParseObjects(Area area, ref string objectData)
        {
            objectData = objectData[objectData.IndexOf("**OBJECT_LIST**")..];
            string objectInitializer = "--OBJECT--";
            string terminator = "\n";
            string objectTerminator = "--END_OBJECT--\r\n";

            while (objectData.IndexOf(objectInitializer) != -1)
            {
                int startIndex = objectData.IndexOf(objectInitializer);
                int endIndex = objectData.IndexOf(objectTerminator) + objectTerminator.Length;
                string currentObject = objectData.Substring(startIndex, endIndex - startIndex);
                TrimCurrentLine(ref currentObject);
                string objectType = ParseValue(ref currentObject, "Object Type: ", terminator);
                TrimCurrentLine(ref currentObject);

                WorldObject obj = ParseStandardObject(ref currentObject, terminator);
                // Set object to be wearable on the assigned locations
                string[] wearLocations = ParseValue(ref currentObject, "Wear Locations: ", terminator).Split(",");
                var possibleLocations = Enum.GetValues(typeof(WorldObject.WearLocation));
                if (wearLocations[0] != "NONE")
                {
                    foreach(var wornLocation in wearLocations)
                    {
                        foreach (WorldObject.WearLocation location in possibleLocations)
                        {
                            if (wornLocation == location.ToString())
                            {
                                obj.WearLocations.Add(location);
                            }
                        }
                    }
                }
                TrimCurrentLine(ref currentObject);
                string[] objectFlags = ParseValue(ref currentObject, "Object Flags: ", terminator).Split(',');
                ParseObjectFlags(obj, objectFlags);
                TrimCurrentLine(ref currentObject);

                if (objectType == "container")
                {
                    Container container = ParseContainer(obj, ref currentObject, terminator);
                    area.Objects[container.ID] = container;
                    container.ObjectToRoom(area.Rooms[0]);
                    area.Objects[container.ID] = container;
                } else
                {
                    area.Objects[obj.ID] = obj;

                }

                objectData = objectData.Substring(endIndex);

            }
        }

        public static void ParseAreaFile(string areaFile)
        {
            string areaData;
            try
            {
                areaData = File.ReadAllText(areaFile);
            }
            catch
            {
                Console.WriteLine("Can't find specified file or directory.");
                return;
            }
            Area area = new Area();
            Game.areas.Add(area);
            area.ID = ParseID(ref areaData);
            area.Filename = ParseValue(ref areaData, "Filename: ", "\n");
            TrimCurrentLine(ref areaData);
            area.Name = ParseName(ref areaData);
            areaData = Area.TrimAreaData(areaData, "\n");
            area.Description = ParseDescription(ref areaData);
            areaData = Area.TrimAreaData(areaData, "\n");
            area.RoomCount = ParseEntityCount(areaData, "Room");
            areaData = Area.TrimAreaData(areaData, "\n");
            area.ObjectCount = ParseEntityCount(areaData, "Object");
            areaData = Area.TrimAreaData(areaData, "\n");
            area.ActorCount = ParseEntityCount(areaData, "Actor");
            areaData = Area.TrimAreaData(areaData, "\n");
            area.InitializeEntities();
        }

        public static void SpawnObject(Actor creator, string args)
        {
            string[] splitArgs = args.Split(' ');
            if (splitArgs.Length < 2)
            {
                Console.WriteLine("Usage: oinvoke [areaID] [itemID]");
                Console.WriteLine("Ex: oinvoke 2 0");
                return;
            }
            int areaID = int.Parse(splitArgs[0]);
            int itemID = int.Parse(splitArgs[1]);

            WorldObject obj = areas[areaID].Objects[itemID];
            obj.ObjectToActor(creator);
            Console.WriteLine($"You reach into the ether and pull out {obj.ShortDescription}!");
        }

        public static void DisplayAreaList()
        {
            foreach (var area in areas) 
            {
                Console.WriteLine($"{area.ID} - {area.Name}");
            }
        }

        public static void PrintColoredText(string text, ConsoleColor foregroundColor, bool newLine, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            ConsoleColor defaultForegroundColor = ConsoleColor.Gray;
            ConsoleColor defaultBackgroundColor = ConsoleColor.Black;

            if (newLine)
            {
                Console.WriteLine(text);
            } else
            {
                Console.Write(text);
            }

            Console.ForegroundColor = defaultForegroundColor;
            Console.BackgroundColor = defaultBackgroundColor;
        }

        public static void ParseObjectFlags(WorldObject obj, string[] flags)
        {
            foreach(var flag in flags)
            {
                string[] flagData= flag.Split(':');
                string keyVal = flagData[0].Substring(1, flagData[0].Length - 2);
                _ = bool.TryParse(flagData[1], out bool flagValue);
                obj.ObjectFlags.TryAdd(keyVal, flagValue);
            }
        }

        public static void PrintCommaSeparatedList(dynamic list)
        {
            if (list.Count == 0)
            {
                Console.WriteLine();
                return;
            }
            for (int i = 0; i <  list.Count; i++)
            {
                if (i < list.Count - 1)
                {
                    Console.Write($"{list[i]}, ");
                }
                else
                {
                    Console.WriteLine(list[i]);
                }
            }
        }

        public static void StartCombat(Actor attacker, Actor target)
        {
            attacker.InCombat = true;
            target.InCombat = true;
            attacker.Opponents.Add(target);
            target.Opponents.Add(attacker);
            ConsoleColor deathColor = ConsoleColor.Red;

            while (attacker.CurrentHP >= 0 && target.CurrentHP >= 0)
            {
                PerformCombatRound(attacker, target);
                Thread.Sleep(3000);
            }

            // At this point, someone's HP is at or below zero. Check to see who died
            if ((attacker.CurrentHP <= 0 && attacker.IsPlayer) ||
                (target.CurrentHP <=0 && target.IsPlayer))
            {
                PrintColoredText("YOU ARE DEAD!", deathColor, true);
                isPlaying = false;
                return;
            }
            else if (attacker.CurrentHP <= 0)
            {
                PrintColoredText($"{UniversalPadding}{attacker.ShortDescription} is DEAD!", deathColor, true);
                Console.WriteLine($"You gain {attacker.CurrentExp}EXP and {attacker.Gold} Gold!{GlobalLineTerminator}");
                attacker.CurrentRoom.actorsInRoom.Remove(attacker);
                target.InCombat = false;
                target.CurrentExp += attacker.CurrentExp;
                target.Gold += attacker.Gold;
                attacker.CurrentHP = attacker.MaxHP; // TODO: Figure out how to spawn object clones instead of referencing base object
            } else
            {
                PrintColoredText($"{UniversalPadding}{target.ShortDescription} is DEAD!", deathColor, true);
                Console.WriteLine($"You gain {target.CurrentExp}EXP and {target.Gold} Gold!{GlobalLineTerminator}");
                target.CurrentRoom.actorsInRoom.Remove(target);
                attacker.InCombat = false;
                attacker.CurrentExp += target.CurrentExp;
                attacker.Gold += target.Gold;
                target.CurrentHP = target.MaxHP;
            }
        }

        /// <summary>
        /// Rolls the given number of dice with the given side counts, adds any supplied modifier and returns the result
        /// </summary>
        /// <param name="numDie">The number of dice to roll.</param>
        /// <param name="numSides">The number of sides the dice will have.</param>
        /// <param name="modifier">A number to add (or subtract if negative) to the result of the dice rolls.</param>
        /// <returns>The calculated dice roll result.</returns>
        public static int RollDice(int numDie, int numSides, int modifier = 0)
        {
            int result = 0;

            for (int i = 0; i < numDie; i++)
            {
                result += diceRoller.Next(1, numSides);
            }

            return result + modifier;
        }

        public static void PerformCombatRound(Actor attacker, Actor target)
        {
            int attackerDamage = 0;
            int targetDamage = 0;

            attackerDamage += RollDice(2, 6);
            Thread.Sleep(100);
            targetDamage += RollDice(2, 6);

            Console.WriteLine($"You hit {target.ShortDescription} for {attackerDamage} points of damage!");
            Console.WriteLine($"{target.ShortDescription} hits you for {targetDamage} points of damage!");

            target.CurrentHP -= attackerDamage;
            attacker.CurrentHP -= targetDamage;

            Game.DisplayPrompt();
        }
    }
}
