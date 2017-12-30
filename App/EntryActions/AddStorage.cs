using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Shared.Commands;
using Shared.Queries;

namespace App.EntryActions
{
    public class AddStorage : EntryAction
    {
        public override string Title
        {
            get { return nameof(AddStorage); }
        }

        public override async Task Execute(IMediator mediator)
        {


            var text = GetEntry("Titel", x => x, Validator<string>.New().Build());

            var result = await mediator.Send(new CreateStorageCommand(text));
            var queryResult = await mediator.Send(new ListStorageQuery(0, 100));
            foreach (var storage in queryResult)
            {
                Console.WriteLine($"{storage.Id}:{storage.Title}");
            }

        }
    }
}