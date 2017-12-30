using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Shared.Commands;
using Shared.Queries;

namespace App.EntryActions
{
    public class AddStorageToFilm : EntryAction{
        public override string Title
        {
            get { return nameof(AddStorageToFilm); }
        }

        public override async Task Execute(IMediator mediator)
        {
            (await mediator.Send(new ListFilmsQuery(0, 5))).Display().ToList();
            (await mediator.Send(new ListStorageQuery(0, 5))).Display().ToList();

            var filmId = GetEntry("FilmId", int.Parse,
                Validator<int>
                    .New()
                    .Custom(x => x > 0, "Værdien skal være større end 0")
                    .Build());

            var storageId = GetEntry("StorageId", int.Parse,
                Validator<int>
                    .New()
                    .Custom(x => x > 0, "Værdien skal værre større end 0")
                    .Build());

            await mediator.Send(new AddFilmToStorageCommand()
            {
                FilmId = filmId,
                StorageId = storageId
            });
        }
    }
}