using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace SPOLab3_Finale
{
    interface IInterface1
    {
        int Method1(int x, int y);
        double Method2(double p2);
    }
    interface IInterface2
    {
        string Method3(string p3);
        void Method4();
    }

    class MyClass : IInterface1, IInterface2
    {
        public int field1;
        protected double field2;
        private string field3;
        [CommandLineAttribute("str")]
        public string str;
        [CommandLineAttribute("bool")]
        public bool b;
        [CommandLineAttribute("int")]
        public int n;


        public MyClass()
        {
            field1 = 0;
            field2 = 0.0;
            field3 = "";
        }

        public MyClass(int p1, double p2, string p3, int p4)
        {
            field1 = p1;
            field2 = p2;
            field3 = p3;
        }

        public string MyProperties
        {
            get { return field3; }
            set { field3 = value; }
        }

        public int Method1(int p1, int p2)
        {
            Console.Write(p1 + p2);
            return p1 + p2;
        }

        public double Method2(double p)
        {
            field2 = p;
            return field2;
        }

        public string Method3(string p)
        {
            field3 = p;
            return field3;
        }

        public void Method4()
        {
            Console.WriteLine("Пример  строки");
        }

    }

}
