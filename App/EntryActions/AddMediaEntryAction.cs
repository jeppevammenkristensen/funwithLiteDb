using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared;
using Shared.Commands;
using Shared.Queries;

namespace App.EntryActions
{
    public class AddMediaEntryAction : EntryAction
    {
        public override string Title
        {
            get { return nameof(AddMediaEntryAction); }
        }

        public override async Task Execute(IMediator mediator)
        {
            var films = (await mediator.Send(new ListFilmWithNoMedia(0, 20))).ToList();
            foreach (var film in films)
            {
                Console.WriteLine($"{film.Id}\t{film.Title}\t{string.Join(",", film.Medias.Select(x => x.Type))}\t{film.Year}");
            }

            var id = GetEntry("Id", x => int.Parse(x),
                Validator<int>.New().Custom(x => films.Any(y => y.Id == x), "Du skal bruge et valid id").Build());
            var amount = GetEntry("Antal", x => int.Parse(x), Validator<int>.New().Custom(x => x >= 1, "Skal være større end eller lig med 1").Build());
            var type = GetEntry("Type", x => Enum.Parse<TypeofMedia>(x), Validator<TypeofMedia>.New().Build());

            await mediator.Send(new AddMediaToFilm(id, type, amount));
        }
    }
}