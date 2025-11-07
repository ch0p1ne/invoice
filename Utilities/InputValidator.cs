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
        public static bool IsValidReferenceString(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return true;
            var premiereLettre = input[0];
            var autreNumero = input.Substring(1);
            if (!char.IsLetter(premiereLettre))
            {
                CancelRecentInputChar(input);
                return false;
            }
            foreach (var c in autreNumero)
            {
                if (!char.IsDigit(c))
                {
                    CancelRecentInputChar(input);
                    return false;
                }
            }
            return true;
        }
        public static bool IsValidPriceString(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return true;
            foreach (var c in input)
            {
                if (!char.IsDigit(c) && c != ',')
                {
                    CancelRecentInputChar(input);
                    return false;
                }
            }
            return true;
        }


        // formateur
        public static string TrimString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return input.Trim();
        }
        public static string CancelRecentInputChar(string input)
        {
            input = input.Substring(0,input.Length - 1);
            return input;
        }
        public static string? ToUpperString(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return input.ToUpperInvariant();
        }
        // Remplacement de la méthode ToUpperCharAt pour corriger CS0200
        public static string ToUpperCharAt(string input, int pos)
        {
            if (string.IsNullOrEmpty(input) || pos < 0 || pos >= input.Length)
                return input;
            var chars = input.ToCharArray();
            chars[pos] = char.ToUpperInvariant(chars[pos]);
            return new string(chars);
        }

        public static string? ToLowerString(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return input.ToLowerInvariant();
        }
        

    }
}
