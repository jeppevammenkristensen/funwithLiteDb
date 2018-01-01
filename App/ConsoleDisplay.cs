using System;
using System.Collections.Generic;
using System.Linq;
using Shared;
using Shared.Queries;

namespace App
{
    public static class ConsoleDisplay
    {
        public static IList<Film> Display(this IEnumerable<Film> source)
        {
            var sourceAsList = source.ToList();

            foreach (var film in sourceAsList)
            {
                Console.WriteLine($"{film.Id}\t{film.Title}\t{string.Join(",", film.Medias.Select(x => x.Type))}\t{film.Year}");
            }

            return sourceAsList;
        }

        public static IList<Storage> Display(this IEnumerable<Storage> source)
        {
            var sourceAsList = source.ToList();

            foreach (var storage in sourceAsList)
            {
                Console.WriteLine($"{storage.Id}\t{storage.Title}");
            }

            return sourceAsList;
        }

        public static IList<Result> Display(this IEnumerable<Result> source)
        {
            var sourceAsList = source.ToList();
            var id = 1;

            foreach (var result in sourceAsList)
            {
                Console.WriteLine($"{id++}:{result.Id}\t{result.Title}\t{result.ReleaseDate?.Year}");
            }

            return sourceAsList;
        }
    }
}