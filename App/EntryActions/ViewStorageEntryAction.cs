using System;
using System.Threading.Tasks;
using MediatR;
using Shared.Queries;

namespace App.EntryActions
{
    public class ViewStorageEntryAction : EntryAction
    {
        public override string Title
        {
            get { return "Vis opbevaring"; }
        }

        public override async Task Execute(IMediator mediator)
        {
            int page = 0;

            while (true)
            {
                var result = (await mediator.Send(new ListStorageQuery(page++, 5))).Display();
                if (result.Count < 5)
                    break;
                GetConsoleInput();
            }

            Console.WriteLine("Alle vist");
            Console.ReadLine();
            throw new BreakException();
        }
    }
}