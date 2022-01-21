# Assignment  (BP)


Visual Studio For Mac (8.10.16), .Net 5.0 solution.  

There are three projects; a library containing the 'solver' class, a console app to call the library with command-line parameters, and a set of unit tests.

## ConsoleApp   
This is very simple and self-explanatory, it is really just an interface to the Solver class. Validation is performed by the class itself so validation errors are raised as exceptions and the console app simply traps them and displays them to the user.


## Tests
uses Fluent.Assertions  
I hadn't used Fluent Assertions before, only normal NUnit-type Asserts, but I thought it would be nice to try and incorporate something a little more modern. The use is superficial though, other than the readability there isn't any real use of FluentAssertions' additional capabilities here.


## Library
This is a simple 'monolith' type class.

There is no evidence here that I understand DI, nor even any use of interfaces; I could have contrived a logging interface and DI'd a logger into the Solver class but it seemed like making additional work and complexity for the sake of proving a point.


## General Notes

Once everything is validated, and the dictionary has been reduced to just the suitable words, the process is simple enough;
1) make a depth-search list of the 'distances' (iteration steps) from each suitable to the end word,  
2) use this depth list to find solutions from the start word to the end word


There are a number of areas that I would dig into if this was production-intended, eg using a hashset to hold the wordlist and some working store.
The assumption is that for general cases, searching this would be quicker
than simple string matching in a list of strings (or an array etc), but if performance was an issue then I would look at how hashset
was being implemented. Not because I think I could do better (!) but simply given that the current brief revolves around 4-letter words,
there may be simpler, more efficient ways of storing and searching such short words than using a hashset. This could be explored with System.Diagnostics.StopWatch and something like Garbage Collection GetTotalMemory, I imagine.

Two of the side effects of using hashset are that the list is no longer ordered, and any duplicates that were in the dictionary are lost.
Neither of these side effects will harm the solution produced, but in production code I would ensure this was documented somewhere as part of
'how the sausage machine works'.  

After the depth-list is created, I junk the list of visited words before I start searching for solutions. This feels like I'm throwing away
something I could use, but it wasn't obvious to me how I could improve things by keeping it.

I was briefly concerned about whether or not the alphasorted nature (or otherwise) of the dictionary mattered and realised that the brief didn't
address this. There's potentially more than one solution to get between 2 words with the same number of steps, and there is no mention
of whether or not there is criteria to prefer a solution over another one. 

At the start of the process I filter the dictionary for invalid-length words; this takes time and is a step you might
consider junking if the word list wasn't going to be reused in the same session. My gut feeling is making the dictionary shorter as
soon as possible is the right thing to do but obviously, as above, I would do some metrics on that before making a 'production assertion'.
This is an area where you would use your understanding of the problem-domain to shape the solution (if the majority of the dictionary words
are the same length as the search word, you might not bother to filter to dictionary, etc).


## Improvements

The single big 'Solver' class is not especially elegant and I'm sure if I attempted the assignment again I would approach it differently, c
ertainly I would be more aware of how it needs to be ordered to assist in accessible unit testing. From a maintainable point of view;
I tried to explain the approach and each individual step without cluttering it with pointless comments, and the key limitations and
'gotchas' in the code are flagged up.