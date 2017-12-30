using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using MediatR;

namespace Shared.Commands
{
    public class AddFilmToStorageRequestHandler : IRequestHandler<AddFilmToStorageCommand>
    {
        private readonly LiteDbProvider _provider;

        public AddFilmToStorageRequestHandler(LiteDbProvider provider)
        {
            _provider = provider;
        }

        public async Task Handle(AddFilmToStorageCommand message, CancellationToken cancellationToken)
        {
            using (var database = _provider.GetDatabase())
            {
                var filmsCollection = database.GetCollection<Film>();
                var storageCollection = database.GetCollection<Storage>();

                var storage = storageCollection.FindById(message.StorageId);
                if (storage == null)
                    throw new InvalidOperationException($"Could not find storage with id {message.StorageId}");

                var film = filmsCollection.FindById(message.FilmId);
                
                if (film == null)
                {
                    throw new InvalidOperationException($"Could not find film with id {message.FilmId}");
                }

                film.Storages.Add(storage);
                filmsCollection.Update(film);

            }
        }
    }

    public class AddFilmToStorageCommand : IRequest
    {
        public int StorageId { get; set; }
       
        public int FilmId { get; set; }
    }
}