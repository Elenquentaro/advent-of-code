using advent_of_code.y2022;

namespace advent_of_code {
    internal static class AdventCalendar {
        public static AdventDay GetCurrentDay() {
            return new Day7();
        }

        public static string GetDefaultPathForDay(AdventDay day) {
            return string.Format("resources/input/2022-{0}.txt", day.DayNumber);
        }
    }
}
