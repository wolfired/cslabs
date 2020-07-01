using System;
using planter;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Mono.Options;

namespace plantor
{
    public class Plantor
    {
        private class ParameterComparer : IComparer<ParameterInfo>
        {
            int IComparer<ParameterInfo>.Compare(ParameterInfo x, ParameterInfo y)
            {
                if (x.Position < y.Position)
                {
                    return -1;
                }
                if (x.Position > y.Position)
                {
                    return 1;
                }
                return 0;
            }
        }

        private List<string> _output;

        public Plantor()
        {
            _output = new List<string>();
        }

        public void exec(string output, string input, string target = null)
        {
            _output.Clear();

            var dll = Assembly.LoadFrom(input);
            Type[] types;

            if (null == target || "" == target)
            {
                types = dll.GetTypes();
            }
            else
            {
                types = new Type[] { dll.GetType(target) };
            }

            _output.Add("@startuml");
            _output.Add("set namespaceSeparator ::");

            foreach (var type in types)
            {
                if ("System" == type.ToString().Split('.')[0] || "Microsoft" == type.ToString().Split('.')[0])
                {
                    continue;
                }
                printType(type);
            }

            _output.Add("@enduml");

            if (0 < _output.Count)
            {
                File.WriteAllLines(output, _output);
            }
        }

        private void printType(Type type)
        {
            List<string> list = new List<string>();

            Type[] interfaces = type.GetInterfaces();
            List<string> interface_strs = new List<string>();

            foreach (var interface_ in interfaces)
            {
                if ("System" == interface_.ToString().Split('.')[0])
                {
                    continue;
                }
                interface_strs.Add(this.formatType(interface_));
            }

            if (type.IsInterface)
            {
                list.Add("interface");
                list.Add(this.formatType(type));

                if (0 < interface_strs.Count)
                {
                    list.Add("extends");
                    list.Add(String.Join(", ", interface_strs));
                }
            }
            else if (type.IsAbstract)
            {
                list.Add("abstract");
                list.Add("class");
                list.Add(formatType(type));

                if (null != type.BaseType)
                {
                    list.Add("extends");
                    list.Add(formatType(type.BaseType));
                }

                if (0 < interface_strs.Count)
                {
                    list.Add("implements");
                    list.Add(String.Join(", ", interface_strs));
                }
            }
            else if (type.IsClass)
            {
                list.Add("class");
                list.Add(formatType(type));

                if (null != type.BaseType)
                {
                    list.Add("extends");
                    list.Add(formatType(type.BaseType));
                }

                if (0 < interface_strs.Count)
                {
                    list.Add("implements");
                    list.Add(String.Join(", ", interface_strs));
                }
            }
            else
            {
                return;
            }

            list.Add("{");

            _output.Add(String.Join(" ", list));

            this.printConstructors(type.Name, type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            this.printFields(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));

            // this.printProperties(type.GetProperties());

            this.printMethods(type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));

            // this.printMembers(type.GetMembers());

            _output.Add("}");
        }

        private void printConstructors(string name, ConstructorInfo[] constructors)
        {
            foreach (var constructor in constructors)
            {
                _output.Add(this.formatConstructor(name, constructor));
            }
        }

        private string formatConstructor(string name, ConstructorInfo constructor)
        {
            List<string> list = new List<string>();

            if (constructor.IsPrivate)
            {
                list.Add("-");
            }
            else if (constructor.IsPublic)
            {
                list.Add("+");
            }
            else
            {
                list.Add("#");
            }

            if (constructor.IsVirtual)
            {
                list.Add("virtual");
            }

            list.Add(name + this.formatParameters(constructor.GetParameters()));

            return String.Join(" ", list);
        }

        private void printFields(FieldInfo[] fields)
        {
            foreach (var field in fields)
            {
                _output.Add(this.formatField(field));
            }
        }

        private string formatField(FieldInfo field)
        {
            List<string> list = new List<string>();

            if (field.IsStatic)
            {
                list.Add("{static}");
            }

            if (field.IsPrivate)
            {
                list.Add("-");
            }
            else if (field.IsPublic)
            {
                list.Add("+");
            }
            else
            {
                list.Add("#");
            }

            list.Add(formatType(field.FieldType));

            list.Add(field.Name);

            return String.Join(" ", list);
        }

        private void printProperties(PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                Console.WriteLine("{0} can read {1}", property, property.CanRead);
                Console.WriteLine("{0} can write {1}", property, property.CanWrite);
            }
        }

        private void printMethods(MethodInfo[] methods)
        {
            foreach (var method in methods)
            {
                _output.Add(this.formatMethod(method));
            }
        }

        private string formatMethod(MethodInfo method)
        {
            List<string> list = new List<string>();

            if (method.IsPrivate)
            {
                list.Add("-");
            }
            else if (method.IsPublic)
            {
                list.Add("+");
            }
            else
            {
                list.Add("#");
            }

            if (method.IsVirtual)
            {

                list.Add("virtual");
            }

            list.Add(formatType(method.ReturnType));
            list.Add(method.Name + this.formatParameters(method.GetParameters()));

            return String.Join(" ", list);
        }

        private string formatParameters(ParameterInfo[] parameters)
        {
            List<string> list = new List<string>();

            Array.Sort(parameters, new ParameterComparer());

            List<string> arg = new List<string>();

            foreach (var parameter in parameters)
            {
                arg.Clear();
                if (parameter.IsIn)
                {
                    arg.Add("in");
                }
                else if (parameter.IsOut)
                {
                    arg.Add("out");
                }
                Type parameter_type = Nullable.GetUnderlyingType(parameter.ParameterType);
                parameter_type = parameter_type ?? parameter.ParameterType;
                if (null != parameter.GetCustomAttribute(typeof(ParamArrayAttribute)))
                {
                    arg.Add("params");
                }
                arg.Add(formatType(parameter_type));
                arg.Add(parameter.Name);
                if (parameter.HasDefaultValue)
                {
                    arg.Add("=");
                    arg.Add(null == parameter.DefaultValue ? "null" : parameter.DefaultValue.ToString());
                }

                list.Add(String.Join(" ", arg));
            }

            return "(" + String.Join(", ", list) + ")";
        }

        private void printMembers(MemberInfo[] members)
        {
            foreach (var member in members)
            {
                Console.WriteLine("{0}", member);
            }
        }

        private string formatType(Type type)
        {
            if (null == type.FullName)
            {
                return "";
            }
            string[] name_arr = type.FullName.Split(".");
            string base_type = name_arr[name_arr.Length - 1];

            int index;

            index = -1;
            if (-1 < (index = base_type.LastIndexOf('&')))
            {
                name_arr[name_arr.Length - 1] = base_type = base_type.Substring(0, index);
            }

            bool is_array = false;

            index = -1;

            if (-1 < (index = base_type.LastIndexOf('[')))
            {
                is_array = true;
                name_arr[name_arr.Length - 1] = base_type = base_type.Substring(0, index);
            }

            switch (base_type)
            {
                case "Int16": return is_array ? "short[]" : "short";
                case "Int32": return is_array ? "int[]" : "int";
                case "Int64": return is_array ? "long[]" : "long";
                case "UInt16": return is_array ? "ushort[]" : "ushort";
                case "UInt32": return is_array ? "uint[]" : "uint";
                case "UInt64": return is_array ? "ulong[]" : "ulong";
                case "Single": return is_array ? "float[]" : "float";
                case "Double": return is_array ? "double[]" : "double";
                case "Boolean": return is_array ? "bool[]" : "bool";
                case "String": return is_array ? "string[]" : "string";
                case "Void": return is_array ? "void[]" : "void";
                default: return is_array ? String.Join("::", name_arr) + "[]" : String.Join("::", name_arr);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string outfile = "";
            string target = "";
            List<string> infiles = new List<string>();
            bool help = false;
            var options = new OptionSet {
                { "o|outfile=", "output file", o => outfile = o },
                { "t|target=", "target", t => target = t },
                { "i|infiles=", "input files", i => infiles.Add(i) },
                { "h|help", "show this message and exit", h => help = h != null },
            };

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("PlantUML: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `plantUML --help' for more information.");
                return;
            }

            if (help)
            {
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            var plantor = new Plantor();
            plantor.exec(outfile, infiles[0], target);
        }
    }
}
