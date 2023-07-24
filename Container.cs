using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    internal class Container : WorldObject
    {
        public List<WorldObject> ContainedItems { get; set; }
        private float _currentWeight;
        public float MaxWeight { get; set; }
        public bool IsClosable { get; set; }
        public bool IsClosed { get; set; }
        public bool IsLockable { get; set; }
        public bool IsLocked { get; set; }

        // Messages showing how a container is opened
        // By default, all closable containers can be "opened" or "closed"
        // This will let you change that to something like "unzip" or "zip up"
        // or anything else you like
        public string OpenAction { get; set; }
        public string CloseAction { get; set; }


        public Container() {
            ContainedItems = new List<WorldObject>();
            _currentWeight = 0f;
            MaxWeight = 50f;
            IsClosable = false;
            IsClosed = false;
            IsLockable = false;
            IsLocked = false;
            OpenAction = "open";
            CloseAction = "close";
        }

        public void ObjectToContainer(WorldObject item)
        {
            if (item.Weight + _currentWeight > MaxWeight)
            {
                Console.WriteLine($"There isn't enough room to put that in {ShortDescription}.");
                return;
            } else
            {
                _currentWeight += item.Weight;
                ContainedItems.Add(item);
                item.ObjectFromActor(Game.Player);
            }
        }

        public new void DisplayObjectInfo()
        {
            string padding = "".PadLeft(12);
            Console.WriteLine(Description);
            Console.WriteLine();
            if (IsClosed)
            {
                Console.WriteLine($"{ShortDescription} is closed.");
            }
            Console.WriteLine($"{ShortDescription} contains:");
            if (ContainedItems.Count == 0) {
                Console.WriteLine($"{padding}Nothing");
                return;
            } else
            {
                foreach ( WorldObject item in ContainedItems )
                {
                    Console.WriteLine($"{padding}{ShortDescription}");
                }
            }
        }
    }
}
