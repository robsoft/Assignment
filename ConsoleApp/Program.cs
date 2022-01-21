using System;
using Library;

namespace ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            if (!CheckArgs(args)) { return; }

            // we haven't validated the args because the Solver class will do that for us
            try
            {
                var scenario = new Solver(
                    args[0], // dictionary
                    args[1], // start
                    args[2] // end
                    );

                var solution = scenario.ShortestSolution;

                if (solution != default)
                {
                    SaveSolution(solution, args[3]);
                    // bit hacky, we want to count the steps, not the number of words in the list
                    Console.WriteLine($"{solution.Count - 1} steps");
                }
                else
                {
                    if (System.IO.File.Exists(args[3]))
                    {
                        System.IO.File.Delete(args[3]);
                    }
                    Console.WriteLine("No solution found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        static void SaveSolution(Solution solution, string filename)
        {
            // write the solution to a file
            try
            {
                System.IO.File.WriteAllLines(filename, solution);
                Console.WriteLine($"Solution saved to {filename}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write output file ({ex.Message})");
            }
        }


        // the brief said to assume the start and end words were in the dictionary, although
        // for completeness the Solver class will check all this - just some friendly message
        // if the args look 'wrong'
        static bool CheckArgs(string[] args)
        {
            bool result = (args.Length == 4);

            if (!result)
            {
                Console.WriteLine("WordSolver - BP test project. (See brief for details)\n");
                Console.WriteLine("requires 4 arguments;\n\tdictionary, startword, endword, outputfile\n");
                Console.WriteLine("eg;\n\twords-english.txt spin spot resultfile.txt\n");
                Console.WriteLine("dictionary must exist.");
                Console.WriteLine("outputfile will be overwritten without prompting.");
                Console.WriteLine("startword and endword must be contained in dictionary\n");
            }
            return result;
        }

    }
}
