using System;

//Testing the functionality of the Multimap

namespace MultiMap
{
    public class MyClass
    {
        public string Value { get; }

        public MyClass(string value) { Value = value; }
        public override string ToString() => Value;
    }

    public class MyClass2 : MyClass 
    {
        public MyClass2(string value) : base(value) { }
        public override string ToString() => String.Format("'{0}'", Value);
    }

    class Program
    {

        static void Main(string[] args)
        {
            var v1 = new MyClass("car");
            var v2 = new MyClass("plane");
            var v3 = new MyClass("NPC2");
            var test = new MultiMap<string, MyClass>(pV => pV == null );

            test.AddKeyEvent += (source, key) => Console.WriteLine("ADDDED KEY: {0}", key);
            test.RemoveKeyEvent += (source, key) => Console.WriteLine("REMOVED KEY: {0}", key);

            test.AddValueEvent += (source, value) => Console.WriteLine("ADDDED VALUE: {0}", value);
            test.RemoveValueEvent += (source, value) => Console.WriteLine("REMOVED VALUE: {0}", value);

            try
            {
                test.Add("vehicle", v1);
                test.Add("vehicle", v2);
                test.Add("npc", v3);
                test.Add("npc", v3);
                test.Add("npc", v3);
                //test.Add("npc", null);
            }
            catch (ValueNotAllowedException valueNotAllowedException)
            {
                throw valueNotAllowedException;
            }
            

            var v4 = new MyClass2("NPC2");
            var test2 = new MultiMap<string, MyClass2>(pV => pV == null);
            test2.Add("npc", v4);
            test2.Add("npc", v4);
            test2.Add("npc", v4);
            test2.Add("pc", v4);

            test.Add(test2);
            Console.WriteLine("Keys: {0}", String.Join(", ", test.Keys));
            Console.WriteLine("Values: {0}", String.Join(", ", test.Values));
            Console.WriteLine("Key {1}: {0}", String.Join(", ", test["npc"]), "npc");
            Console.WriteLine("Key {1}: {0}", String.Join(", ", test["pc"]), "pc");
            Console.WriteLine("Contains {1}->{2}: {0}", test.ContainsValue("vehicle", v1), "vehicle", v1);
            Console.WriteLine("Contains {1}->{2}: {0}", test.ContainsValue("vehicles", v1), "vehicles", v1);

            test.Remove("npc", v3);
            Console.WriteLine("Key {1}: {0}", String.Join(", ", test["npc"]), "npc");
            test.Remove("npc", v3);
            Console.WriteLine("Key {1}: {0}", String.Join(", ", test["npc"]), "npc");
            test.Remove("npc", v3);
            Console.WriteLine("Contains {1}: {0}", test.ContainsKey("npc"), "npc");

            test.RemoveAll( (key, value) => ((key.Length + value.Value.Length) % 2) == 1);

            Console.ReadLine();
        }
    }
}
