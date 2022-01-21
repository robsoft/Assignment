using System;

namespace Library
{

    public class WordNotInDictionaryException : Exception
    {
        public WordNotInDictionaryException()
        {
        }

        public WordNotInDictionaryException(string message)
            : base(message)
        {
        }

        public WordNotInDictionaryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }


    public class WordLengthsNotMatchException : Exception
    {
        public WordLengthsNotMatchException()
        {
        }

        public WordLengthsNotMatchException(string message)
            : base(message)
        {
        }

        public WordLengthsNotMatchException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class WordsAreSameException : Exception
    {
        public WordsAreSameException()
        {
        }

        public WordsAreSameException(string message)
            : base(message)
        {
        }

        public WordsAreSameException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class InvalidWordException : Exception
    {
        public InvalidWordException()
        {
        }

        public InvalidWordException(string message)
            : base(message)
        {
        }

        public InvalidWordException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }


    public class DictionaryNoValidWordsException : Exception
    {
        public DictionaryNoValidWordsException()
        {
        }

        public DictionaryNoValidWordsException(string message)
            : base(message)
        {
        }

        public DictionaryNoValidWordsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }


    public class DictionaryFileNotExistsException : Exception
    {
        public DictionaryFileNotExistsException()
        {
        }

        public DictionaryFileNotExistsException(string message)
            : base(message)
        {
        }

        public DictionaryFileNotExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
