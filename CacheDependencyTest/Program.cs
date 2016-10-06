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
        static void Main(string[] args)
        {
            var lines = ExtractHelper.IterateProps(typeof(Container)).ToArray();

            foreach (var line in lines)
                Console.WriteLine(line);

            Console.ReadLine();
        }
    }

    static class ExtractHelper
    {

        public static IEnumerable<string> IterateProps(Type baseType)
        {
            return IteratePropsInner(baseType, baseType.Name);
        }

        private static IEnumerable<string> IteratePropsInner(Type baseType, string baseName)
        {
            var props = baseType.GetProperties();

            foreach (var property in props)
            {
                var name = property.Name;
                var type = ListArgumentOrSelf(property.PropertyType);
                if (IsMarked(type))
                    foreach (var info in IteratePropsInner(type, name))
                        yield return string.Format("{0}.{1}", baseName, info);
                else
                    yield return string.Format("{0}.{1}", baseName, property.Name);
            }
        }

        static bool IsMarked(Type type)
        {
            return type.GetCustomAttributes(typeof(ExtractNameAttribute), true).Any();
        }


        public static Type ListArgumentOrSelf(Type type)
        {
            if (!type.IsGenericType)
                return type;
            if (type.GetGenericTypeDefinition() != typeof(List<>))
                throw new Exception("Only List<T> are allowed");
            return type.GetGenericArguments()[0];
        }
    }

    [ExtractName]
    public class Container
    {
        public string Name { get; set; }
        public List<Address> Addresses { get; set; }
    }

    [ExtractName]
    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public List<Telephone> Telephones { get; set; }
    }

    [ExtractName]
    public class Telephone
    {
        public string CellPhone { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
    public sealed class ExtractNameAttribute : Attribute
    { }

    //  class Program
    //  {

    //  // private static Dictionary<Type, TableInfo> tableInfoList =
    //  //new Dictionary<Type, TableInfo>();

    ////  private static Dictionary<Type, List<PropertyInfo>> _propertyDictionary = new Dictionary<Type, List<PropertyInfo>>();
    //  private static Dictionary<Type, PropertyInfo> _propertyDictionary = new Dictionary<Type, PropertyInfo>();
    //  static void Main(string[] args)
    //  {
    //      var foo = new TestClassOne();
    //      foo.Name = new Child();
    //      foo.test = "dfddd";

    //      foo.Name.Name1 = new Child();

    //      foo.Name.Children = new Child2();
    //      foo.Name.Children.Children = new List<Child> { new Child(), new Child()  };
    //      var itterations = 1;



    //      Stopwatch sw = Stopwatch.StartNew();

    //      for (var i = 0; i < itterations; i++)
    //      {
    //          //  ParseObject(foo);
    //          foreach (var info in GetPropertInfos(foo))
    //              Console.WriteLine(info);
    //          //       PrintPropertiesWithParent(foo, 1);
    //      }

    //      Console.WriteLine(sw.ElapsedMilliseconds);

    //      Console.ReadKey();
    //  }

    //  public static void Recursive(Type obj)
    //  {
    //      foreach (var prop in obj.GetProperties().Where(n => n.IsDefined(typeof(CacheDependencyAttribute), true)))
    //      {
    //          Console.WriteLine("prop:" + prop);
    //          Recursive(prop.GetType());



    //          var attrs = prop.GetCustomAttributes(true);

    //          foreach (object attr in attrs)
    //          {
    //              var authAttr = attr as CacheDependencyAttribute;
    //              if (authAttr != null)
    //              {
    //                  string propName = prop.Name;
    //                  Direction auth = authAttr.Direction;

    //                  Console.WriteLine(propName);
    //                  Console.WriteLine(auth);
    //              }
    //          }
    //      }
    //  }


    //  private static void ParseObject(object obj, Type parent = null)
    //  {
    //      if (obj == null) return;

    //      Type objType = obj.GetType();

    //     // Type objParent = null;

    //    //  if (parent != null)
    //      //    objParent = parent.GetType();

    //      var properties = objType.GetProperties().Where(n => n.IsDefined(typeof(CacheDependencyAttribute), true));
    //      foreach (PropertyInfo property in properties)
    //      {
    //          object propValue = property.GetValue(obj, null);



    //          var elems = propValue as IList;
    //          if ((elems != null) && !(elems is string[]))
    //          {
    //            //  object last = null;

    //              foreach (var item in elems)
    //              {
    //                  ParseObject(item, property.PropertyType);
    //              }
    //          }
    //          else
    //          {
    //              ParseObject(propValue, objType);
    //          }

    //          var attrs = property.GetCustomAttributes(true);
    //          foreach (object attr in attrs)
    //          {
    //              var authAttr = attr as CacheDependencyAttribute;
    //              if (authAttr != null)
    //              {
    //                  string propName = property.Name;
    //                  Direction direction = authAttr.Direction;

    //                  switch (direction)
    //                  {
    //                      case Direction.Parent:
    //                          Console.WriteLine($"{propName} depends on {parent?.Name}");
    //                          break;
    //                      case Direction.TwoWay:
    //                          Console.WriteLine($"{propName} depends on {parent?.Name}.");
    //                          Console.WriteLine($"{parent?.Name} depends on {propName} .");
    //                          break;
    //                  }
    //                  Console.WriteLine("--");
    //              }
    //          }
    //      }
    //  }

    //  public static IEnumerable<string> GetPropertInfos(object o, string parent = null)
    //  {


    //      Type t = o.GetType();
    //      //   String namespaceValue = t.Namespace;


    //      PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //      foreach (PropertyInfo prp in props)
    //      {
    //          if (prp.PropertyType.Module.ScopeName != "CommonLanguageRuntimeLibrary")
    //          {
    //              // fix me: you have to pass parent + "." + t.Name instead of t.Name if parent != null
    //              object value = prp.GetValue(o);
    //              if (value == null)
    //              {

    //                  value =
    //                      Activator.CreateInstance(Type.GetType(
    //                          (prp.PropertyType).AssemblyQualifiedName.Replace("[]", "")));
    //              }

    //              var propertInfos = GetPropertInfos(value, t.Name);
    //              foreach (var info in propertInfos)
    //                  yield return info;
    //          }
    //          else
    //          {
    //              var type = prp.GetType().Name;

    //              var info = t.Name + "." + prp.Name;
    //              if (String.IsNullOrWhiteSpace(parent))
    //                  yield return info;
    //              else
    //                  yield return parent + "." + info;
    //          }
    //      }
    //  }
    //  private static void PrintProperties(object obj, int indent)
    //  {
    //      if (obj == null) return;
    //      string indentString = new string(' ', indent);
    //      Type objType = obj.GetType();
    //      var properties = objType.GetProperties().Where(n => n.IsDefined(typeof (CacheDependencyAttribute), true));
    //      foreach (PropertyInfo property in properties)
    //      {
    //          object propValue = property.GetValue(obj, null);

    //          var elems = propValue as IList;
    //          if ((elems != null) && !(elems is string[]))
    //          {
    //              foreach (var item in elems)
    //              {
    //                  PrintProperties(item, indent + 3);
    //              }
    //          }
    //          else
    //          {
    //              // This will not cut-off System.Collections because of the first check
    //              if (property.PropertyType.Assembly == objType.Assembly)
    //              {
    //                  Console.WriteLine("{0}{1}:", indentString, property.Name);
    //                  PrintProperties(propValue, indent + 2);
    //              }
    //              else
    //              {
    //                  if (propValue is string[])
    //                  {
    //                      var str = new StringBuilder();
    //                      foreach (string item in (string[])propValue)
    //                      {
    //                          str.AppendFormat("{0}; ", item);
    //                      }
    //                      propValue = str.ToString();
    //                      str.Clear();
    //                  }
    //                  Console.WriteLine("{0}{1}: {2}", indentString, property.Name, propValue);
    //              }
    //          }
    //      }
    //  }

    //  private static void PrintPropertiesWithParent(object obj, int indent, object parent = null)
    //  {
    //      if (obj == null) return;
    //      string indentString = new string(' ', indent);
    //      Type objType = obj.GetType();
    //      var properties = objType.GetProperties().Where(n => n.IsDefined(typeof(CacheDependencyAttribute), true));
    //      foreach (PropertyInfo property in properties)
    //      {
    //          object propValue = property.GetValue(obj, null);

    //          var elems = propValue as IList;
    //          if ((elems != null) && !(elems is string[]))
    //          {
    //              foreach (var item in elems)
    //              {
    //                  PrintPropertiesWithParent(item, indent + 3, obj);
    //              }
    //          }
    //          else
    //          {

    //              // This will not cut-off System.Collections because of the first check
    //            //  if (property.PropertyType.Assembly == objType.Assembly)
    //              {
    //              //    Console.WriteLine("{0}--{1}{2}:", obj.GetType().Name, indentString, property.Name);
    //              //    PrintPropertiesWithParent(propValue, indent + 2, obj);
    //              }
    //            //  else
    //              {
    //                  Console.WriteLine("..");
    //                  if (propValue is string[])
    //                  {
    //                      var str = new StringBuilder();
    //                      foreach (string item in (string[])propValue)
    //                      {
    //                          str.AppendFormat("{0}; ", item);

    //                      }
    //                      propValue = str.ToString();
    //                      str.Clear();
    //                  }
    //                  Console.WriteLine("{0}-{1}{2}: {3}", obj.GetType().Name, indentString, property.Name, propValue);
    //                  PrintPropertiesWithParent(propValue, indent + 2, obj);
    //              }
    //          }
    //      }
    //  }
    //  }
}
