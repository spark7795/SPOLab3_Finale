using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
//using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Mono.Cecil;

namespace SPOLab3_Finale
{
    class MyTestClass : MyClass
    {
        const string NS = "SPOLab3_Finale"; //константное значение пространства имен (необходимо для открытия файлов)
                                   //Выводит по имени класса имена методов, которые содержат строковые параметры
        public void InfoStrMeth(string className)
        {
            Type myType = Type.GetType(NS + "." + className);
            foreach (MethodInfo meth in myType.GetMethods())
            {
                foreach (ParameterInfo infPar in meth.GetParameters())
                {
                    if (infPar.ParameterType == typeof(String))
                    {
                        Console.WriteLine(meth.Name);
                    }
                }
            }

        }

        //Вызывать метод класса, значения для его параметров читаются из текстового файла

        public void EvokeMethod(string className, string name, string filename)

        {
            StreamReader readF = new StreamReader(filename);
            Type myType = Type.GetType(NS + "." + className);
            object exemplar = Activator.CreateInstance(myType);
            MethodInfo infM = myType.GetMethod(name);
            ParameterInfo[] param = infM.GetParameters();
            object[] arg = new object[param.Length];
            string line;
            for (int i = 0; i < param.Length; i++)
            {
                line = readF.ReadLine();
                switch (param[i].ParameterType.Name)
                {
                    case "Double":
                        arg[i] = Double.Parse(line);
                        break;
                    case "Int32":
                        arg[i] = Int32.Parse(line);
                        break;
                    default:
                        arg[i] = line;
                        break;
                }
            }
            readF.Close();
            infM.Invoke(exemplar, arg);
        }
        //Выводит всё содержимое класса в текстовый файл
        public void ClassToText(string className)
        {
            Type t = Type.GetType(NS + "." + className);
            StreamWriter wrt = new StreamWriter("allData.txt");
            wrt.WriteLine("using System;");
            wrt.WriteLine("namespace {0}", t.Namespace);
            wrt.WriteLine("{");
            Type[] ifaces = t.GetInterfaces();
            foreach (Type i in ifaces)
            { wrt.WriteLine("{0} ", i.Name); }
            wrt.WriteLine("{");
            MemberInfo[] me = t.GetMembers();
            foreach (MemberInfo fld in me)
            { wrt.WriteLine("{0}", fld.Name); }
            wrt.WriteLine("}}");
            wrt.Close();

        }

        //Записывает все члены класса в файл *.cs

        public void ClassToCS(string className)
        {
            Type t = Type.GetType(NS + "." + className);
            StreamWriter writer = new StreamWriter("compile.cs");
            MethodInfo[] mi = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (MethodInfo m in mi)
            {
                Globals.LoadOpCodes();
                MethodBodyReader mr = new MethodBodyReader(m);
                string msil = mr.GetBodyCode();
                writer.WriteLine(msil);
            }
            writer.Close();
        }

        // Разбор командной строки
        public object CMDLine(string[] args, string nameClass)
        {
            Type t = Type.GetType(NS + "." + nameClass);
            object obj = Activator.CreateInstance(t);
            foreach (FieldInfo fi in t.GetFields())
            {
                var parameter = fi.GetCustomAttributes(typeof(CommandLineAttribute), false);
                foreach (var par in parameter.Cast<CommandLineAttribute>())
                {
                    string s1 = fi.FieldType.Name;
                    string s = "-" + par.CommandSwitch + "=";
                    string st;
                    foreach (string arg in args)
                    {
                        if (arg.Contains(s))
                        {
                            int n = arg.IndexOf(s);
                            st = arg.Remove(n, s.Length);
                            var result = Convert.ChangeType(st, fi.FieldType);
                            fi.SetValue(obj, result);
                        }
                    }
                }
            }
            return obj;
        }
    }
    class CommandLineAttribute : Attribute
    {
        public string CommandSwitch;
        public CommandLineAttribute(string CommandSwitch)
        {
            this.CommandSwitch = CommandSwitch;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            MyTestClass mtc = new MyTestClass();
            Console.WriteLine(" Все методы, принимающие в параметры данные строкового типа:");
            mtc.InfoStrMeth("MyClass");
            Console.WriteLine("\n Вызов метода по его имени: ");
            mtc.EvokeMethod("MyClass", "Method1", "args.txt");
            mtc.ClassToText("MyClass");
            mtc.ClassToCS("MyClass");
            MyClass mc = (MyClass)mtc.CMDLine(args, "MyClass");
            Console.WriteLine("\n Присвоение параметров из командной строки: ");
            Console.WriteLine("str=" + mc.str);
            Console.WriteLine("n=" + mc.n);
            Console.WriteLine("b=" + mc.b);
            Console.ReadKey();
        }

    }
}