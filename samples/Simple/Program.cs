using System;
using CompSharp;
using CompSharp.BuiltIn;
using CompSharp.InMemory;

namespace Simple
{
    public class Program
    {
        public static void Main(string[] args)
        { 
            Console.WriteLine("Hello World");

            var host = new InMemoryEntityHost();
            var entity = host.Create<Player, PlayerToString>(new {Name = "A Player Name "});
            entity.Complete<ReverseNameDecorator>();
            Console.WriteLine(entity);

            Console.WriteLine("Bye World");
            Console.ReadKey(true);
        }
    }

    public interface IPlayer
    {
        string Name { get; set; }
    }

    public interface INameDecorator
    {
        string Decorate(string name);
    }

    public class ReverseNameDecorator : Component, INameDecorator
    {
        public string Decorate(string name)
        {
            var charArray = name.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    public class Player : Component, IPlayer
    {
        public string Name { get; set; }
    }

    public class PlayerToString : Component, IToString
    {
        [Required]
        private IPlayer Player { get; set; }

        [Support]
        private INameDecorator NameDecorator { get; set; }

        string IToString.ToString()
        {
            return NameDecorator == null ? Player.Name : NameDecorator.Decorate(Player.Name);
        }
    }
}