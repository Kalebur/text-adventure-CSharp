﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    // Base class for weapons, armor, items, furniture, etc.
    internal class WorldObject
    {
        public enum WearLocation { HEAD, NECK, TORSO, LEGS, FEET, EARS, FINGER_L, FINGER_R, BACK, WAIST,
                                   ABOUT, OVER, HELD, WIELD_L, WIELD_R, WIELD_DUAL, WRIST_L, WRIST_R };
        public List<WearLocation> WearLocations { get; set; }

        public int ID { get; set; }
        private int _areaID;

        public int AreaID
        {
            get { return _areaID; }
            set { _areaID = value; }
        }

        public string[] Keywords { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Description { get; set; } // The description of the object itself.
        public float Weight { get; set; }
        public Dictionary<string, bool> ObjectFlags { get; set; } = new Dictionary<string, bool>();


        public WorldObject()
        {
            Keywords = new string[] { "woof", "meow", "bark" };
            ShortDescription = "a new Object";
            LongDescription = "Some deity abandoned a newly created Object here.";
            Description = "BEHOLD! It's an Object!";
            Weight = 2f;
            ObjectFlags.Add("canTake", false);
            WearLocations = new List<WearLocation>();
        }

        public void ObjectToRoom(Room room)
        {
            room.objectsInRoom.Add(this);
        }

        public void ObjectFromRoom(Room room)
        {
            room.objectsInRoom.Remove(this);
        }

        public void ObjectToActor(Actor actor)
        {
            actor.Inventory.Add(this);
        }

        public void ObjectFromActor(Actor actor)
        {
            actor.Inventory.Remove(this);
        }

        public void DisplayObjectInfo()
        {
            Console.WriteLine(Description);
        }


    }
}
