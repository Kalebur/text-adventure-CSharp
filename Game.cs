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

        public static Actor Player
        {
            get { return _player; }
            set { _player = value; }
        }

        private static readonly List<Area> areas = new();


        public static void ProcessCommand(string command)
        {
            string[] splitCommand = command.ToLower().Split(' ');
            WorldObject targetObject;
            int targetIndex = -1;
            switch (splitCommand[0]) {
                case "quit":
                    isPlaying = false;
                    break;

                case "look":
                    if (splitCommand.Length == 1)
                    {
                        Player.CurrentRoom.DisplayRoom();
                    } else if (splitCommand[1] == "self")
                    {
                        Console.WriteLine("You look yourself over.");
                        Console.WriteLine(Player.Description);
                        Player.ShowEquipment();
                    } else
                    {
                        foreach(Actor actor in Player.CurrentRoom.actorsInRoom)
                        {
                            if (actor.Name.Contains(splitCommand[1]) || actor.ShortDescription.Contains(splitCommand[1]))
                            {
                                Console.WriteLine(actor.Description);
                                actor.ShowEquipment();
                                return;
                            }
                        }
                        Console.WriteLine("You don't see anyone like that here.");
                    }
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
                    areas[(int)Player.CurrentRoom.AreaID].DisplayArea();
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
                    targetObject = Player.CurrentRoom.ObjectInRoom(splitCommand[1]);

                    if (targetObject != null && targetObject.ObjectFlags["canTake"])
                    {
                        targetObject.ObjectToActor(Player);
                        targetObject.ObjectFromRoom(Player.CurrentRoom);
                        Console.WriteLine($"You get {targetObject.ShortDescription}.");
                    } else if (targetObject != null && !targetObject.ObjectFlags["canTake"])
                    {
                        Console.WriteLine("Sorry, you can't take that. Not EVERYTHING is just free for the taking, you know! Ugh, greedy adventurers...");
                    } else
                    {
                        Console.WriteLine("You don't see anything like that here.");
                    }
                    break;

                case "drop":
                    targetIndex = Player.IsCarrying(splitCommand[1]);
                    if (targetIndex != -1)
                    {
                        Player.Inventory[targetIndex].ObjectToRoom(Player.CurrentRoom);
                        Console.WriteLine($"You drop {Player.Inventory[targetIndex].ShortDescription}.");
                        Player.Inventory[targetIndex].ObjectFromActor(Player);
                    } else
                    {
                        Console.WriteLine("You aren't carrying that.");
                    }
                    break;

                case "wear":
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
                    string objToRemove = splitCommand[1];
                    foreach(var wornItem in Player.Equipment)
                    {
                        if (wornItem.Value != null && 
                            (wornItem.Value.Keywords.Contains(objToRemove) || wornItem.Value.ShortDescription.Contains(objToRemove)))
                        {
                            wornItem.Value.ObjectToActor(Player);
                            Player.Equipment[wornItem.Key] = null;
                            Console.WriteLine($"You remove {wornItem.Value.ShortDescription}.");
                            return;
                        }
                    }
                    Console.WriteLine("You're not wearing that.");
                    break;

                case "alist":
                    foreach(Area area in Game.areas)
                    {
                        Console.WriteLine($"{area.ID} - {area.Name}");
                    }
                    break;

                case "ostat":
                    if (splitCommand.Length == 1)
                    {
                        Console.WriteLine("Get stats for which object?");
                        Console.WriteLine("Usage: ostat AREA_ID OBJECT_ID");
                        Console.WriteLine("Ex: ostat 0 2");
                        return;
                    }
                    targetObject = Game.areas[int.Parse(splitCommand[1])].Objects[int.Parse(splitCommand[2])];
                    Console.Write("Can be worn on: ");
                    foreach (var loc in targetObject.WearLocations)
                    {
                        Console.Write($"{loc.ToString()},");
                    }
                    break;

                case "exa":
                case "examine":
                    {
                        Player.ExamineObject(splitCommand[1]);
                        break;
                    }

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
                        int containerIndex = -1;
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
                                else
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

                default:
                    Console.WriteLine("WHAT?!");
                    break;
            }
        }


        public static void DisplayPrompt()
        {
            Console.WriteLine($"\nHP: {Player.CurrentHP}/{Player.MaxHP} Gold: {Player.Gold}");
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
            obj3.Description = "A huge teddy bear with big, brown eyes. There is a zipper on its back, and the stuffing has been removed.";
            obj3.ObjectFlags["canTake"] = true;
            obj3.WearLocations.Add(WorldObject.WearLocation.HELD);
            obj3.MaxWeight = 15f;
            obj3.ObjectToActor(Player);

        }

        public static void Play()
        {
            string? userInput;
            Player.CurrentRoom.DisplayRoom();

            while (Game.isPlaying)
            {
                DisplayPrompt();
                userInput = Console.ReadLine().ToLower();
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
            stringToTrim = stringToTrim.Substring(stringToTrim.IndexOf("\n") + 1);
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
            areaData = areaData.Substring(areaData.IndexOf("\n") + 1);
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

        public static Area ParseRooms(Area area, ref string areaData)
        {
            for (int i = 0; i < area.Rooms.Count; i++)
            {
            ParseRoom(area, ref areaData);
            }
            return area;
        }

        public static void ParseRoom(Area area, ref string areaData)
        {
            string dividerText = "--ROOM--";
            string terminator = "--END_ROOM--";
            string trimmedData = areaData.Substring(areaData.IndexOf(dividerText));
            trimmedData = trimmedData.Substring(trimmedData.IndexOf("\n") + 1);
            int dataSize = trimmedData.IndexOf(terminator) + terminator.Length;
            string roomData = trimmedData.Substring(0, dataSize);

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

        public static void ParseObjects(Area area, ref string objectData)
        {
            objectData = objectData.Substring(objectData.IndexOf("**OBJECT_LIST**"));
            string objectInitializer = "--OBJECT--";
            string terminator = "\n";
            string objectTerminator = "--END_OBJECT--\r\n";

            while (objectData.IndexOf(objectInitializer) != -1)
            {
                WorldObject obj = new WorldObject();
                int startIndex = objectData.IndexOf(objectInitializer);
                int endIndex = objectData.IndexOf(objectTerminator) + objectTerminator.Length;
                string currentObject = objectData.Substring(startIndex, endIndex - startIndex);
                TrimCurrentLine(ref currentObject);
                int id = ParseID(ref currentObject);
                area.Objects[id].ID = id;
                string[] keywords = ParseValue(ref currentObject, "Keywords: ", terminator).Split(',');
                obj.Keywords = keywords;
                TrimCurrentLine(ref currentObject);
                obj.ShortDescription = ParseValue(ref currentObject, "Short Description: ", terminator);
                TrimCurrentLine(ref currentObject);
                obj.LongDescription = ParseValue(ref currentObject, "Long Description: ", terminator);
                TrimCurrentLine(ref currentObject);
                obj.Description = ParseValue(ref currentObject, "Description: ", terminator);
                TrimCurrentLine(ref currentObject);
                obj.Weight = float.Parse(ParseValue(ref currentObject, "Weight: ", terminator));
                TrimCurrentLine(ref currentObject);

                // Set object to be wearable on the assigned locations
                string[] wearLocations = ParseValue(ref currentObject, "Wear Locations: ", terminator).Split(",");
                var possibleLocations = Enum.GetValues(typeof(WorldObject.WearLocation));
                WorldObject testObj = new WorldObject();
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

                area.Objects[id] = obj;
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
            _ = ParseRooms(area, ref areaData);
        }
    }
}
