using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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

        private void PrintClosedMessage()
        {
            Console.WriteLine($"{ShortDescription[0].ToString().ToUpper() + ShortDescription[1..]} is closed.");
        }

        public void ObjectToContainer(WorldObject item)
        {
            if (item.Weight + _currentWeight > MaxWeight)
            {
                Console.WriteLine($"There isn't enough room to put that in {ShortDescription}.");
                return;
            } else if (IsClosed)
            {
                PrintClosedMessage();
                return;
            }
            {
                _currentWeight += item.Weight;
                ContainedItems.Add(item);
                item.ObjectFromActor(Game.Player);
                Console.WriteLine($"You put {item.ShortDescription} in {ShortDescription}.");
            }
        }

        public void ObjectFromContainer(string objToTake, string itemNotFoundMessage, Actor receiver)
        {
            if (IsClosed)
            {
                PrintClosedMessage();
                return;
            }
            if (ContainerHasItem(objToTake, out WorldObject item))
            {
                Console.WriteLine($"You take {item.ShortDescription} out of {ShortDescription}.");
                item.ObjectToActor(receiver);
                _currentWeight -= item.Weight;
                ContainedItems.Remove(item);
            }
            else
            {
                Console.WriteLine($"{itemNotFoundMessage}{ShortDescription}.");
            }
        }

        public bool ContainerHasItem(string targetItem, out WorldObject i)
        {
            foreach (var item in ContainedItems)
            {
                if (item.ShortDescription.ToLower().Contains(targetItem.ToLower() ))
                {
                    i = item;
                    return true;
                }
            }
            i = WorldObject.nullObject;
            return false;
        }

        public override void DisplayObjectInfo()
        {
            string padding = "".PadLeft(12);
            Console.WriteLine(Description);
            Console.WriteLine();
            if (IsClosed)
            {
                Console.WriteLine($"{ShortDescription} is closed.");
            } else
            {
                Console.WriteLine($"{ShortDescription} contains:");
                if (ContainedItems.Count == 0) {
                    Console.WriteLine($"{padding}Nothing");
                    return;
                } else
                {
                    foreach ( var item in ContainedItems )
                    {
                        Console.WriteLine($"{padding}{item.ShortDescription}");
                    }
                }
            }
        }
    }
}
