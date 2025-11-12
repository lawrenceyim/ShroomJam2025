using System;
using Godot;
using ServiceSystem;

namespace InputSystem;

public class TransactionService : IService {
	private MerchandiseService _merchandiseService;
	private PlayerDataSerivce _playerDataService;
	private const int ValuePerTier = 100;

	public void Initialize(MerchandiseService merchandiseService, PlayerDataSerivce playerDataService) {
		_merchandiseService = merchandiseService;
		_playerDataService = playerDataService;
	}
	
	// Returns transaction profit
	public int SellMerchandise(CustomerSaleDto saleDto) {
		GD.Print($"Sale DTO: {saleDto}");
		GD.Print($"MERCHJ SERVICE IS NULL {_merchandiseService is null}");
		Merchandise merchandise = _merchandiseService.GetHeldMerchandise();
		GD.Print($"HELD Merch: {merchandise}");

		_merchandiseService.SetHeldMerchandise(null);
		
		int profit = _ComputeSalesAmount(saleDto, merchandise);
		int money = _playerDataService.GetMoney();
		money += profit;
		_playerDataService.SetMoney(money);
		return profit;
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
			GD.Print($"COLOR DID NOT MATCH. {amount} => {amount / 2}");
			amount /= 2;
		}

		if (merchandise.Type != saleDto.merchandiseTypeWanted) {
			amount /= 2;
		}

		amount *= totalTipRatio;
		
		GD.Print($"Amount {amount} Total Tip Ratio {totalTipRatio}");
		
		return Math.Max(1, (int)amount);
	}
}
