using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using MediatR;

namespace Shared.Commands
{
    public class RemoveDuplicatesRequestHandler : IRequestHandler<RemoveDuplicates>
    {
        private readonly LiteDbProvider _liteDbProvider;

        public RemoveDuplicatesRequestHandler(LiteDbProvider liteDbProvider)
        {
            _liteDbProvider = liteDbProvider;
        }

        public Task Handle(RemoveDuplicates message, CancellationToken cancellationToken)
        {
            using (var liteDatabase = _liteDbProvider.GetDatabase())
            {
                var collection = liteDatabase.GetCollection<Film>();
                var allSources = collection.Find(x => x.ExternalSource != null).Select(x => new {x.Id, x.ExternalId, x.ExternalSource})
                    .GroupBy(x => new {x.ExternalId, x.ExternalSource})
                    .Where(x => x.Count() > 1)
                    .SelectMany(x => x.Skip(1).Select(y => new BsonValue(y.Id)));

                foreach (var allSource in allSources)
                {
                    collection.Delete(allSource);
                }

                return Task.CompletedTask;
            }
        }
    }

    public class RemoveDuplicates : IRequest
    {
    }
}