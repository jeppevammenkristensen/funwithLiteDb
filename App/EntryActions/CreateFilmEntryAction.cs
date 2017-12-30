using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared;
using Shared.Commands;
using Shared.Queries;

namespace App.EntryActions
{
    public class CreateFilmEntryAction : EntryAction
    {
        public override string Title
        {
            get { return nameof(CreateFilmEntryAction); }
        }

        public override async Task Execute(IMediator mediator)
        {
            var films = await mediator.Send(new ListFilmsQuery(0, 20));
            films.Display().ToList();

            var titel = GetEntry("Titel", x => x,
                Validator<string>
                .New()
                .Custom(x => !string.IsNullOrEmpty(x), "Titel må ikke være tom")
                .Build());

            var year = GetEntry("År", x => int.Parse(x),
                Validator<int>
                .New()
                .Custom(x => x > 1900, "Året skal være større end 1900")
                .Build());

            var type = GetEntry("Type", x => Enum.Parse<TypeofMedia>(x), Validator<TypeofMedia>.New().Build());
            var amount = GetEntry("Antal", x => int.Parse(x), Validator<int>.New().Custom(x => x >= 1, "Skal være større end eller lig med 1").Build());


            var result = await mediator.Send(new CreateFilmCommand()
            {
                Title = titel,
                Year = year,
                MediaType = type,
                MediaAmount = amount

            });

            Console.WriteLine(result);
        }
    }
}