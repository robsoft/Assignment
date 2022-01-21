using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Library
{

    public class Solution : List<string> { }


    public class Solver
    {

        public string StartWord { get; private set; }
        public string EndWord { get; private set; }

        public List<Solution> Solutions { get; private set; } = new List<Solution>();

        public Solution ShortestSolution { get => GetShortestSolution(); }
        public int ShortestSolutionSteps { get => GetShortestSolutionSteps(); }


        private HashSet<string> _validWordList = new HashSet<string>();
        private int _bfsDepth = 0;
        private Dictionary<string, int> _depthDictionary;
        private HashSet<string> _previouslyCheckedWords;
        private Solution _shortestSolution;


        /// <summary>
        /// Attempts to solve start->end word ladder puzzle from given dictionary file
        /// </summary>
        /// <param name="dictionaryFileName">dictionary file (throws if not exists or empty)</param>
        /// <param name="start">word to start from (throws if not in dictionary)</param>
        /// <param name="end">word to end at (throws if not in dictionary)</param> 
        public Solver(string dictionaryFileName, string start, string end)
        {
            // check file exists etc
            if (!System.IO.File.Exists(dictionaryFileName))
            {
                throw new DictionaryFileNotExistsException($"Dictionary file '{dictionaryFileName}' not found");
            }

            Init(File.ReadAllLines(dictionaryFileName), start, end);
        }


        /// <summary>
        /// internal initialiser, sets up various structures and tries to find solutions
        /// </summary>
        /// <param name="dictionary">string array of unfiltered words</param>
        /// <param name="start">starting word</param>
        /// <param name="end">end word</param>
        private void Init(string[] dictionary, string start, string end)
        {
            StartWord = start;
            EndWord = end;
            ValidateParams();

            // trim dictionary to only hold right size words first, cut out any invalid words
            _validWordList = new HashSet<string>(dictionary.Where(x => x.Length == StartWord.Length && x.All(char.IsLetter)).ToArray());
            ValidateDictionary();

            CheckWordsExist();

            _bfsDepth = 0;
            _depthDictionary = new Dictionary<string, int>();
            _previouslyCheckedWords = new HashSet<string>();

            FindSolutions();
        }


        /// <summary>
        /// return the shortest solution (an empty solution if no solution found)
        /// </summary>
        /// <returns></returns>
        private Solution GetShortestSolution()
        {
            if (_shortestSolution == null)
            {
                // get any solution with lowest count (there may be more than 1 solution)
                if (Solutions.Count > 0)
                {
                    _shortestSolution = Solutions.OrderBy(item => item.Count).First();
                }
                else
                {
                    _shortestSolution = new Solution(); // somewhat rubbish way of indicating no solution
                }
            }
            return _shortestSolution;
        }


        /// <summary>
        /// return number of steps in shortest solution (implicity 0 if no solution)
        /// </summary>
        /// <returns></returns>
        private int GetShortestSolutionSteps()
        {
            // the number of steps is the count of words in the solution, minus 1 (the start word is in the solution)
            return Math.Max(ShortestSolution.Count - 1, 0);
        }


        /// <summary>
        /// given our setup, try and calculate solutions for the scenario 
        /// </summary>
        private void FindSolutions()
        {

            // first, make a 'depthDictionary' with a sense of how far from the startWord each entry is
            BuildDepthDictionary();
            if (_bfsDepth == 0) { return; } // safety check, no endWord found

            // TODO: is there a better way to indiscriminately iterate & update through a dictionary?
            var allWords = new List<string>(_depthDictionary.Keys);
            foreach (var word in allWords)
            {
                // the depthDictionary is tracking the distance (depth?) _from_ the startword, but now we
                // want to turn that around so we're tracking the 'normalised' distance _to_ endword
                _depthDictionary[word] = _bfsDepth - 1 - _depthDictionary[word];
            }

            // this feels like a potential waste, but I can't see any way of making use of it. It's working-store really
            _previouslyCheckedWords.Clear();

            // the initial call looks a bit odd because the method is usually told which word to start with, and
            // which distance to use, rather than using the fields StartWord and _bfsDepth
            WordDepthSolver(StartWord, _bfsDepth, new Solution());

        }



        /// <summary>
        /// Branch out from StartWord, iterating 'adjacent' words in the dictionary, chasing them until we get to the EndWord. 
        /// As we go, track the measure of each word's 'distance' from StartWord.
        /// </summary>
        private void BuildDepthDictionary()
        {
            _bfsDepth = 0;
            var bfsQueue = new Queue<Tuple<string, int>>();

            // set up the queue;
            // StartWord is naturally '0' distance from the startword
            bfsQueue.Enqueue(new Tuple<string, int>(StartWord, 0));
            // and we consider it one of our 'previously checked words'
            _previouslyCheckedWords.Clear(); // just to be sure
            _previouslyCheckedWords.Add(StartWord);

            // so, while we have something in the queue
            while (bfsQueue.Count > 0)
            {
                // get this word and it's distance from the start
                var member = bfsQueue.Dequeue();
                var word = member.Item1;
                var thisDistance = member.Item2;

                // and store it in our distance dictionary
                _depthDictionary[word] = thisDistance;

                // if we've hit the target word
                if (word == EndWord)
                {
                    // signal back we're done, sending the final bfs-derived distance (depth)
                    _bfsDepth = thisDistance + 1;
                    return;
                }

                // otherwise, we go through our list of valid words looking for words that are a single letter different
                // from this word, and that we haven't already checked...
                foreach (string newWord in _validWordList)
                {
                    if (newWord.HasSingleCharDifferent(word) && !_previouslyCheckedWords.Contains(newWord))
                    {
                        // yep, this is of interest; add it to the queue, with the appropriate new distance from the startword
                        bfsQueue.Enqueue(new Tuple<string, int>(newWord, thisDistance + 1));
                        // and add it to our tracking list so we don't accidentally repeat ourselves
                        _previouslyCheckedWords.Add(newWord);
                    }
                }

                // this will track round, working the queue until it is exhausted.
                // along the way, we should have already jumped out having reached the end word (target)
            }
        }




        /// <summary>
        /// recursively search our dictionary for a given 'currentWord', with a given 'starting distance' (depth, or cost if you like),
        /// building up a solution in 'currentSolution'.
        /// When a successful solution is found, its added to the list of solutions.
        /// </summary>
        /// <param name="currentWord">the word we are currently solving from</param>
        /// <param name="currentDepth">the track of depth from our endword</param>
        /// <param name="currentSolution">the in-progress solution (the list of words so far)</param>
        private void WordDepthSolver(
            string currentWord,
            int currentDepth,
            Solution currentSolution)
        {

            // track what we're currently working on
            _previouslyCheckedWords.Add(currentWord);
            currentSolution.Add(currentWord);

            // have we found the word?
            if (currentWord == EndWord)
            {
                // add the current working solution to the overall list of solutions
                var thisList = new Solution();
                thisList.AddRange(currentSolution);
                Solutions.Add(thisList);
            }
            // otherwise, are we still within depth range? 
            else if (currentDepth > 0 && _depthDictionary[currentWord] < currentDepth)
            {
                // If so, find another 'adjacent' word to step to - assuming we calculated the depth for it
                // (words that are a single letter from the currentWord, that we haven't already looked at...)
                foreach (var newWord in _validWordList)
                {
                    if (_depthDictionary.Keys.Contains(newWord) && newWord.HasSingleCharDifferent(currentWord) && !_previouslyCheckedWords.Contains(newWord))
                    {
                        // ok, we haven't tried to solve for this newWord yet...
                        WordDepthSolver(
                            newWord, // new word to solve for
                            currentDepth - 1, // because this next iteration will be 1 single word 'further' from the currentWord
                            currentSolution);
                    }
                }
            }

            // remove this (last word) from our current solution
            currentSolution.RemoveAt(currentSolution.Count - 1);
            // don't forget to take this back off our visited list, ready for next use
            _previouslyCheckedWords.Remove(currentWord);
        }


        #region Validation Methods

        /// <summary>
        /// Validate that StartWord!=EndWord, StartWord & EndWord same length
        /// </summary>
        private void ValidateParams()
        {
            if (String.IsNullOrEmpty(StartWord))
            {
                throw new InvalidWordException("Start word is empty/invalid");
            }
            if (String.IsNullOrEmpty(EndWord))
            {
                throw new InvalidWordException("End word is empty/invalid");
            }
            if (StartWord == EndWord)
            {
                throw new WordsAreSameException($"Start and end words must be different! '{StartWord}'");
            }
            if (StartWord.Length != EndWord.Length)
            {
                throw new WordLengthsNotMatchException($"Start and End words must be of equal length; {StartWord}, {StartWord.Length} and {EndWord}, {EndWord.Length}");
            }
        }

        /// <summary>
        /// Validate there is something in the ValidWordList dictionary
        /// </summary>
        private void ValidateDictionary()
        {
            if (_validWordList.Count < 1)
            {
                throw new DictionaryNoValidWordsException($"Dictionary contains no valid words for this wordlength {StartWord.Length}");
            }
        }

        /// <summary>
        /// Check that both StartWord and EndWord are in the ValidWordDictionary
        /// </summary>
        private void CheckWordsExist()
        {

            if (!_validWordList.Contains(StartWord))
            {
                throw new WordNotInDictionaryException($"{StartWord} not found in dictionary");
            }

            if (!_validWordList.Contains(EndWord))
            {
                throw new WordNotInDictionaryException($"{EndWord} not found in dictionary");
            }

        }

        #endregion

    }
}
