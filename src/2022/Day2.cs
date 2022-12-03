// using op = opponent :)

namespace advent_of_code.y2022 {
    class Day2 : AdventDay {
        public override int DayNumber => 2;

        public override void SolvePart1(string[] inputLines) {
            int totalScore = 0;
            int errors = 0;
            foreach (var parsed in IterateConditions(inputLines)) {
                var myChoice = GetMyMoveFromSymbol(parsed.me);
                var opChoice = GetOpMoveFromSymbol(parsed.opponent);
                var gameResult = RockPaperScissors.GetResult(myChoice, opChoice);
                totalScore += gameResult.GetScore() + myChoice.GetScore();
                Console.WriteLine("my:{0} (+{1}) vs op:{2}, game result: {3}", myChoice, myChoice.GetScore(), opChoice, gameResult);
            }
            Console.WriteLine("\n----\nfinal score: {0}   ({1} lines, {2} errors)", totalScore, inputLines.Length, errors);
        }

        public override void SolvePart2(string[] inputLines) {
            int total = 0;
            foreach (var parsed in IterateConditions(inputLines)) {
                var me = parsed.me;
                var targetResult = GetDesiredResult(me);
                var opMove = GetOpMoveFromSymbol(parsed.opponent);
                var myMove = RockPaperScissors.GetMyMoveFromResult(targetResult, opMove);
                var score = targetResult.GetScore() + myMove.GetScore();
                total += score;

                Console.WriteLine($"op:{opMove}, I sholud {targetResult}, result: {(int)targetResult} + {myMove.GetScore()} = {score}");
            }

            Console.WriteLine("\n----\ntotal score: {0}", total);
        }

        static IEnumerable<(char opponent, char me)> IterateConditions(string[] inputLines) {
            for (int i = 0; i < inputLines.Length; i++) {
                var line = inputLines[i];
                if (TryParseLine(line, out var parsed)) {
                    yield return parsed;
                }
            }
        }

        static bool TryParseLine(string line, out (char opponent, char me) result) {
            var splitted = line.Split(' ');
            bool success = false;
            result = default;
            if (splitted.Length == 2) {
                result = (splitted[0].First(), splitted[1].First());
                success = true;
            }
            return success;
        }

        static RockPaperScissors.Move GetMyMoveFromSymbol(char symbol) {
            return RockPaperScissors.LoopMove(symbol - 'X');
        }

        static RockPaperScissors.Move GetOpMoveFromSymbol(char symbol) {
            return RockPaperScissors.LoopMove(symbol - 'A');
        }

        static RockPaperScissors.Result GetDesiredResult(char symbol) {
            return (RockPaperScissors.Result)(symbol - 'X');
        }
    }

    public static class RockPaperScissors {
        public static int GetScore(this Move move) {
            return (int)move + 1;
        }

        public static int GetScore(this Result result) {
            return (int)result * 3;
        }

        public static Move GetMyMoveFromResult(Result targetResult, Move opponentsChoice) {
            return targetResult switch {
                Result.Win => GetNext(opponentsChoice),
                Result.Lose => GetPrev(opponentsChoice),
                _ => opponentsChoice
            };
        }

        public static Result GetResult(Move myChoice, Move opponentsChoice) {
            var myScore = Result.Win;

            // same:
            if (myChoice == opponentsChoice) myScore = Result.Draw;
            // opponent has next figure, lose:
            else if (GetNext(myChoice) == opponentsChoice) myScore = Result.Lose;
            //otherwise win

            return myScore;
        }

        public static Move GetNext(Move current) {
            return LoopMove((int)current + 1);
        }

        public static Move GetPrev(Move current) {
            return LoopMove((int)current - 1);
        }

        public static Move LoopMove(int moveAsInt) {
            const int CLAMPER = (int)Move.Scissors + 1;
            while (moveAsInt >= CLAMPER) moveAsInt -= CLAMPER;
            while (moveAsInt < 0) moveAsInt += CLAMPER;
            return (Move)moveAsInt;
        }

        public enum Move {
            Rock,
            Paper,
            Scissors
        }

        public enum Result {
            Lose,
            Draw,
            Win
        }
    }
}
