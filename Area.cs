using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    internal class Area
    {
        private int _id;

        public int ID
        {   
            get { return _id; }
            set { _id = value; }
        }

        public string Name { get; set; }
        public string Description { get; set; }

        // The number of each entity type belonging to the area
        public int RoomCount { get; set; }
        public int ObjectCount { get; set; }
        public int ActorCount { get; set; }

        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<WorldObject> Objects { get; set; } = new List<WorldObject>();

        public Area() {
            ID = 0;
            Name = "A Brand New Area";
            Description = "This is a new area. In fact, it still has that 'new area' smell!";
            RoomCount = 1;
            ActorCount = 0;
            ObjectCount = 0;
        }

        public void DisplayArea()
        {
            Console.WriteLine("ID: " + ID);
            Console.WriteLine("Name: " + Name);
            Console.WriteLine("Description: " + Description);
            Console.WriteLine("---------");
            Console.WriteLine("Rooms:");
            foreach(Room room in Rooms)
            {
                Console.WriteLine(room.ID.ToString().PadLeft(5) + ": " + room.Name);
            }
            Console.WriteLine("");
            Console.WriteLine("Objects:");
            foreach (WorldObject obj in Objects)
            {
                Console.WriteLine(obj.ID.ToString().PadLeft(5) + ": " + obj.LongDescription);
            }
            Console.WriteLine("");
        }

        public static string TrimAreaData(string areaData, string terminator)
        {
            return areaData.Substring(areaData.IndexOf(terminator) + terminator.Length);
        }

        public void InitializeEntities()
        {
            for (int i = 0;  i < RoomCount; i++)
            {
                Room room = new Room();
                room.AreaID = ID;
                Rooms.Add(room);
            }
            for (int i = 0;  i < ObjectCount; i++)
            {
                WorldObject obj = new WorldObject();
                Objects.Add(obj);
            }
            for (int i = 0;  i < ActorCount; i++)
            {
                Actor actor = new Actor();
                Actors.Add(actor);
            }

        }
    }
}
