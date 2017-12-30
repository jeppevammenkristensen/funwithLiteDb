using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using MediatR;
using Shared.Queries;

namespace Shared.Commands
{
    public class ListFilmsWithQuery : PagedQuery<IEnumerable<Film>>
    {
        public ListFilmsWithQuery(int page, int pageSize) : base(page, pageSize)
        {
        }

        public string Query { get; set; }
    }

    public class ListFilmsWithQueryRequestHandler : IRequestHandler<ListFilmsWithQuery, IEnumerable<Film>>
    {
        private readonly LiteDbProvider _provider;

        public ListFilmsWithQueryRequestHandler(LiteDbProvider provider)
        {
            _provider = provider;
        }

        public async Task<IEnumerable<Film>> Handle(ListFilmsWithQuery request, CancellationToken cancellationToken)
        {
            using (var liteDatabase = _provider.GetDatabase())
            {
                var filmCollection = liteDatabase
                    .GetCollection<Film>()
                    .Include(x => x.Storages);

                if (!string.IsNullOrWhiteSpace(request.Query))
                {
                    return filmCollection.Find(Query.Contains("Title", request.Query), request.Page * request.PageSize, request.PageSize);
                }

                return filmCollection.Find(Query.All(), request.Page * request.PageSize, request.PageSize);
            }
        }
    }
}