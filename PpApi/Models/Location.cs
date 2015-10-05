using System;

namespace PpApi.Models
{
    public class Location
    {
        public int Id { set; get; }
        public long Timestamp { set; get; }
        public float X { set; get; }
        public float Y { set; get; }
        public string Name { set; get; }
        public bool Waypoint { set; get; }
    }
}
