using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

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

            if (!char.IsLetter(premiereLettre))
            {
                CancelRecentInputChar(input);
                return false;
            }
            if (input?.Length > 2)
            {
                var autreNumero = input.Substring(2);
                foreach (var c in autreNumero)
                {
                    if (!char.IsDigit(c))
                    {
                        CancelRecentInputChar(input);
                        return false;
                    }
                }
            }

            return true;
        }
        public static string FormatAndValidateInput(string? rawInput)
        {
            if (string.IsNullOrEmpty(rawInput))
            {
                return string.Empty;
            }

            // 1. Nettoyer la saisie : Supprimer les espaces et les caractères non numériques
            StringBuilder digitsOnly = new StringBuilder();
            foreach (char c in rawInput)
            {
                // On annule le caractère si ce n'est pas un entier (chiffre)
                if (char.IsDigit(c))
                {
                    digitsOnly.Append(c);
                }
            }

            string cleanedInput = digitsOnly.ToString();

            // Si après nettoyage, il n'y a rien, on s'arrête.
            if (cleanedInput.Length == 0)
            {
                return string.Empty;
            }

            // 2. Appliquer les règles de formatage
            StringBuilder formatted = new StringBuilder();

            for (int i = 0; i < cleanedInput.Length; i++)
            {
                char c = cleanedInput[i];
                formatted.Append(c);

                //// Règle 1 : Ajouter un espace après le 3ème caractère
                //if (i == 2)
                //{
                //    formatted.Append(' ');
                //    continue; // Passer au caractère suivant après l'espace
                //}

                // Règle 2 : Ajouter un espace tous les 2 caractères APRÈS les 3 premiers
                // (i >= 3) && (i - 2) % 2 == 0
                // Cela signifie : si l'indice est 4, 6, 8, etc.
                if ((i + 1) % 2 == 0)
                {
                    // Vérifiez que nous ne sommes pas à la fin de la chaîne pour éviter un espace final
                    if (i < cleanedInput.Length - 1)
                    {
                        formatted.Append(' ');
                    }
                }
            }

            return formatted.ToString();
        }
        public static string CapitalizeEachWord(string input)
        {
            // 1. Vérification de la chaîne
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            // 2. Définir la culture
            // Nous utilisons la culture actuelle (fr-GA dans votre cas) pour une gestion correcte 
            // des apostrophes et des accents dans le contexte francophone.
            TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

            // 3. Appliquer ToTitleCase
            // Attention : ToTitleCase met d'abord toute la chaîne en minuscules avant de capitaliser la première lettre de chaque mot.
            // Cela garantit que "THOMAS JUNIOR" devient "Thomas Junior".
            string formattedString = textInfo.ToTitleCase(input.ToLower());

            return formattedString;
        }
        //public static int ValidIntString(string input)
        //{
        //    if (input < 0)
        //    {
        //        input = ToAbsoluteValue(input);
        //        return input;
        //    }
        //    return input;
        //}
        public static decimal ValidPriceString(decimal input)
        {
            if (decimal.IsNegative(input))
            {
                input = ToAbsoluteValue(input);
                return input;
            }

            var inputStr = input.ToString();
            foreach (var c in inputStr)
            {
                if (!char.IsDigit(c) && c != ',')
                {
                    CancelRecentInputDecimal(input);
                    return input;
                }
            }
            return input;
        }


        // formateur
        public static string TrimString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return input.Trim();
        }
        public static decimal ToAbsoluteValue(decimal input)
        {
            input = decimal.Abs(input);
            return input;
        }
        public static int ToAbsoluteValue(int input)
        {
            input = int.Abs(input);
            return input;
        }
        public static string CancelRecentInputChar(string input)
        {
            input = input.Substring(0, input.Length - 1);
            return input;
        }
        public static decimal CancelRecentInputDecimal(decimal input)
        {
            string strInput = input.ToString()[..^1];
            input = decimal.Parse(strInput);
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
