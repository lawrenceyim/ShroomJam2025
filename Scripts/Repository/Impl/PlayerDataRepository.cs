using RepositorySystem;

public class PlayerDataRepository : IRepository {
    private int _money;
    private int _customerRarityUpgradeLevel = 1;
    private int _merchandiseRarityUpgradeLevel = 1;

    public void SetMoney(int money) {
        _money = money;
    }

    public int GetMoney() {
        return _money;
    }

    public void SetCustomerRarityUpgradeLevel(int level) {
        _customerRarityUpgradeLevel = level;
    }

    public int GetCustomerRarityUpgradeLevel() {
        return _customerRarityUpgradeLevel;
    }

    public void SetMerchandiseRarityUpgradeLevel(int level) {
        _merchandiseRarityUpgradeLevel = level;
    }

    public int GetMerchandiseRarityUpgradeLevel() {
        return _merchandiseRarityUpgradeLevel;
    }
}