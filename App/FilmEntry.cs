using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using App.EntryActions;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.Commands;

namespace App
{
    public class FilmEntry
    {
        private List<EntryAction> Entries = new List<EntryAction>();

        public FilmEntry()
        {
            Entries.Add(new CreateFilmEntryAction());
            Entries.Add(new AddStorage());
            Entries.Add(new AddStorageToFilm());
            Entries.Add(new ViewStorageEntryAction());
            Entries.Add(new ViewFilmsEntryAction());
            Entries.Add(new SearchMovieDbEntryAction());
            Entries.Add(new RemoveDuplicatesEntryAction());
        }

        public async Task DoFilmEntry()
        {
            var mediator = GetMediator();

            while (true)
            {
                Console.Clear();
                foreach (var entryAction in Entries.Select((e,i) => new { entryAction = e, Index = i+1}))
                {
                    Console.WriteLine($"{entryAction.Index} {entryAction.entryAction.Title}");
                }

                var index = ConsoleUtil.GetEntry("Id", int.Parse, Validator<int>.New().Custom(x => x > 0 && x <= Entries.Count,
                    "Skal være valgbare").Build());
                var action = Entries[index - 1];

                while (true)
                {

                    try
                    {

                        await action.Execute(mediator);

                    }
                    catch (BreakException e)
                    {
                        break;
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return;
                    }
                }
                ////await AddMedia(mediator);
                ////await AddFilms(mediator);
                ////await AddStorage(mediator);
                //try
                //{
                //    await AddMediaToStorage(mediator);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //    Console.ReadKey();
                //}
            }
        }

   

        
        
        private async Task AddMedia(IMediator mediator)
        {
            

        }

        private static IMediator GetMediator()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<FilmEntry>()
                .Build();

            var container = new StructureMap.Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<CreateFilmCommand>(); // Our assembly with requests & handlers
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestPreProcessor<>));
                });


                cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(RequestPreProcessorBehavior<,>));

                cfg.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                cfg.For<IMediator>().Use<Mediator>();
                cfg.For<LiteDbProvider>().Use(x => new LiteDbProvider("c:\\temp\\dvdcollection.db")).Singleton();
                cfg.For<IConfigurationRoot>().Use(x => configuration);
            });

            var mediator = container.GetInstance<IMediator>();
            return mediator;
        }
    }
}