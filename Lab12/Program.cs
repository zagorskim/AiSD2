using System;
using System.Linq;
using System.Collections.Generic;

namespace ASD
{

    public class Stage1TestCase : TestCase
    {
        protected readonly (double x, double y)[] expectedResult;

        protected readonly (double x, double y)[] dogs;
        protected readonly (float A, float B, float C) shed;

        protected (double x, double y)[] result;

        public Stage1TestCase((double x, double y)[] dogs, (float A, float B, float C) shed, (double x, double y)[] expectedResult, double timeLimit, string description) : base(timeLimit, null, description)
        {
            this.dogs = dogs;
            this.shed = shed;
            this.expectedResult = expectedResult;
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((SheepHeard)prototypeObject).SafePolygon(dogs, shed);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            var (code, msg) = checkSolution(result);
            return (code, $"{msg} [{this.Description}]");
        }

        private (Result resultCode, string message) checkSolution((double x, double y)[] returnedResult)
        {
            if (returnedResult == null)
            {
                if (expectedResult == null)
                    return OkResult("OK");
                return (Result.WrongResult, $"Zwrócono null w przypadku kiedy bezpieczne miejsce istenieje");
            }

            if (returnedResult.Length != expectedResult.Length)
            {
                return (Result.WrongResult, $"Zwrócona tablica ma {returnedResult.Length} elementów, powinno być {expectedResult.Length}");
            }

            if (!expectedResult.ToHashSet().IsSubsetOf(returnedResult.Select(x => (Math.Round(x.x, 5), Math.Round(x.y, 5))).ToHashSet()))
            {
                return (Result.WrongResult, $"Zwrócona tablica zawiera błędne elementy");
            }

            return OkResult("OK");
        }

        public (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime.ToString("#0.00")}s");
    }


    public class Stage2TestCase : TestCase
    {
        protected readonly int expectedResult;

        protected readonly (double x, double y)[] sheeps;
        protected readonly (double x, double y)[] dogs;
        protected readonly (float A, float B, float C) shed;

        protected int result;

        public Stage2TestCase((double x, double y)[] sheeps, (double x, double y)[] dogs, (float A, float B, float C) shed, int expectedResult, double timeLimit, string description) : base(timeLimit, null, description)
        {
            this.sheeps = sheeps;
            this.dogs = dogs;
            this.shed = shed;
            this.expectedResult = expectedResult;
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((SheepHeard)prototypeObject).CheckCoverage(sheeps, dogs, shed);
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            var (code, msg) = checkSolution(result);
            return (code, $"{msg} [{this.Description}]");
        }

        private (Result resultCode, string message) checkSolution(int returnedResult)
        {
            if (expectedResult != returnedResult)
            {
                return (Result.WrongResult, $"Zwrócono {returnedResult}, powinno być {expectedResult}");
            }

            return OkResult("OK");
        }

        public (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime.ToString("#0.00")}s");
    }


    public class Lab12Tests : TestModule
    {
        TestSet Stage1 = new TestSet(prototypeObject: new SheepHeard(), description: "Etap 1, bezpieczny wielokat", settings: true);
        TestSet Stage2 = new TestSet(prototypeObject: new SheepHeard(), description: "Etap 2, liczba bezpiecznych owiec", settings: true);

        public override void PrepareTestSets()
        {
            TestSets["Stage1"] = Stage1;
            TestSets["Stage2"] = Stage2;

            prepare();
        }




        private void prepare()
        {
            (double x, double y)[] sheeps;
            (double x, double y)[] dogs;
            (float A, float B, float C) shed;
            int expected_result;
            (double x, double y)[] expected_polygon;

            sheeps = new (double x, double y)[] { (0, 0) };
            dogs = new (double x, double y)[] { };
            shed = (1, 0, 0);
            expected_result = 0;
            expected_polygon = null;

            Stage1.TestCases.Add(new Stage1TestCase(dogs, shed, expected_polygon, timeLimit: 10, description: "No dogs"));
            Stage2.TestCases.Add(new Stage2TestCase(sheeps, dogs, shed, expected_result, timeLimit: 10, description: "No dogs"));

            sheeps = new (double x, double y)[] { (0, 0) };
            dogs = new (double x, double y)[] { (0, 1) };
            shed = (1, 0, 0);
            expected_result = 0;
            expected_polygon = null;

            Stage1.TestCases.Add(new Stage1TestCase(dogs, shed, expected_polygon, timeLimit: 10, description: "One dog"));
            Stage2.TestCases.Add(new Stage2TestCase(sheeps, dogs, shed, expected_result, timeLimit: 10, description: "One dog"));

            sheeps = new (double x, double y)[] { (-1, 0), (2, 0), (0.5, 0.5) };
            dogs = new (double x, double y)[] { (1, 0), (1, 1) };
            shed = (1, 0, 0);
            expected_result = 1;
            expected_polygon = new (double x, double y)[] { (0, 0), (0, 1), (1, 1), (1, 0) };

            Stage1.TestCases.Add(new Stage1TestCase(dogs, shed, expected_polygon, timeLimit: 10, description: "Two dogs - square"));
            Stage2.TestCases.Add(new Stage2TestCase(sheeps, dogs, shed, expected_result, timeLimit: 10, description: "Two dogs - square"));

            sheeps = new (double x, double y)[] { (-1, 0), (2, 0), (0.5, 0.5), (1, 1), (1.5, 0.5) };
            dogs = new (double x, double y)[] { (1, 0), (2, 2) };
            shed = (1, 0, 0);
            expected_result = 2;
            expected_polygon = new (double x, double y)[] { (0, 0), (0, 2), (2, 2), (1, 0) };

            Stage1.TestCases.Add(new Stage1TestCase(dogs, shed, expected_polygon, timeLimit: 10, description: "Two dogs - trapezoid"));
            Stage2.TestCases.Add(new Stage2TestCase(sheeps, dogs, shed, expected_result, timeLimit: 10, description: "Two dogs - trapezoid"));


            sheeps = new (double x, double y)[] { (1, 1), (1, 2), (0, 3), (3, 0), (3, 2), (3, 3), (4, 3), (1, 6) };
            dogs = new (double x, double y)[] { (1, 0), (0, 1), (1, 3), (2, 2), (3, 4), (4, 1) };
            shed = (1, 0, 1);
            expected_result = 5;
            expected_polygon = new (double x, double y)[] { (-1, 0), (-1, 4), (3, 4), (4, 1), (1, 0) };

            Stage1.TestCases.Add(new Stage1TestCase(dogs, shed, expected_polygon, timeLimit: 10, description: "Simple example"));
            Stage2.TestCases.Add(new Stage2TestCase(sheeps, dogs, shed, expected_result, timeLimit: 10, description: "Simple example"));


            sheeps = new (double x, double y)[] { (0, 0), (0, 4), (2, 1) };
            dogs = new (double x, double y)[] { (1, 0), (0, 1), (1, 3), (2, 2), (3, 4), (4, 3) };
            shed = (1, 0, 1);
            expected_result = 3;
            expected_polygon = new (double x, double y)[] { (-1, 0), (-1, 4), (3, 4), (4, 3), (1, 0) };

            Stage1.TestCases.Add(new Stage1TestCase(dogs, shed, expected_polygon, timeLimit: 10, description: "Sheeps on egdes"));
            Stage2.TestCases.Add(new Stage2TestCase(sheeps, dogs, shed, expected_result, timeLimit: 10, description: "Sheeps on egdes"));

            sheeps = new (double x, double y)[] { (0, 0), (1, 1), (1, 2), (0, 3), (3, 0), (3, 2), (3, 3), (4, 3) };
            dogs = new (double x, double y)[] { (1, 0), (0, 1), (1, 3), (2, 2), (3, 4), (4, 1) };
            shed = (0, 1, 1);
            expected_result = 6;
            expected_polygon = new (double x, double y)[] { (0, 1), (1, 3), (3, 4), (4, 1), (4, -1), (0, -1) };

            Stage1.TestCases.Add(new Stage1TestCase(dogs, shed, expected_polygon, timeLimit: 10, description: "Horizontal line"));
            Stage2.TestCases.Add(new Stage2TestCase(sheeps, dogs, shed, expected_result, timeLimit: 10, description: "Horizontal line"));

            sheeps = new (double x, double y)[] { (0, 0), (1, 1), (1, 2), (0, 3), (3, 0), (3, 2), (3, 3), (4, 3) };
            dogs = new (double x, double y)[] { (1, 0), (0, 1), (1, 3), (2, 2), (3, 4), (4, 1) };
            shed = (1, 1, 1);
            expected_result = 6;
            expected_polygon = new (double x, double y)[] { (-1.5, 0.5), (1, 3), (3, 4), (4, 1), (1, -2) };

            Stage1.TestCases.Add(new Stage1TestCase(dogs, shed, expected_polygon, timeLimit: 10, description: "Diagonal line"));
            Stage2.TestCases.Add(new Stage2TestCase(sheeps, dogs, shed, expected_result, timeLimit: 10, description: "Diagonal line"));

            sheeps = new (double x, double y)[] { (-0.8, 1.8), (-3.4, 3.5), (-5.2, 1.0), (-6.2, 3.9), (3.8, 9.8), (-5.8, 1.9), (-9.4, 2.4), (-8.6, 8.0), (-0.3, 1.6), (-8.3, -7.8), (6.2, -8.4), (-3.7, -4.7), (-0.2, -1.0), (-4.8, -3.1), (9.1, 7.8), (4.6, -6.6), (3.1, 0.7), (-7.9, 4.5), (9.6, -8.2), (4.8, -6.8), (4.0, 3.7), (-9.8, -6.5), (4.2, 0.6), (-4.9, 7.9), (7.1, 7.0), (-6.3, -7.4), (6.6, 7.3), (0.1, -7.2), (3.0, -6.0), (3.4, 5.6), (5.7, 4.4), (-1.1, 1.1), (7.5, -3.2), (-5.5, -9.9), (6.5, -5.4), (1.3, 1.2), (3.6, -5.7), (-6.7, 6.4), (0.4, 0.8), (6.3, 5.9), (-6.4, -0.5), (-9.0, -8.8), (-2.8, -3.5), (0.9, -4.5), (-2.3, 5.8), (-4.3, 7.4), (-2.9, -5.6), (9.4, -8.0), (-9.6, -6.1), (-1.9, -2.1), (8.1, 7.6), (6.8, 2.3), (-3.9, -9.7), (-5.3, -1.2), (8.4, 8.7), (-3.3, 2.6), (-1.4, 5.3), (-0.9, -9.5), (-1.8, 6.0), (-1.5, -7.3), (2.1, -0.6), (0.5, 4.7), (9.5, 8.6), (2.0, -4.2), (-4.4, -10.0), (7.2, 3.2), (-2.2, -2.0), (7.7, 8.2), (8.5, -2.7), (1.4, -9.1), (-7.1, 1.7), (-4.6, 4.9), (-1.7, 6.1), (5.1, -8.1), (-4.0, 8.8), (-8.7, -2.5), (0.2, -9.3), (-1.3, 0.3), (-2.6, 2.5), (-7.5, -9.2), (10.0, -2.4), (-8.5, 2.9), (-5.0, -0.7), (2.8, 2.7), (9.3, -3.6), (-8.9, -5.9), (-7.7, -7.6), (-7.0, -1.6), (6.7, -3.0), (3.3, 5.0), (-3.8, 1.5), (2.2, 5.2), (9.0, 4.3), (-4.1, 0.0), (-6.9, 5.4), (-5.1, 9.7), (4.1, 9.2), (9.9, 5.5), (-0.4, -0.1), (8.9, 8.3) };
            dogs = new (double x, double y)[] { (-3.8, 7.1), (3.4, -4.7), (9.0, -0.1), (9.4, 7.7), (7.0, -9.0), (3.7, -3.2), (1.1, -1.9), (9.8, -1.1), (-9.6, -9.6), (8.9, -1.4), (8.9, 3.4), (2.9, -3.2), (-9.3, -6.5), (8.1, -2.6), (0.5, 1.9), (-1.9, 1.2), (-6.6, 1.7), (-4.5, 8.7), (-8.7, -10.0), (9.5, 1.2), (-4.3, 2.3), (5.1, -4.8), (-6.1, 0.2), (-8.5, -9.4), (-2.1, 2.5), (6.7, 8.0), (6.3, -2.9), (1.1, 6.9), (4.4, -3.0), (9.3, 9.7), (9.2, 6.7), (8.9, -8.0), (0.7, 4.5), (7.8, 1.6), (8.2, -1.7), (7.8, 8.3), (9.1, 6.8), (9.0, -2.6), (-2.3, 4.5), (-2.0, 8.3), (-1.1, 3.8), (-8.1, -3.4), (-6.6, -0.9), (-3.3, -0.2), (9.2, -9.3), (-0.1, -5.4), (6.4, -3.6), (4.8, 9.8), (1.6, 10.0), (-7.4, -5.0), (-1.7, 0.1), (-8.7, -0.2), (6.7, -1.0), (8.4, -0.4), (7.6, 5.0), (-4.9, 0.6), (6.0, 7.1), (-6.9, 7.1), (-3.6, -6.6), (-0.6, -1.0), (9.5, 5.9), (2.9, -6.0), (7.9, -1.1), (-5.7, 9.2), (3.0, 1.9), (1.9, -8.9), (-5.4, 7.3), (5.1, -8.3), (3.1, -6.0), (4.9, -6.8), (-5.6, -2.4), (1.8, 1.2), (-0.5, -9.1), (8.4, -9.1), (8.9, 0.6), (-4.8, 5.2), (1.5, 2.0), (5.3, -2.6), (9.1, 7.5), (-8.6, -9.7), (-3.6, -2.4), (1.0, -4.3), (6.9, -5.3), (7.4, 5.0), (-6.1, 0.2), (4.2, 5.1), (-6.0, 0.5), (-10.0, 7.1), (-1.0, -5.2), (6.7, -1.8), (4.0, 3.8), (-3.6, -7.1), (-10.0, 0.5), (9.5, 5.1), (1.0, 4.1), (3.3, 6.3), (0.6, -2.0), (7.8, -8.4), (8.4, 1.9), (8.9, -1.4) };
            shed = (3, 3, 90);
            expected_result = 93;
            expected_polygon = new (double x, double y)[] { (9.2, -9.3), (9.8, -1.1), (9.5, 5.9), (9.3, 9.7), (1.6, 10), (-5.7, 9.2), (-10, 7.1), (-23.55, -6.45), (-5.75, -24.25) };
            Stage1.TestCases.Add(new Stage1TestCase(dogs, shed, expected_polygon, timeLimit: 10, description: "Big example"));
            Stage2.TestCases.Add(new Stage2TestCase(sheeps, dogs, shed, expected_result, timeLimit: 10, description: "Big example"));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Lab12Tests();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }
        }
    }
}