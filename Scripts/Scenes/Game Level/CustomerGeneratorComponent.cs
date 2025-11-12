using System;

public class CustomerGeneratorComponent {
    private const int BaseCustomerRoll = 100;
    private const int CustomerRollPerLevel = 20;
    private readonly Random _random = new();

    public CustomerId GetRandomCustomerId(int rarityUpgradeLevel) {
        int merchandiseRollIncrease = rarityUpgradeLevel * CustomerRollPerLevel;
        int threshold = BaseCustomerRoll + merchandiseRollIncrease;
        int rolledRarity = _random.Next(0, threshold + 1) / BaseCustomerRoll;
        bool isMale = _random.Next(0, 2) == 1;

        return rolledRarity switch {
            2 => isMale ? CustomerId.RichMale : CustomerId.RichFemale,
            1 => isMale ? CustomerId.RegularMale : CustomerId.RegularFemale,
            _ => isMale ? CustomerId.PoorMale : CustomerId.PoorFemale
        };
    }
}