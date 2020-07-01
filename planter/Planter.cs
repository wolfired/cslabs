using System;

namespace planter
{
    public class Parent
    {
        public string aaaa;
    }

    public interface IParent
    {
        void vvv();
    }

    public class Sun : Parent, IParent
    {
        public class Son
        {

        }
        public Son son;
        public static string lala;
        public string name;
        public readonly string prof;

        protected int sex;

        private int age;

        public string email
        {
            get;
            set;
        }

        public string phone
        {
            get;
        }

        public string call => "";

        public Sun()
        {

        }

        private Sun(int x)
        {

        }

        public Sun(string name)
        {
            this.name = name;
        }


        public void hi(in int a, out Son son, Son son0, Son[] sons, int? b = null, params int[] list)
        {
            son = new Son();
        }

        public virtual int max(int x, int y, float z, double w, bool b)
        {
            return x > y ? x : y;
        }

        protected int say()
        {
            return 0;
        }

        static public int min(int x, int y)
        {
            return x < y ? x : y;
        }

        public void vvv()
        {
            throw new NotImplementedException();
        }
    }
}
