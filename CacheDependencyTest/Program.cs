using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CacheDependencyTest
{


    class Program
    {
        // private static Dictionary<Type, TableInfo> tableInfoList =
        //new Dictionary<Type, TableInfo>();

      //  private static Dictionary<Type, List<PropertyInfo>> _propertyDictionary = new Dictionary<Type, List<PropertyInfo>>();
        private static Dictionary<Type, PropertyInfo> _propertyDictionary = new Dictionary<Type, PropertyInfo>();
        static void Main(string[] args)
        {
            var foo = new TestClassOne();
            foo.Name = new Child();
            foo.test = "dfddd";

            foo.Name.Name1 = new Child();

            foo.Name.Children = new Child2();
            foo.Name.Children.Children = new List<Child> { new Child(), new Child()  };
            var itterations = 1;



            Stopwatch sw = Stopwatch.StartNew();
            
            for (var i = 0; i < itterations; i++)
            {
                //  ParseObject(foo);

                PrintPropertiesWithParent(foo, 1);
            }

            Console.WriteLine(sw.ElapsedMilliseconds);

            Console.ReadKey();
        }

        public static void Recursive(Type obj)
        {
            foreach (var prop in obj.GetProperties().Where(n => n.IsDefined(typeof(CacheDependencyAttribute), true)))
            {
                Console.WriteLine("prop:" + prop);
                Recursive(prop.GetType());



                var attrs = prop.GetCustomAttributes(true);

                foreach (object attr in attrs)
                {
                    var authAttr = attr as CacheDependencyAttribute;
                    if (authAttr != null)
                    {
                        string propName = prop.Name;
                        Direction auth = authAttr.Direction;

                        Console.WriteLine(propName);
                        Console.WriteLine(auth);
                    }
                }
            }
        }


        private static void ParseObject(object obj, Type parent = null)
        {
            if (obj == null) return;

            Type objType = obj.GetType();

           // Type objParent = null;

          //  if (parent != null)
            //    objParent = parent.GetType();

            var properties = objType.GetProperties().Where(n => n.IsDefined(typeof(CacheDependencyAttribute), true));
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);



                var elems = propValue as IList;
                if ((elems != null) && !(elems is string[]))
                {
                  //  object last = null;

                    foreach (var item in elems)
                    {
                        ParseObject(item, property.PropertyType);
                    }
                }
                else
                {
                    ParseObject(propValue, objType);
                }

                var attrs = property.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    var authAttr = attr as CacheDependencyAttribute;
                    if (authAttr != null)
                    {
                        string propName = property.Name;
                        Direction direction = authAttr.Direction;

                        switch (direction)
                        {
                            case Direction.Parent:
                                Console.WriteLine($"{propName} depends on {parent?.Name}");
                                break;
                            case Direction.TwoWay:
                                Console.WriteLine($"{propName} depends on {parent?.Name}.");
                                Console.WriteLine($"{parent?.Name} depends on {propName} .");
                                break;
                        }
                        Console.WriteLine("--");
                    }
                }
            }
        }


        private static void PrintProperties(object obj, int indent)
        {
            if (obj == null) return;
            string indentString = new string(' ', indent);
            Type objType = obj.GetType();
            var properties = objType.GetProperties().Where(n => n.IsDefined(typeof (CacheDependencyAttribute), true));
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);

                var elems = propValue as IList;
                if ((elems != null) && !(elems is string[]))
                {
                    foreach (var item in elems)
                    {
                        PrintProperties(item, indent + 3);
                    }
                }
                else
                {
                    // This will not cut-off System.Collections because of the first check
                    if (property.PropertyType.Assembly == objType.Assembly)
                    {
                        Console.WriteLine("{0}{1}:", indentString, property.Name);
                        PrintProperties(propValue, indent + 2);
                    }
                    else
                    {
                        if (propValue is string[])
                        {
                            var str = new StringBuilder();
                            foreach (string item in (string[])propValue)
                            {
                                str.AppendFormat("{0}; ", item);
                            }
                            propValue = str.ToString();
                            str.Clear();
                        }
                        Console.WriteLine("{0}{1}: {2}", indentString, property.Name, propValue);
                    }
                }
            }
        }

        private static void PrintPropertiesWithParent(object obj, int indent, object parent = null)
        {
            if (obj == null) return;
            string indentString = new string(' ', indent);
            Type objType = obj.GetType();
            var properties = objType.GetProperties().Where(n => n.IsDefined(typeof(CacheDependencyAttribute), true));
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);

                var elems = propValue as IList;
                if ((elems != null) && !(elems is string[]))
                {
                    foreach (var item in elems)
                    {
                        PrintPropertiesWithParent(item, indent + 3, obj);
                    }
                }
                else
                {
                    // This will not cut-off System.Collections because of the first check
                    if (property.PropertyType.Assembly == objType.Assembly)
                    {
                        Console.WriteLine("{0}--{1}{2}:", obj.GetType().Name, indentString, property.Name);
                        PrintPropertiesWithParent(propValue, indent + 2, obj);
                    }
                    else
                    {
                        if (propValue is string[])
                        {
                            var str = new StringBuilder();
                            foreach (string item in (string[])propValue)
                            {
                                str.AppendFormat("{0}; ", item);
                            }
                            propValue = str.ToString();
                            str.Clear();
                        }
                        Console.WriteLine("{0}-{1}{2}: {3}", obj.GetType().Name, indentString, property.Name, propValue);
                    }
                }
            }
        }
    }
}
