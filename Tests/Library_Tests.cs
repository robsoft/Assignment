using NUnit.Framework;
using FluentAssertions;
using Library;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class Library_Tests
    {
        // file to keep our test list of words in
        private const string _dictionaryFileName = "test_dictionary.txt";

        private List<string> words;

        [SetUp]
        public void Setup()
        {
            words = new List<string>{ "spin", "spit", "spat", "spot", "span" };
            SaveDictionary();
        }


        private void SaveDictionary()
        {
            // just write 'words' to our test dictionary
            System.IO.File.WriteAllLines(_dictionaryFileName, words);
        }


        [Test]
        public void CheckProvidedSolution()
        {
            var scenario = new Solver(_dictionaryFileName, "spin", "spot");

            scenario.ShortestSolutionSteps.Should().Be(2);
            scenario.ShortestSolution.Should().HaveCount(3);
            scenario.ShortestSolution[0].Should().Contain("spin");
            scenario.ShortestSolution[1].Should().Contain("spit");
            scenario.ShortestSolution[2].Should().Contain("spot");
        }

        [Test]
        public void CheckNoSolution()
        {
            // add a word to our test dictionary
            words.Add("door");
            SaveDictionary();

            var scenario = new Solver(_dictionaryFileName, "spin", "door");
            scenario.ShortestSolutionSteps.Should().Be(0);
        }


        [Test]
        public void CheckNoStartWordSolution()
        {
            // should throw if start word not in dictionary
            Action act = () => new Solver(_dictionaryFileName, "spiv", "spot");
            act.Should().Throw<WordNotInDictionaryException>();
        }

        [Test]
        public void CheckNoEndWordSolution()
        {
            // should throw if end word not in dictionary
            Action act = () => new Solver(_dictionaryFileName, "spin", "spiv");
            act.Should().Throw<WordNotInDictionaryException>();
        }

        [Test]
        public void CheckWordAreDifferent()
        {
            // should throw if end word not in dictionary
            Action act = () => new Solver(_dictionaryFileName, "spin", "spiv");
            act.Should().Throw<WordNotInDictionaryException>();
        }

        [Test]
        public void CheckWordLengthsAreSame()
        {
            // add a longer word to our dictionary
            words.Add("latch");
            SaveDictionary();

            // should throw as word lengths not the same
            Action act = () => new Solver(_dictionaryFileName, "spin", "latch");
            act.Should().Throw<WordLengthsNotMatchException>();
        }


        [Test]
        public void CheckDictionaryExists()
        {
            var FileName = "an invalid file.txt";

            // should throw as this file doesn't exist
            Action act = () => new Solver(FileName, "spin", "spiv");
            act.Should().Throw<DictionaryFileNotExistsException>();
        }

        [Test]
        public void CheckDictionaryNoValidWords()
        {
            words.Clear();
            SaveDictionary();

            // should throw as dictionary file is empty
            Action act = () => new Solver(_dictionaryFileName, "spin", "spiv");
            act.Should().Throw<DictionaryNoValidWordsException>();
        }


    }

}