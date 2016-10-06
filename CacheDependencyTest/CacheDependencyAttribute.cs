using System;


namespace CacheDependencyTest
{
    public enum Direction { Parent, TwoWay }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CacheDependencyAttribute : Attribute
    {


        public Direction Direction { get; set; }

        public CacheDependencyAttribute(Direction direction)
        {
            Direction = direction;
        }
    }
}
