using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Shared.Commands
{
    public class AddMediaToFilmHandler : IRequestHandler<AddMediaToFilm>
    {
        private readonly LiteDbProvider _provider;

        public AddMediaToFilmHandler(LiteDbProvider provider)
        {
            _provider = provider;
        }

        public async Task Handle(AddMediaToFilm message, CancellationToken cancellationToken)
        {
            using (var liteDatabase = _provider.GetDatabase())
            {
                var filmCollection = liteDatabase.GetCollection<Film>();
                var film = filmCollection.FindById(message.FilmId);
                film.Medias.Add(new Media(){Amount = message.Amount, Type = message.TypeOfMedia});
                filmCollection.Update(film);
            }
        }
    }
}