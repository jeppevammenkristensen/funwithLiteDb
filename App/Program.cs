using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using MediatR;
using Shared;
using Shared.Commands;
using Shared.Queries;

namespace App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fileentry = new FilmEntry();
            await fileentry.DoFilmEntry();
            
        }
    }

    public class FilmEntry
    {
        public async Task DoFilmEntry()
        {
            var mediator = GetMediator();

            while (true)
            {
                Console.Clear();
                await AddMedia(mediator);
                await AddFilms(mediator);
            }
        }

        private async Task AddMedia(IMediator mediator)
        {
            var films = await mediator.Send(new ListFilmWithNoMedia(0, 20));
            foreach (var film in films)
            {
                Console.WriteLine($"{film.Id}\t{film.Title}\t{string.Join(",",film.Medias.Select(x => x.Type))}\t{film.Year}");
            }

            var id = GetEntry("Id", x => int.Parse(x),
                Validator<int>.New().Custom(x => films.Any(y => y.Id == x), "Du skal bruge et valid id").GetFunc());
            var media = GetEntry("Antal", x => int.Parse(x), Validator<int>.New().GetFunc());
            var type = GetEntry("Type", x => Enum.Parse<TypeofMedia>(x), Validator<TypeofMedia>.New().GetFunc());

            await mediator.Send(new AddMediaToFilm(id, type, media));

        }

        private async Task AddFilms(IMediator mediator)
        {
            var films = await mediator.Send(new ListFilmsQuery(0, 20));
            foreach (var film in films)
            {
                Console.WriteLine($"{film.Id}\t{film.Title}\t{string.Join(",", film.Medias.Select(x => x.Type))}\t{film.Year}");
            }

            var titel = GetName();
            var year = GetEntry("År", x => int.Parse(x), Validator<int>.New()
                //.NotNull()
                .Custom(x => x > 1900, "Året skal være større end 1900").GetFunc());

            var result = await mediator.Send(new CreateFilmCommand()
            {
                Title = titel,
                Year = year
            });

            Console.WriteLine(result);
        }

        private static IMediator GetMediator()
        {
            var container = new StructureMap.Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<CreateFilmCommand>(); // Our assembly with requests & handlers
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });
                cfg.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                cfg.For<IMediator>().Use<Mediator>();
                cfg.For<LiteDbProvider>().Use(x => new LiteDbProvider("c:\\temp\\dvdcollection.db")).Singleton();
            });

            var mediator = container.GetInstance<IMediator>();
            return mediator;
        }

        private string GetName()
        {
            return GetEntry<string>("Titel", x => x,
                Validator<string>.New().Custom(x => !string.IsNullOrEmpty(x), "Titel må ikke være tom").GetFunc());
        }

        private T GetEntry<T>(string name, Func<string, T> parseInput, Func<T, ValidationResult[]> validate)
        {
            while (true)
            {
                bool continueProcessing = false;

                Console.WriteLine($"Indtast værdi for {name}");
                var result = Console.ReadLine();

                T parsedResult = default(T);
                try
                {
                    parsedResult = parseInput(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Værdien er ikke korrekt {e.Message}");
                    continueProcessing = true;
                }

                if (!continueProcessing)
                {
                    var validationResult = validate(parsedResult);
                    if (validationResult?.Any() == false)
                    {
                        return parsedResult;
                    }

                    foreach (var valResult in validationResult ?? Enumerable.Empty<ValidationResult>())
                    {
                        Console.WriteLine($"Error: {valResult}");
                    }
                }
            }

        }


        public class Validator<T>
        {
            private readonly List<Func<T, (bool, string)>> Validators = new List<Func<T, (bool, string)>>();

            public Validator<T> Custom(Func<T, bool> validate, string message)
            {
                Validators.Add((t) =>
                {
                    var valid = validate(t);
                    return (valid, valid ? null : message);
                });

                return this;
            }

            public Validator<T> NotNull() 
            {
                Validators.Add((t) =>
                {
                    if (t == null)
                    {
                        return (false, "Værdien må ikke være tom");
                    }

                    return (true, null);
                });

                return null;
            }

            public static Validator<T> New()
            {
                return new Validator<T>();
            }

            public Func<T, ValidationResult[]> GetFunc()
            {
                return t =>
                {
                    return this.Validators.Select(x => x(t)).Where(x => !x.Item1)
                        .Select(x => new ValidationResult(x.Item2))
                        .ToArray();
                };
            }
        }
    }
}
