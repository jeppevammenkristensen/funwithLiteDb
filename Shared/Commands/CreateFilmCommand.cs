using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Shared.Commands
{ 
    public class CreateFilmHandler : IRequestHandler<CreateFilmCommand, int>
    {
        private readonly LiteDbProvider _provider;

        public CreateFilmHandler(LiteDbProvider provider)
        {
            _provider = provider;
        }

        public async Task<int> Handle(CreateFilmCommand request, CancellationToken cancellationToken)
        {
            using (var liteDatabase = _provider.GetDatabase())
            {
                var film = liteDatabase.GetCollection<Film>();
                return film.Insert(new Film()
                {
                    Title = request.Title,
                    Year = request.Year
                }).AsInt32;
            }
        }
    }

    public class CreateFilmCommand : IRequest<int>
    {
        public string Title { get; set; }
        public int? Year { get; set; }
    }
}