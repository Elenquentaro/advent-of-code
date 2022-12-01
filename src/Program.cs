namespace advent_of_code {
    internal class Program {
        static void Main(string[] args) {
            string inputPath = ReadInput("path to your input") ?? "resources/input/2022-1.txt";
            string[] lines = ReadInputFromFile(inputPath);
            var currentDay = AdventCalendar.GetCurrentDay();
            currentDay.SolvePart1(lines);
            currentDay.SolvePart2(lines);
        }

        static string? ReadInput(string title) {
            Console.Write("{0}: ", title);
            return Console.ReadLine();
        }

        private static string[] ReadInputFromFile(string path) {
            path = HandlePath(path);
            var lines = File.ReadAllLines(path);
            return lines;
        }

        static string HandlePath(string inputPath) {
            return inputPath.Trim('"');
        }
    }
}
