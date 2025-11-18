using ServiceSystem;

public class UpgradeService : IService {
    private readonly PlayerDataService _playerDataService;
    private const int CustomerRarityUpgradeMaxLevel = 10;
    private const int MerchandiseRarityUpgradeMaxLevel = 20;
    private const int CostPerLevel = 100;

    public UpgradeService(PlayerDataService playerDataService) {
        _playerDataService = playerDataService;
    }

    public bool CanUpgrade(UpgradeType upgradeType) {
        return GetUpgradeLevel(upgradeType) < GetMaxUpgradeLevel(upgradeType) &&
               ComputeUpgradeCost(upgradeType) <= _playerDataService.GetMoney();
    }

    public bool IsMaxLevel(UpgradeType upgradeType) {
        return GetUpgradeLevel(upgradeType) >= GetMaxUpgradeLevel(upgradeType);
    }

    public void UpgradeRarity(UpgradeType upgradeType) {
        int cost = ComputeUpgradeCost(upgradeType);
        _playerDataService.SetMoney(_playerDataService.GetMoney() - cost);

        switch (upgradeType) {
            case UpgradeType.CustomerRarity:
                _playerDataService.SetCustomerRarityUpgradeLevel(_playerDataService.GetCustomerRarityUpgradeLevel() + 1);
                break;
            case UpgradeType.MerchandiseRarity:
                _playerDataService.SetMerchandiseRarityUpgradeLevel(_playerDataService.GetMerchandiseRarityUpgradeLevel() + 1);
                break;
        }
    }

    public int ComputeUpgradeCost(UpgradeType upgradeType) {
        return upgradeType switch {
            UpgradeType.CustomerRarity => CostPerLevel * _playerDataService.GetCustomerRarityUpgradeLevel(),
            UpgradeType.MerchandiseRarity => CostPerLevel * _playerDataService.GetMerchandiseRarityUpgradeLevel(),
        };
    }

    public int GetUpgradeLevel(UpgradeType upgradeType) {
        return upgradeType switch {
            UpgradeType.CustomerRarity => _playerDataService.GetCustomerRarityUpgradeLevel(),
            UpgradeType.MerchandiseRarity => _playerDataService.GetMerchandiseRarityUpgradeLevel(),
        };
    }

    public int GetMaxUpgradeLevel(UpgradeType upgradeType) {
        return upgradeType switch {
            UpgradeType.CustomerRarity => CustomerRarityUpgradeMaxLevel,
            UpgradeType.MerchandiseRarity => MerchandiseRarityUpgradeMaxLevel,
        };
    }

    public enum UpgradeType {
        CustomerRarity,
        MerchandiseRarity
    }
}