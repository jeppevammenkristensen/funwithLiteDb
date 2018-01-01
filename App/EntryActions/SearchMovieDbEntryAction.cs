using System;
using System.Threading.Tasks;
using MediatR;
using Shared;
using Shared.Commands;
using Shared.Queries;

namespace App.EntryActions
{
    public class SearchMovieDbEntryAction : EntryAction
    {
        public override string Title
        {
            get { return "Search movie database and add"; }
        }

        public override async Task Execute(IMediator mediator)
        {
            string query = GetEntry("Søgning", x => x, Validator<string>.New().Build());
            var searchResult = await mediator.Send(new RequestFilmsFromMovieDb() {Query = query});
            var results = searchResult.Results.Display();
            var itemToAdd = GetEntry("Vælg item", int.Parse, Validator<int>.New().Build());
            var item = results[itemToAdd - 1];

            var type = GetEntry("Type", x => Enum.Parse<TypeofMedia>(x), Validator<TypeofMedia>.New().Build());
            var amount = GetEntry("Antal", x => int.Parse(x), Validator<int>.New().Custom(x => x >= 1, "Skal være større end eller lig med 1").Build());

            await mediator.Send(new CreateFilmCommand()
            {
                ExternalId = item.Id.ToString(),
                ExternalSource = ExternalSource.TheMovieDatabase,
                Title = item.Title,
                Year = item.ReleaseDate?.Year,
                MediaAmount = amount,
                MediaType = type
            });


        }
    }
}