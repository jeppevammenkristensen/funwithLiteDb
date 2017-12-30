using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace App.EntryActions
{
    public abstract class EntryAction
    {
        public abstract string Title { get; }
        public abstract Task Execute(IMediator mediator);

        protected T GetEntry<T>(string title, Func<string, T> parser, Func<T, ValidationResult[]> result)
        {
            return ConsoleUtil.GetEntry(title, parser, result, GetConsoleInput);
        }

        public string GetConsoleInput()
        {
            var result = Console.ReadLine();
            if (result?.Equals("exit", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                throw new BreakException();
            }

            return result;
        }

        
    }

    public static class ConsoleUtil
    {
        public static T GetEntry<T>(string name, Func<string, T> parseInput, Func<T, ValidationResult[]> validate, Func<string> getInput = null)
        {
            while (true)
            {
                bool continueProcessing = false;

                Console.WriteLine($"Indtast værdi for {name}");
                var result = getInput == null ? Console.ReadLine() : getInput();

                T parsedResult = default(T);
                try
                {
                    parsedResult = parseInput(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Værdien er ikke korrekt {e.Message}");
                    continueProcessing = true;
                }

                if (!continueProcessing)
                {
                    var validationResult = validate(parsedResult);
                    if (validationResult?.Any() == false)
                    {
                        return parsedResult;
                    }

                    foreach (var valResult in validationResult ?? Enumerable.Empty<ValidationResult>())
                    {
                        Console.WriteLine($"Error: {valResult}");
                    }
                }
            }
        }
    }
}