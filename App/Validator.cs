using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace App
{
    public class Validator<T>
    {
        private readonly List<Func<T, (bool, string)>> Validators = new List<Func<T, (bool, string)>>();

        public Validator<T> Custom(Func<T, bool> validate, string message)
        {
            Validators.Add((t) =>
            {
                var valid = validate(t);
                return (valid, valid ? null : message);
            });

            return this;
        }

        public Validator<T> NotNull()
        {
            Validators.Add((t) =>
            {
                if (t == null)
                {
                    return (false, "Værdien må ikke være tom");
                }

                return (true, null);
            });

            return null;
        }

        public static Validator<T> New()
        {
            return new Validator<T>();
        }

        public Func<T, ValidationResult[]> Build()
        {
            return t =>
            {
                return Validators.Select(x => x(t)).Where(x => !x.Item1)
                    .Select(x => new ValidationResult(x.Item2))
                    .ToArray();
            };
        }
    }
}