using RepositorySystem;

public class PlayerDataRepository : IRepository {
    private int _money;
    private int _customerRarityUpgradeLevel = 1;
    private int _merchandiseRarityUpgradeLevel = 1;
    private int _day = 1;
    private int _careerProfit;

    public void SetMoney(int money) {
        _money = money;
    }

    public void AddMoney(int amount) {
        _money += amount;
        _careerProfit += amount;
    }

    public int GetCareerProfit() {
        return _careerProfit;
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

    public int GetDay() {
        return _day;
    }

    public void SetDay(int day) {
        _day = day;
    }
}