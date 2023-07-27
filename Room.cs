using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    internal class Room
    {
        public readonly static Room nullRoom = new Room();

        public string Name { get; set; }
        public string Description { get; set; }

        public List<Actor> actorsInRoom;
        public List<WorldObject> objectsInRoom;
        public int AreaID { get; set; }

        public int ID { get; set; }


        //public Dictionary<string, RoomExit> Exits { get; } = new Dictionary<string, RoomExit>();
        public List<RoomExit> Exits { get; } = new List<RoomExit>();

        public void DisplayExits()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Exits:");
            Console.ForegroundColor = ConsoleColor.Gray;
            if (Exits.Count == 0) Console.WriteLine("None");
            foreach (var exit in Exits)
            {
                Console.WriteLine($"{exit.Direction} - {exit.ConnectedRoom.Name}");
            }
        }

        public void DisplayRoom()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('[');
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Name);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(']');
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(Description + "\n");
            DisplayExits();
            if (actorsInRoom.Count > 1)
            {
                DisplayActors();
            }
            if (objectsInRoom.Count > 0)
            {
                DisplayObjects();
            }
        }

        private void DisplayActors()
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (Actor actor in actorsInRoom)
            {
                if (actor.IsPlayer) continue;
                else Console.WriteLine(actor.LongDescription);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void DisplayObjects()
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            foreach(WorldObject obj in objectsInRoom)
            {
                Console.WriteLine(obj.LongDescription);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public Room()
        {
            Name = "A Boring, Empty Room";
            Description = "The room has four plain walls, a wooden floor, and a single light bulb hanging from a leaky ceiling.";
            actorsInRoom = new List<Actor>();
            objectsInRoom = new List<WorldObject>();
        }

        public void AddExit(string exitDirection, Room connectedRoom)
        {
            var exit = new RoomExit(exitDirection)
            {
                ConnectedRoom = connectedRoom
            };
            Exits.Add(exit);
        }

        public bool HasExit(string direction)
        {
            foreach(var exit in Exits)
            {
                if (exit.Direction == direction) return true;
            }
            return false;
        }

        public void AddRoomToArea(Area area)
        {
            area.Rooms.Add(this);
            this.AreaID = area.ID;
            this.ID = area.Rooms.Count - 1;
        }

        public RoomExit GetExit(string direction)
        {
            RoomExit ex = new RoomExit();
            foreach(var exit in Exits)
            {
                if (exit.Direction == direction) ex = exit;
            }
            return ex;
        }

        /// <summary>
        /// Checks to see if an object with a name that includes the param targetObj is in the room.
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns>The object if found, null if not found.</returns>
        public WorldObject ObjectInRoom(string targetObj)
        {
            foreach (var obj in objectsInRoom)
            {
                if (obj.ShortDescription.ToLower().Contains(targetObj) || obj.Keywords.Contains(targetObj)) return obj;
            }
            return WorldObject.nullObject;
        }

        public bool ObjectInRoom(WorldObject obj)
        {
            if (!objectsInRoom.Contains(obj)) return false;
            return true;
        }

    }
}
