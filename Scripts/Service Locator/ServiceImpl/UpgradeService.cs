using ServiceSystem;

public class UpgradeService : IService {
    private PlayerDataSerivce _playerDataSerivce;
    private int _customerRarityUpgradeMaxLevel = 10;
    private int _merchandiseRarityUpgradeMaxLevel = 20;
    private int _costPerLevel = 100;

    public UpgradeService(PlayerDataSerivce playerDataSerivce) {
        _playerDataSerivce = playerDataSerivce;
    }

    public bool CanUpgrade(UpgradeType upgradeType) {
        return GetUpgradeLevel(upgradeType) < GetMaxUpgradeLevel(upgradeType) &&
               ComputeUpgradeCost(upgradeType) < _playerDataSerivce.GetMoney();
    }

    public void UpgradeCustomerRarity(UpgradeType upgradeType) {
        int cost = ComputeUpgradeCost(upgradeType);
        _playerDataSerivce.SetMoney(_playerDataSerivce.GetMoney() - cost);

        switch (upgradeType) {
            case UpgradeType.CustomerRarity:
                _playerDataSerivce.SetCustomerRarityUpgradeLevel(_playerDataSerivce.GetCustomerRarityUpgradeLevel() + 1);
                break;
            case UpgradeType.MerchandiseRarity:
                _playerDataSerivce.SetMerchandiseRarityUpgradeLevel(_playerDataSerivce.GetMerchandiseRarityUpgradeLevel() + 1);
                break;
        }
    }


    public int ComputeUpgradeCost(UpgradeType upgradeType) {
        return upgradeType switch {
            UpgradeType.CustomerRarity => _costPerLevel * _playerDataSerivce.GetCustomerRarityUpgradeLevel(),
            UpgradeType.MerchandiseRarity => _costPerLevel * _playerDataSerivce.GetMerchandiseRarityUpgradeLevel(),
        };
    }

    public int GetUpgradeLevel(UpgradeType upgradeType) {
        return upgradeType switch {
            UpgradeType.CustomerRarity => _playerDataSerivce.GetCustomerRarityUpgradeLevel(),
            UpgradeType.MerchandiseRarity => _playerDataSerivce.GetMerchandiseRarityUpgradeLevel(),
        };
    }

    public int GetMaxUpgradeLevel(UpgradeType upgradeType) {
        return upgradeType switch {
            UpgradeType.CustomerRarity => _customerRarityUpgradeMaxLevel,
            UpgradeType.MerchandiseRarity => _merchandiseRarityUpgradeMaxLevel,
        };
    }

    public enum UpgradeType {
        CustomerRarity,
        MerchandiseRarity
    }
}