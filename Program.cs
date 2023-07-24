namespace TextAdventure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game.InitializeGame();
            Game.Play();
            Console.WriteLine("Thanks for playing!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}