using System;

public class CustomerGeneratorComponent {
    private const int MinRarity = 0;
    private const int MaxRarity = 2;
    private const int BaseCustomerRoll = 100;
    private const int CustomerRollPerLevel = 20;
    private readonly Random _random = new();

    public CustomerId GetRandomCustomerId(int rarityUpgradeLevel) {
        int merchandiseRollIncrease = rarityUpgradeLevel * CustomerRollPerLevel;
        int threshold = BaseCustomerRoll + merchandiseRollIncrease;
        int rolledRarity = Math.Clamp(_random.Next(0, threshold + 1) / BaseCustomerRoll, MinRarity, MaxRarity);
        bool isMale = _random.Next(0, 2) == 1;

        return rolledRarity switch {
            2 => isMale ? CustomerId.RichMale : CustomerId.RichFemale,
            1 => isMale ? CustomerId.RegularMale : CustomerId.RegularFemale,
            _ => isMale ? CustomerId.PoorMale : CustomerId.PoorFemale
        };
    }
}