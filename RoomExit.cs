using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventure
{
    internal class RoomExit
    {
        public enum ExitDirection { NORTH, SOUTH, WEST, EAST, UP, DOWN, NORTHEAST, SOUTHEAST, NORTHWEST, SOUTHWEST, CUSTOM }

        private readonly ExitDirection _direction;
        private string _directionName;

        public string Direction
        {
            get
            {
                if (_direction == ExitDirection.CUSTOM) { return _directionName; }
                return _direction.ToString()[0] + _direction.ToString()[1..].ToLower();
            }

            set
            {
                if (_direction == ExitDirection.CUSTOM) _directionName = value;
            }
        }

        public string Description { get; set; }
        public Room ConnectedRoom { get; set; }
        public int ConnectedAreaID { get; set; }
        public int ConnectedRoomID { get; set; }
        //public string DirectionName
        //{
        //    get { return _directionName; }
        //    set { _directionName = value; }
        //}
        public Dictionary<string, bool> ExitFlags { get; set; } = new Dictionary<string, bool>();

        public RoomExit()
        {
            _direction = ExitDirection.NORTH;
            _directionName = "Somewhere";
            Description = "the door";
            ConnectedRoom = new Room();

        }

        public RoomExit(string direction)
        {
            switch (direction.ToLower())
            {
                case "north":
                    _direction = ExitDirection.NORTH;
                    break;

                case "south":
                    _direction = ExitDirection.SOUTH;
                    break;

                case "west":
                    _direction = ExitDirection.WEST;
                    break;

                case "east":
                    _direction = ExitDirection.EAST;
                    break;

                case "up":
                    _direction = ExitDirection.UP;
                    break;

                case "down":
                    _direction = ExitDirection.DOWN;
                    break;

                case "northeast":
                    _direction = ExitDirection.NORTHEAST;
                    break;

                case "northwest":
                    _direction = ExitDirection.NORTHWEST;
                    break;

                case "southeast":
                    _direction = ExitDirection.SOUTHEAST;
                    break;

                case "southwest":
                    _direction = ExitDirection.SOUTHWEST;
                    break;

                case "custom":
                    _direction = ExitDirection.CUSTOM;
                    _directionName = "Somewhere";
                    break;

                default:
                    Console.WriteLine("Invalid exit direction. Setting direction to 'CUSTOM' and initializing.");
                    _direction = ExitDirection.CUSTOM;
                    _directionName = "Somewhere";
                    break;
            }

            _directionName = "Somewhere...";
            Description = _directionName;
            ConnectedRoom = new Room();
        }
    }
}
