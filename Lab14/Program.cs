using System;
using System.Linq;
using ASD;

namespace ASD_lab14
{
    public class Program
    {

        public class Stage1TestCase : TestCase
        {
            protected readonly string text;
            protected readonly string longestSubstring;
            protected readonly bool checkAssignment;
            protected int length
            {
                get
                {
                    return this.longestSubstring.Length;
                }
            }

            protected (int length, string longestSubstring) result;

            public Stage1TestCase(string text, string longestSubstring, bool checkAssignment, double timeLimit, string description) : base(timeLimit, null, description)
            {
                this.text = text;
                this.longestSubstring = longestSubstring;
                this.checkAssignment = checkAssignment;
            }

            protected override (Result resultCode, string message) VerifyTestCase(object settings)
            {
                var (code, msg) = CheckSolution(result.length, result.longestSubstring);
                return (code, $"{msg} [{Description}]");
            }

            private (Result resultCode, string message) CheckSolution(int length, string longestSubstring)
            {
                if (this.length != length)
                {
                    return (Result.WrongResult, $"Zwrócono złą długość. Jest {length}, powinno być {this.length}");
                }

                if (!this.checkAssignment)
                    return OkResult("OK");

                if (longestSubstring is null)
                {
                    return (Result.WrongResult, $"Zwrócono dobrą długość, nie zwrócono fragmentu.");
                }

                if (length != longestSubstring.Length)
                {
                    return (Result.WrongResult, $"Zwrócono dobrą długość, ale zwrócony fragment nie występuje w tekście.");
                }

                var firstOccurrence = this.text.IndexOf(longestSubstring);
                var lastOccurrence = this.text.LastIndexOf(longestSubstring);
                if (firstOccurrence == -1)
                {
                    return (Result.WrongResult, $"Zwrócono dobrą długość, ale zwrócony fragment nie występuje w tekście.");
                }

                if (lastOccurrence == firstOccurrence)
                {
                    return (Result.WrongResult, $"Zwrócono dobrą długość, ale zwrócony fragment występuje w tekście tylko raz.");
                }

                if (lastOccurrence - firstOccurrence < longestSubstring.Length)
                {
                    return (Result.WrongResult, $"Zwrócono dobrą długość, ale wystąpienia tego fragmentu na siebie nachodzą.");
                }

                return OkResult("OK");
            }

            public (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime:#0.00}s");

            protected override void PerformTestCase(object prototypeObject)
            {
                this.result = ((Substrings)prototypeObject).StageOne(this.text);
            }
        }
        public class Stage2TestCase : TestCase
        {
            protected readonly string x;
            protected readonly string y;
            protected readonly string longestSubstring;
            protected readonly bool checkAssignment;

            protected int length
            {
                get
                {
                    return this.longestSubstring.Length;
                }
            }

            protected (int length, string longestSubstring) result;

            public Stage2TestCase(string x, string y, string longestSubstring, bool checkAssignment, double timeLimit, string description) : base(timeLimit, null, description)
            {
                this.x = x;
                this.y = y;
                this.longestSubstring = longestSubstring;
                this.checkAssignment = checkAssignment;
            }

            protected override (Result resultCode, string message) VerifyTestCase(object settings)
            {
                var (code, msg) = CheckSolution(result.length, result.longestSubstring);
                return (code, $"{msg} [{Description}]");
            }

            private (Result resultCode, string message) CheckSolution(int length, string longestSubstring)
            {
                if (this.length != length)
                {
                    return (Result.WrongResult, $"Zwrócono złą długość. Jest {length}, powinno być {this.length}.");
                }

                if (longestSubstring is null)
                {
                    return (Result.WrongResult, $"Zwrócono dobrą długość, nie zwrócono fragmentu.");
                }


                if (length != longestSubstring.Length)
                {
                    return (Result.WrongResult, $"Zwrócono dobrą długość, ale zwrócony fragment jest innej długości.");
                }

                var xOccurrence = this.x.IndexOf(longestSubstring);
                var yOccurrence = this.y.IndexOf(longestSubstring);

                if (xOccurrence == -1 || yOccurrence == -1)
                {
                    return (Result.WrongResult, $"Zwrócono dobrą długość, ale zwrócony fragment nie występuje w przynajmniej jednym z tekstów.");
                }

                return OkResult("OK");
            }

            public (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime:#0.00}s");

            protected override void PerformTestCase(object prototypeObject)
            {
                this.result = ((Substrings)prototypeObject).StageTwo(this.x, this.y);
            }
        }


        public class Lab14Tests : TestModule
        {
            TestSet Stage1a = new TestSet(prototypeObject: new Substrings(), description: "Etap 1, zwrócenie długości najdłuższego powtarzającego się fragmentu", settings: true);
            TestSet Stage1b = new TestSet(prototypeObject: new Substrings(), description: "Etap 1, zwrócenie najdłuższego powtarzającego się fragmentu", settings: true);
            TestSet Stage2a = new TestSet(prototypeObject: new Substrings(), description: "Etap 2, zwrócenie długości najdłuższego powtarzającego się fragmentu (w dwóch stringach)", settings: true);
            TestSet Stage2b = new TestSet(prototypeObject: new Substrings(), description: "Etap 2, zwrócenie najdłuższego powtarzającego się fragmentu (w dwóch stringach)", settings: true);

            public override void PrepareTestSets()
            {
                TestSets["Stage1a"] = Stage1a;
                TestSets["Stage1b"] = Stage1b;
                TestSets["Stage2a"] = Stage2a;
                TestSets["Stage2b"] = Stage2b;

                Prepare();
            }

            private void AddStage1(string text, string longestSubstring, double timeLimit, string description)
            {
                Stage1a.TestCases.Add(
                    new Stage1TestCase(text: text, longestSubstring: longestSubstring, checkAssignment: false, timeLimit: timeLimit, description: description)
                );
                Stage1b.TestCases.Add(
                    new Stage1TestCase(text: text, longestSubstring: longestSubstring, checkAssignment: true, timeLimit: timeLimit, description: description)
                );
            }

            private void AddStage2(string x, string y, string longestSubstring, double timeLimit, string description)
            {
                Stage2a.TestCases.Add(
                    new Stage2TestCase(x: x, y: y, longestSubstring: longestSubstring, checkAssignment: false, timeLimit: timeLimit, description: description)
                );
                Stage2b.TestCases.Add(
                    new Stage2TestCase(x: x, y: y, longestSubstring: longestSubstring, checkAssignment: true, timeLimit: timeLimit, description: description)
                );
            }


            public bool AreAllTestsPassed()
            {
                return Stage1a.FailedCount + Stage1b.FailedCount + Stage2a.FailedCount + Stage2b.FailedCount == 0;
            }

            private void Prepare()
            {
                AddStage1(text: "abba", longestSubstring: "a", timeLimit: 2.5, description: "abba");
                AddStage1(text: "aaaaaaaaaaaaaaaaaaaaaaaa", longestSubstring: "aaaaaaaaaaaa", timeLimit: 2.5, description: "wielokrotnie ten sam znak, parzyście");
                AddStage1(text: "aaaaaaaaaaaaaaaaaaaaaaaaa", longestSubstring: "aaaaaaaaaaaa", timeLimit: 2.5, description: "wielokrotnie ten sam znak, nieparzyście");
                AddStage1(text: "abcdefghijklmnopqrs", longestSubstring: "", timeLimit: 2.5, description: "nic się nie powtarza");
                AddStage1(text: this.GetRandomInput("ab", 100, 42), longestSubstring: "babaabaaababbaa", timeLimit: 2.5, description: "100 losowych znaków (mały zbiór)");
                AddStage1(text: this.GetRandomInput("ab", 1000, 42), longestSubstring: "babaababbababbbaba", timeLimit: 2.5, description: "1000 losowych znaków (mały zbiór)");
                AddStage1(text: this.GetRandomInput("abcdefghijk", 100, 42), longestSubstring: "fai", timeLimit: 2.5, description: "100 losowych znaków (duży zbiór)");
                AddStage1(text: this.GetRandomInput("abcdefghijk", 1000, 42), longestSubstring: "ajbif", timeLimit: 2.5, description: "1000 losowych znaków (duży zbiór)");
                AddStage1(text: "ababab", longestSubstring: "ab", timeLimit: 2.5, description: "Custom test 1");

                AddStage2(y: "abc", x: "ajhdvjhgywqvhgcvjafetyfqniunefivqnfvabcashdbajbiufvbqvfkjqvfvqf", longestSubstring: "abc", timeLimit: 2.5, description: "jeden jest podzbiorem drugiego");
                AddStage2(y: this.GetRandomInput("ab", 100, 44), x: this.GetRandomInput("cd", 100, 44), longestSubstring: "", timeLimit: 2.5, description: "nic nie się pokrywa");
                AddStage2(y: this.GetRandomInput("ab", 100, 44), x: this.GetRandomInput("ab", 100, 44), longestSubstring: this.GetRandomInput("ab", 100, 44), timeLimit: 2.5, description: "to samo dwa razy");
                AddStage2(y: this.GetRandomInput("ab", 100, 44), x: this.GetRandomInput("ab", 100, 43), longestSubstring: "aabbbabbaababbaa", timeLimit: 2.5, description: "100 losowych znaków (mały zbiór)");
                AddStage2(y: this.GetRandomInput("ab", 1000, 44), x: this.GetRandomInput("ab", 1000, 43), longestSubstring: "aaaaabaaaababbbaaaaba", timeLimit: 2.5, description: "1000 losowych znaków (mały zbiór)");
                AddStage2(y: this.GetRandomInput("abcdefghijk", 100, 44), x: this.GetRandomInput("abcdefghijk", 100, 43), longestSubstring: "adbk", timeLimit: 2.5, description: "100 losowych znaków (duży zbiór)");
                AddStage2(y: this.GetRandomInput("abcdefghijk", 1000, 44), x: this.GetRandomInput("abcdefghijk", 1000, 43), longestSubstring: "hjidba", timeLimit: 2.5, description: "1000 losowych znaków (duży zbiór)");
            }

            private string GetRandomInput(string letters, int length, int seed)
            {
                Random rng = new Random(seed);
                return new string(Enumerable.Repeat(letters, length)
                    .Select(s => s[rng.Next(s.Length)]).ToArray());
            }

        }

        static void Main(string[] args)
        {
            var tests = new Lab14Tests();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }
        }
    }
}
