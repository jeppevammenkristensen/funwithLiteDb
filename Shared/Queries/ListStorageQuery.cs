using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using MediatR;

namespace Shared.Queries
{
    public class ListStorageRequestHandler : IRequestHandler<ListStorageQuery, IEnumerable<Storage>>
    {
        private readonly LiteDbProvider _provider;

        public ListStorageRequestHandler(LiteDbProvider provider)
        {
            _provider = provider;
        }

        public async Task<IEnumerable<Storage>> Handle(ListStorageQuery request, CancellationToken cancellationToken)
        {
            using (var db = _provider.GetDatabase())
            {
                var storageCollection = db.GetCollection<Storage>();
                return storageCollection.Find(Query.All("Title"),request.Page, request.PageSize);
            }
        }
    }

    public class ListStorageQuery : PagedQuery<IEnumerable<Storage>>
    {
        public ListStorageQuery(int page, int pageSize) : base(page, pageSize)
        {
        }
    }
}