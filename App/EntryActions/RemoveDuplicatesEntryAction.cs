using System.Threading.Tasks;
using MediatR;
using Shared.Commands;

namespace App.EntryActions
{
    public class RemoveDuplicatesEntryAction : EntryAction
    {
        public override string Title
        {
            get { return "Remove duplicates"; }
        }

        public override async Task Execute(IMediator mediator)
        {
            await mediator.Send(new RemoveDuplicates());
            throw new BreakException();
        }
    }
}