using System.Collections.Generic;

namespace CacheDependencyTest
{
    public class TestClassOne
    {
        [CacheDependency(Direction.Parent)]
        public Child Name { get; set; }

        [CacheDependency(Direction.Parent)]
        public string test { get; set; }
    }


    public class Child
    {
        [CacheDependency(Direction.TwoWay)]
        public Child Name1 { get; set; }
        [CacheDependency(Direction.TwoWay)]
        public Child2 Children { get; set; }
    }

    public class Child2
    {
        public Child2()
        {
            Children = new List<Child>();
        }
        public List<Child> Children;
    }
}
