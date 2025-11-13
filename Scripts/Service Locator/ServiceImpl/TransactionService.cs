using System;
using Godot;
using ServiceSystem;

namespace InputSystem;

public class TransactionService : IService {
    private MerchandiseService _merchandiseService;
    private PlayerDataSerivce _playerDataService;
    private const int ValuePerTier = 100;
    private int _profitFromDay = 0;

    public void Initialize(MerchandiseService merchandiseService, PlayerDataSerivce playerDataService) {
        _merchandiseService = merchandiseService;
        _playerDataService = playerDataService;
    }

    public int SellMerchandise(CustomerSaleDto saleDto) {
        Merchandise merchandise = _merchandiseService.GetHeldMerchandise();
        _merchandiseService.SetHeldMerchandise(null);
        int profit = _ComputeSalesAmount(saleDto, merchandise);
        _profitFromDay += profit;
        return profit;
    }

    public void ResetProfitFromDay() {
        _profitFromDay = 0;
    }

    public int GetProfitFromDay() {
        return _profitFromDay;
    }

    public void AddProfitFromDayToPlayerMoney() {
        _playerDataService.SetMoney(_playerDataService.GetMoney() + _profitFromDay);
        ResetProfitFromDay();
    }

    private int _ComputeSalesAmount(CustomerSaleDto saleDto, Merchandise merchandise) {
        float amount = merchandise.Tier * ValuePerTier;
        float baseTip = saleDto.customerId switch {
            CustomerId.RichMale => 1,
            CustomerId.RichFemale => 1,
            CustomerId.RegularMale => .5f,
            CustomerId.RegularFemale => .5f,
            CustomerId.PoorMale => .1f,
            CustomerId.PoorFemale => .1f,
            _ => 1
        };
        float tipModifier = saleDto.customerMood switch {
            CustomerMood.Happy => 1,
            CustomerMood.Neutral => .5f,
            CustomerMood.Angry => 0,
            _ => 0
        };

        float totalTipRatio = 1 + (baseTip * tipModifier);

        if (merchandise.Color != saleDto.colorWanted) {
            amount /= 2;
        }

        if (merchandise.Type != saleDto.merchandiseTypeWanted) {
            amount /= 2;
        }

        amount *= totalTipRatio;

        return Math.Max(1, (int)amount);
    }
}