using System.Linq;

namespace Library
{
    public static class ExtensionMethods
    {
        // Compare the 2 strings and return true if only 1 char is different.
        //TODO: this will bomb if compareString is shorter than the subject string
        public static bool HasSingleCharDifferent(this string str, string compareString) =>
            str.Where((chr, idx) => str[idx] != compareString[idx]).Count() == 1;

    }
}
