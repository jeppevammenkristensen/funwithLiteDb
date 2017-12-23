using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using MediatR;

namespace Shared.Queries
{
    public class ListFilmsQuery : IRequest<IEnumerable<Film>>
    {
        public ListFilmsQuery(int page = 0, int pageSize = 100)
        {
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; }
        public int PageSize { get; }
    }

    public class ListFilmsQueryHandler : IRequestHandler<ListFilmsQuery, IEnumerable<Film>>
    {
        private readonly LiteDbProvider _provider;

        public ListFilmsQueryHandler(LiteDbProvider provider)
        {
            _provider = provider;
        }

        public async Task<IEnumerable<Film>> Handle(ListFilmsQuery request, CancellationToken cancellationToken)
        {
            using (var liteDatabase = _provider.GetDatabase())
            {
                var collection = liteDatabase.GetCollection<Film>();
                return collection.Find(Query.All("Title"), request.Page * request.PageSize, request.PageSize);
            }
        }
    }
}
