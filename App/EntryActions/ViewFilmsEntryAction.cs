using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Shared.Commands;
using Shared.Queries;

namespace App.EntryActions
{
    public class ViewFilmsEntryAction : EntryAction
    {
        public override string Title
        {
            get { return "Vis film"; }
        }

        public override async Task Execute(IMediator mediator)
        {
            var query = GetEntry("Søgning (kan være tom)", x => x, Validator<string>.New().Build());
            int page = 0;
            while (true)
            {
                var result = await mediator.Send(new ListFilmsWithQuery(page++, 5)
                {
                    Query = query
                });
                var listResult = result.Display();
                if (listResult.Count < 5)
                    break;
                GetConsoleInput();
            }

            Console.WriteLine("Alle titler vist");
        }

        
    }
}