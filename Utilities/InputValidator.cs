using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Utilities
{
    public static class InputValidator
    {
        // validator


        // formateur
        public static string ToUpperString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return input.ToUpperInvariant();
        }
    }
}
