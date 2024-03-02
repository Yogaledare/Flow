using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml;
using LanguageExt.Common;
using LanguageExt.Pretty;

namespace Flow;

class Program {
    static void Main(string[] args) {
        while (true) {
            PrintMenu();
            int selection = RetrieveInput("Selection: ", ValidateNumber);
            Console.WriteLine();
            switch (selection) {
                case 0:
                    Console.WriteLine("Exiting...");
                    return;
                case 1:
                    Handle1();
                    break;
                case 2:
                    Handle2();
                    break;
                case 3:
                    Handle3();
                    break;
                case 4:
                    Handle4();
                    break;
                default:
                    Console.WriteLine("Bad input, try again.");
                    break;
            }
        }
    }


    private static void PrintMenu() {
        Console.WriteLine();
        Console.WriteLine("Main menu (select with number + enter): ");
        Console.WriteLine("-----------------------");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Cinema, single");
        Console.WriteLine("2. Cinema, group");
        Console.WriteLine("3. Repeat 10 times");
        Console.WriteLine("4. Echo third word");
        Console.WriteLine("-----------------------");
    }


    private static void Handle1() {
        var age = RetrieveInput("Age: ", ValidateNumber);
        var priceGroup = CinemaPriceGroupManager.Instance.FindPriceGroup(age);

        Console.WriteLine($"{priceGroup.PriceName}: {priceGroup.Price}");
    }


    private static void Handle2() {
        var numPersons = RetrieveInput("Number of persons: ", ValidateNumber);
        int sum = 0;

        for (int i = 0; i < numPersons; i++) {
            var age = RetrieveInput($"Age of person {i + 1}: ", ValidateNumber);
            var priceGroup = CinemaPriceGroupManager.Instance.FindPriceGroup(age);
            sum += priceGroup.Price;
        }

        Console.WriteLine($"Total cost: {sum}");
    }


    private static void Handle3() {
        Console.Write("Input: ");
        var input = Console.ReadLine() ?? "";

        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < 10; i++) {
            stringBuilder.Append($"{i + 1}. {input}");
            if (i < 9) {
                stringBuilder.Append(", ");
            }
        }

        Console.WriteLine(stringBuilder.ToString());
    }


    private static void Handle4() {
        var sentenceAtLeastThreeWords = RetrieveInput("Input sentence: ", ValidateSentence);
        var thirdWord = sentenceAtLeastThreeWords[2];

        Console.WriteLine(thirdWord);
    }


    private record CinemaPriceGroup(string PriceName, int AgeLowerLimit, int AgeUpperLimit, int Price);


    private class CinemaPriceGroupManager {
        private static readonly CinemaPriceGroupManager instance = new CinemaPriceGroupManager();
        public static CinemaPriceGroupManager Instance => instance;
        private List<CinemaPriceGroup> _ageGroups;

        private CinemaPriceGroupManager() {
            _ageGroups = new List<CinemaPriceGroup> {
                new CinemaPriceGroup("Barnpris: ", 0, 5, 0),
                new CinemaPriceGroup("Ungdomspris", 0, 19, 80),
                new CinemaPriceGroup("Pensionärspris", 65, 200, 90),
                new CinemaPriceGroup("Standardpris", int.MinValue, int.MaxValue, 120),
            };
        }

        public CinemaPriceGroup FindPriceGroup(int age) {
            return _ageGroups
                    .Where(g => age > g.AgeLowerLimit && age < g.AgeUpperLimit)
                    .MinBy(g => g.Price)!
                ;
        }
    }


    private static Result<string[]> ValidateSentence(string? input) {
        if (string.IsNullOrWhiteSpace(input)) {
            var error = new ValidationException("Error: null or empty input");
            return new Result<string[]>(error);
        }

        var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length < 3) {
            var error = new ValidationException("Error: Sentence needs to be at least three words long");
            return new Result<string[]>(error);
        }

        return tokens;
    }


    private static Result<int> ValidateNumber(string? input) {
        if (string.IsNullOrWhiteSpace(input)) {
            var error = new ValidationException("Error: null or empty input");
            return new Result<int>(error);
        }

        var tokens = input.Split(' ');

        if (tokens.Length > 1) {
            var error = new ValidationException("Error: too many inputs");
            return new Result<int>(error);
        }

        if (!int.TryParse(tokens[0], out int number)) {
            var error = new ValidationException("Error: cannot parse integer");
            return new Result<int>(error);
        }

        if (number < 0) {
            var error = new ValidationException("Error: cannot have negative number");
            return new Result<int>(error);
        }

        return number;
    }


    private static T RetrieveInput<T>(string prompt, Func<string?, Result<T>> validator) {
        T output = default;

        while (true) {
            Console.Write(prompt);
            var input = Console.ReadLine();
            var result = validator(input);

            bool shouldBreak = result.Match(
                Succ: validatedSentence => {
                    output = validatedSentence;
                    return true;
                },
                Fail: ex => {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            );

            if (shouldBreak) {
                break;
            }
        }

        if (output == null) throw new InvalidOperationException("Parsing failed");
        return output;
    }
}