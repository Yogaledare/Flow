namespace Flow;

class Program {
    static void Main(string[] args) {
        while (true) {
            Console.WriteLine();
            Console.WriteLine("Main menu (select with number + enter): ");
            Console.WriteLine("-----------------------");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Youth or retiree");
            Console.WriteLine("2. Groups");
            Console.WriteLine("-----------------------");
            Console.Write("Selection: ");
            var selection = Console.ReadLine();
            // Console.WriteLine();

            if (!int.TryParse(selection, out int selectionInt)) {
                Console.WriteLine("Incorrect input");
                continue;
            }

            switch (selectionInt) {
                case 0:
                    Console.WriteLine("Exiting...");
                    return;
                case 1:
                    Handle1();
                    break;
                case 2:
                    Handle2();
                    break;
            }
        }
    }


    private static void Handle1() {
        Console.Write("Age: ");
        var input = Console.ReadLine();
        var age = ValidateAgeInput(input);

        if (age == null) return;

        var cinemaPriceGroup = CinemaPriceGroupManager.Instance.FindPriceGroup((int) age);

        if (cinemaPriceGroup == null) {
            Console.WriteLine($"could not find price group for age: {age}");
            return;
        }

        Console.WriteLine($"{cinemaPriceGroup.PriceName}: {cinemaPriceGroup.Price}kr");
    }


    private static void Handle2() {
        
    }


    private static int? ValidateAgeInput(string? input) {
        if (string.IsNullOrWhiteSpace(input)) {
            Console.WriteLine("Error: null or empty input");
            return null;
        }

        var tokens = input.Split(' ');

        if (tokens.Length > 1) {
            Console.WriteLine("Error: too many inputs");
            return null;
        }

        if (!int.TryParse(tokens[0], out int age)) {
            Console.WriteLine("Error: cannot parse integer");
            return null;
        }

        return age;
    }


    private record CinemaPriceGroup(string PriceName, int AgeLowerLimit, int AgeUpperLimit, int Price);


    private class CinemaPriceGroupManager {
        private static readonly CinemaPriceGroupManager instance = new CinemaPriceGroupManager();
        public static CinemaPriceGroupManager Instance => instance;
        private List<CinemaPriceGroup> _ageGroups;

        private CinemaPriceGroupManager() {
            _ageGroups = new List<CinemaPriceGroup> {
                new CinemaPriceGroup("Ungdomspris", 0, 19, 80),
                new CinemaPriceGroup("Standardpris", 20, 64, 120),
                new CinemaPriceGroup("Pensionärspris", 65, 200, 90),
            };
        }

        public CinemaPriceGroup? FindPriceGroup(int age) {
            return _ageGroups.FirstOrDefault(group => age >= group.AgeLowerLimit && age <= group.AgeUpperLimit);
        }
    }
}


// private enum AgeCategory {
//     Youth,
//     Adult,
//     Retiree
// }
//
//
// private static AgeCategory FindAgeCategory(int age) {
//     switch (age) {
//         case < 20:
//             return AgeCategory.Youth;
//         case > 64:
//             return AgeCategory.Retiree;
//         default:
//             return AgeCategory.Adult;
//     }
// }
//
//
// private static int FindTicketPrice(AgeCategory ageCategory) {
//     Dictionary<AgeCategory, int> prices = new Dictionary<AgeCategory, int>() {
//         {AgeCategory.Youth, 80},
//         {AgeCategory.Adult, 120},
//         {AgeCategory.Retiree, 90},
//     };
//
//     return prices[ageCategory];
// }


// private static readonly CinemaAgeGroupManager _instance = new CinemaAgeGroupManager();
// public static CinemaAgeGroupManager Instance => _instance; 


// var ageCategory = FindAgeCategory((int) age);
// var price = FindTicketPrice(ageCategory);


// switch (ageCategory) {
//     case AgeCategory.Youth:
//         Console.WriteLine($"Ungdomspris: {price}kr");
//         return; 
//     case AgeCategory.Retiree:
//         Console.WriteLine($"Pensionärspris: {price}kr");
//         return; 
//     default:
//         Console.WriteLine($"Standardpris: {price}kr");
//         return;
// }