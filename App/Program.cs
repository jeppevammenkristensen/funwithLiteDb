using System;
using System.Threading.Tasks;

namespace App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fileentry = new FilmEntry();
            await fileentry.DoFilmEntry();
            Console.ReadLine();
        }
    }
}
