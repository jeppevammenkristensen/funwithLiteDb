using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Shared.Queries
{
    public abstract class PagedQuery<T> : IRequest<T>
    {
        protected PagedQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; }
        public int PageSize { get; }
    }

    public class ListFilmWithNoMedia : PagedQuery<IEnumerable<Film>>
    {
        public ListFilmWithNoMedia(int page, int pageSize) : base(page, pageSize)
        {
        }
    }

    public class ListFilmWithNoMediaRequestHandler : IRequestHandler<ListFilmWithNoMedia, IEnumerable<Film>>
    {
        private readonly LiteDbProvider _provider;

        public ListFilmWithNoMediaRequestHandler(LiteDbProvider provider)
        {
            _provider = provider;
        }


        public async Task<IEnumerable<Film>> Handle(ListFilmWithNoMedia request, CancellationToken cancellationToken)
        {
            using (var liteDatabase = _provider.GetDatabase())
            {
                var liteCollection = liteDatabase.GetCollection<Film>();
                return liteCollection.Find(x => x.Medias.Count == 0, request.Page, request.PageSize);
            }
        }
    }
}