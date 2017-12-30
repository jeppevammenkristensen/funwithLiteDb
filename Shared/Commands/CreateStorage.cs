using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Shared.Commands
{
    public class CreateStorageCommand : IRequest<int>
    {
        public CreateStorageCommand(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }

    public class CreateStorageCommandRequestHandler : IRequestHandler<CreateStorageCommand, int>
    {
        private readonly LiteDbProvider _provider;

        public CreateStorageCommandRequestHandler(LiteDbProvider provider)
        {
            _provider = provider;
        }

        public async Task<int> Handle(CreateStorageCommand request, CancellationToken cancellationToken)
        {
            using (var liteDatabase = _provider.GetDatabase())
            {
                var storages = liteDatabase.GetCollection<Storage>();
                return storages.Insert(new Storage() {Title = request.Title}).AsInt32;
            }
        }
    }
}