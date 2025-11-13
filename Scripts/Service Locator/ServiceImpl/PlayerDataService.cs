using ServiceSystem;

public class PlayerDataService : IService {
    private readonly PlayerDataRepository _playerDataRepository;

    public PlayerDataService(PlayerDataRepository playerDataRepository) {
        _playerDataRepository = playerDataRepository;
    }

    public void SetMoney(int money) {
        _playerDataRepository.SetMoney(money);
    }

    public void AddMoney(int money) {
        _playerDataRepository.AddMoney(money);
    }

    public int GetCareerProfit() {
        return _playerDataRepository.GetCareerProfit();
    }

    public int GetMoney() {
        return _playerDataRepository.GetMoney();
    }

    public void SetCustomerRarityUpgradeLevel(int level) {
        _playerDataRepository.SetCustomerRarityUpgradeLevel(level);
    }

    public int GetCustomerRarityUpgradeLevel() {
        return _playerDataRepository.GetCustomerRarityUpgradeLevel();
    }

    public void SetMerchandiseRarityUpgradeLevel(int level) {
        _playerDataRepository.SetMerchandiseRarityUpgradeLevel(level);
    }

    public int GetMerchandiseRarityUpgradeLevel() {
        return _playerDataRepository.GetMerchandiseRarityUpgradeLevel();
    }

    public int GetDay() {
        return _playerDataRepository.GetDay();
    }

    public void SetDay(int day) {
        _playerDataRepository.SetDay(day);
    }
}