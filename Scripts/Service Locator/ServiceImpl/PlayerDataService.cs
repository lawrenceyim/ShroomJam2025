using ServiceSystem;

public class PlayerDataService : IService {
    private readonly PlayerDataRepository _playerDataRepository;

    public PlayerDataService(PlayerDataRepository playerDataRepository) {
        _playerDataRepository = playerDataRepository;
    }

    public void SetMoney(int money) {
        _playerDataRepository.SetMoney(money);
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
}