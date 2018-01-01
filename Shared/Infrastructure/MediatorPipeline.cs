using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using MediatR;
using MediatR.Pipeline;
using Shared.Commands;

namespace Shared.Infrastructure
{
    public class MediatorPipeline : IRequestPreProcessor<CreateFilmCommand>
    {
        private readonly LiteDbProvider _provider;

        public MediatorPipeline(LiteDbProvider provider)
        {
            _provider = provider;
        }

        public Task Process(CreateFilmCommand request, CancellationToken cancellationToken)
        {
            if (request.ExternalSource != null && request.ExternalId != null)
            {
                using (var liteDatabase = _provider.GetDatabase())
                {
                    var collection = liteDatabase.GetCollection<Film>();
                    var result = collection.Count(Query.And(Query.EQ("ExternalId",request.ExternalId), Query.EQ("ExternalSource", request.ExternalSource.ToString())));
                    if (result > 0)
                    {
                        throw new ValidationException(
                            $"There is allready a film with the reference {request.ExternalId} for source {request.ExternalSource}");
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
    
}