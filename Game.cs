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

        public static Actor Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public static readonly List<Area> areas = new();

        private static void QuitGame()
        {
            isPlaying = false;
        }


        public static void ProcessCommand(string command)
        {
            string[] splitCommand = command.ToLower().Split(' ');
            int targetIndex;
            string args = command[(command.IndexOf(' ') + 1)..];
            switch (splitCommand[0]) {
                case "quit":
                    QuitGame();
                    break;

                case "look":
                    Player.DoLook(args.ToLower());
                    break;

                case "ne":
                case "northeast":
                    Player.MoveActor("Northeast");
                    break;

                case "n":
                case "north":
                    Player.MoveActor("North");
                    break;

                case "s":
                case "south":
                    Player.MoveActor("South");
                    break;

                case "se":
                case "southeast":
                    Player.MoveActor("Southeast");
                    break;

                case "sw":
                case "southwest":
                    Player.MoveActor("Southwest");
                    break;

                case "e":
                case "east":
                    Player.MoveActor("East");
                    break;

                case "w":
                case "west":
                    Player.MoveActor("West");
                    break;

                case "u":
                case "up":
                    Player.MoveActor("Up");
                    break;

                case "d":
                case "down":
                    Player.MoveActor("Down");
                    break;

                case "astat":
                    areas[Player.CurrentRoom.AreaID].DisplayArea();
                    break;

                case "inv":
                case "inventory":
                case "i":
                    Player.ShowInventory();
                    break;

                case "eq":
                    Player.ShowEquipment();
                    break;

                case "get":
                    Player.DoGet(args.ToLower());
                    break;

                case "drop":
                    Player.DoDrop(args.ToLower());
                    break;

                case "wear":
                    if (splitCommand.Length == 1)
                    {
                        Console.WriteLine("Wear WHAT?!");
                        break;
                    }
                    targetIndex = Player.IsCarrying(splitCommand[1]);
                    var obj = Player.Inventory[targetIndex];
                    if (obj != null && obj.WearLocations.Count > 0)
                    {
                        foreach (var loc in obj.WearLocations)
                        {
                            if (loc.ToString() == "HELD")
                            {
                                if (obj.WearLocations.Contains(WorldObject.WearLocation.WIELD_DUAL))
                                {
                                    if (Player.Equipment[WorldObject.WearLocation.WIELD_R] != null ||
                                        Player.Equipment[WorldObject.WearLocation.WIELD_L] != null) {
                                        Console.WriteLine("You need both hands free to wield that!");
                                        return;
                                    }
                                }
                                if (Player.Equipment[WorldObject.WearLocation.WIELD_R] == null)
                                {
                                    Player.Equipment[WorldObject.WearLocation.WIELD_R] = obj;
                                    Console.WriteLine($"You wield {obj.ShortDescription} in your right hand.");
                                    obj.ObjectFromActor(Player);
                                    return;
                                } else if (Player.Equipment[WorldObject.WearLocation.WIELD_L] == null)
                                {
                                    Player.Equipment[WorldObject.WearLocation.WIELD_L] = obj;
                                    Console.WriteLine($"You wield {obj.ShortDescription} in your left hand.");
                                    obj.ObjectFromActor(Player);
                                    return;
                                } else
                                {
                                    Console.WriteLine("Your hands are too full to hold anything else!");
                                    return;
                                }
                            } else
                            {
                                if (Player.Equipment[loc] == null)
                                {
                                    Player.Equipment[loc] = obj;
                                    obj.ObjectFromActor (Player);
                                    Console.WriteLine($"You wear {obj.ShortDescription} on your {loc.ToString().ToLower()}.");
                                    return;
                                } else if (Player.Equipment[loc] != null && obj.WearLocations.Count == 1)
                                {
                                    Console.WriteLine($"You're already wearing something on your {loc.ToString().ToLower()}! You might wanna take that off first.");
                                    return;
                                } else
                                {
                                    continue;
                                }
                            }
                        }
                    } else if (obj != null && obj.WearLocations.Count == 0)
                    {
                        Console.WriteLine("You can't figure out how to wear that.");
                    } else
                    {
                        Console.WriteLine("You're not carrying that.");
                    }
                    break;

                case "remove":
                case "rem":
                    Player.DoRemove(args.ToLower());
                    break;

                case "alist":
                    DisplayAreaList();
                    break;

                case "ostat":
                    Actor.DoOstat(args.ToLower());
                    break;

                case "exa":
                case "examine":
                    Player.DoExamine(args.ToLower());
                    break;

                case "say":
                    Player.DoSay(args);
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
                        targetIndex = Player.IsCarrying(itemToStore);
                        int containerIndex;
                        Container container;

                        // If player isn't carrying target item
                        if (targetIndex == -1)
                        {
                            Console.WriteLine("You aren't carrying that.");
                        } else if (targetIndex != -1) // If player IS carrying target item
                        {
                            // Check if player is carrying target container
                            containerIndex = Player.IsCarrying(targetContainer);
                            if (containerIndex == -1)
                            {
                                // If player isn't carrying target container, check to see if
                                // the container is an object in the room
                                container = (Container)Player.CurrentRoom.ObjectInRoom(targetContainer);
                                if (container == null)
                                {
                                    Console.WriteLine("There's nothing like that here.");
                                    return;
                                } else if (!Container.IsContainer(container))
                                {
                                    Console.WriteLine("That's not a container.");
                                    return;
                                } else
                                {
                                    container.ObjectToContainer(Player.Inventory[targetIndex]);
                                }
                            } 

                            else
                            {
                                // Player is carrying both target item and target container
                                if (itemToStore == targetContainer)
                                {
                                    // Prevent player from putting a container inside itself.
                                    Console.WriteLine("You can't put containers in themselves! Are you TRYING to destroy the universe?!");
                                    return;
                                }
                                else if (!Container.IsContainer(Player.Inventory[containerIndex]))
                                {
                                    Console.WriteLine("That's not a container.");
                                    return;
                                }
                                {
                                    // Player has both items and isn't trying to create a paradox
                                    // Store item in container and stored item from player's inventory
                                    container = (Container)Player.Inventory[containerIndex];
                                    container.ObjectToContainer(Player.Inventory[targetIndex]);
                                }
                            }
                        }
                    }
                    break;

                case "oinvoke":
                    SpawnObject(Player, args.ToLower());
                    break;

                default:
                    Console.WriteLine("WHAT?!");
                    break;
            }
        }


        public static void DisplayPrompt()
        {
            Console.WriteLine($"\nHP: {Player.CurrentHP}/{Player.MaxHP} MP:{Player.CurrentMP}/{Player.MaxMP} Gold: {Player.Gold}");
            Console.Write("> ");
        }

        // TODO: Most of this is hardcoded game data and needs to be removed once full file parsing
        // functionality is implemented.
        public static void InitializeGame()
        {
            Area area = new Area();
            areas.Add(area);
            Room room0 = new Room();
            Room room1 = new Room();
            Room room2 = new Room();
            Room room3 = new Room();
            Room room4 = new Room();
            Room room5 = new Room();

            room0.AddRoomToArea(area);
            room1.AddRoomToArea(area);
            room2.AddRoomToArea(area);
            room3.AddRoomToArea(area);
            room4.AddRoomToArea(area);
            room5.AddRoomToArea(area);
            area.RoomCount += 6;

            area.Name = "The First Area";
            area.Description = "The area which was created first. That's it. There's nothing special to the name besides that.";

            room1.Name = "The First Room";
            room1.Description = "The hallowed halls of 'The First Room,' the room which was created first. Blessed be the walls of this sanctuary. Blessed indeed.";
            room1.AddExit("South", room0);
            room1.AddExit("Northeast", room2);
            
            room0.Name = "An Endless Void";
            room0.Description = "A nebulous void of nothingness. Dark clouds go on endlessly in all directions. To the north is a doorway with blinding light spilling forth.";
            room0.AddExit("North", room1);

            room2.Name = "Gates of Mulforth Citadel";
            room2.Description = "The entry arch of Mulforth Citadel has one iron gate that's still partially attached by its hinges. The other gate lies ruined just inside the citadel proper.";
            room2.AddExit("Southwest", room1);
            room2.AddExit("North", room3);

            room3.Name = "Citadel Training Grounds";
            room3.Description = "People trained here. That's why it's called 'training' grounds.";
            room3.AddExit("South", room2);
            room3.AddExit("North", room4);

            room4.Name = "Mess Hall";
            room4.Description = "Ugh, this hall sure is a mess!";
            room4.AddExit("South", room3);
            room4.AddExit("West", room5);

            room5.Name = "Kitchen";
            room5.Description = "A long counter nearly 20 feet in length runs along the center of the room. Around the walls are a half-dozen ovens. Rusty pots, pans and other utensils are strewn about the area. Decaying foodstuffs fill the room with a powerful stench.";
            room5.AddExit("East", room4);

            Player = new Actor
            {
                CurrentRoom = room0,
                IsPlayer = true,
                Description = "This adventurer has an aura of such daring that it's illegal in three countries! (Seriously, they're wanted in two of those already!)"
            };

            Actor mob1 = new()
            {
                CurrentRoom = room3,
                Name = "skeletal soldier",
                ShortDescription = "a skeletal soldier",
                LongDescription = "A skeletal soldier is wandering the grounds.",
                Description = "The walking corpse of a long dead soldier. No flesh remains attached to its bones, and the armor it wears has been ravaged by time. The insignia of an enemy kingdom is faintly visible on the breastplate.",
                Level = 1,

            };
            mob1.MoveActor(room3);

            Actor mob2 = new()
            {
                Name = "a zombie cook",
                ShortDescription = "a zombie cook",
                LongDescription = "A zombie in a tattered apron and trousers is shuffling about.",
                Description = "You see nothing special about it. Nope. Not a thing. Absolutely nothing. It's the most UN-special zombie ever to exist. Zombies are just part of every day life. You should already know what a zombie looks like, therefore it's pointless to describe it any further.\n\nHmm? What's that? Is it a man or a woman? Human? Elf? Dwarf? Look, it's a zombie. I already said you see NOTHING SPECIAL ABOUT IT. Stop asking questions! Geez! You'd think you want details in your games or something. Dang millenials...",
                Level = 2,
                CurrentHP = 100,
                MaxHP = 100,
                Gold = 10,
                CurrentExp = 250
            };
            mob2.MoveActor(room5);

            WorldObject obj1 = new WorldObject();
            WorldObject obj2 = new WorldObject()
            {
                ID = 1,
                LongDescription = "A huge butcher's cleaver has been dropped here.",
                Keywords = new string[] { "butcher", "cleaver" },
                ShortDescription = "a huge butcher's cleaver",
                Weight = 5f,
            };

            obj2.WearLocations.Add(WorldObject.WearLocation.HELD);
            obj2.ObjectToRoom(room5);
            obj2.ObjectFlags["canTake"] = true;
            obj1.ObjectToRoom(room2);
            LoadAreas();
            room1.AddExit("West", Game.areas[1].Rooms[0]);
            room0.AddExit("West", Game.areas[2].Rooms[0]);

            Container obj3 = new Container() { MaxWeight = 50f };
            obj3.ShortDescription = "a teddy bear";
            obj3.LongDescription = "A big teddy bear is lying on the ground.";
            obj3.Description = "A huge teddy bear with big, brown eyes. There is a zipper on its back, and the stuffing has been removed.";
            obj3.ObjectFlags["canTake"] = true;
            obj3.WearLocations.Add(WorldObject.WearLocation.HELD);
            obj3.MaxWeight = 15f;
            area.Objects.Add(obj3);
            obj3.ObjectToActor(Player);

            if (Player.IsCarrying("bear", out WorldObject @object))
            {

                Console.WriteLine($"You're carrying {@object.ShortDescription}, which is a {@object.GetType()}!");
            }

        }

        public static void Play()
        {
            string? userInput;
            Player.CurrentRoom.DisplayRoom();

            while (Game.isPlaying)
            {
                DisplayPrompt();
                userInput = Console.ReadLine();
                Game.ProcessCommand(userInput);
            }
        }

        public static void LoadAreas()
        {
            var fileList = Directory.EnumerateFiles(@"D:\Creative Stuff\Programming Projects\CSharp\TextAdventure\Areas\");
            foreach (var file in fileList)
            {
                ParseAreaFile(file);
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
                // TODO: Remove the following line once object flags can be parsed
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

                obj.ObjectToRoom(area.Rooms[1]);
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
            area.Filename = ParseValue(ref areaData, "Filename", "\n");
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
            ParseObjects(area, ref areaData);
            ParseRooms(area, ref areaData);
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
    }
}
