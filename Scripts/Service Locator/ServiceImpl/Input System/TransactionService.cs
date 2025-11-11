using System;
using ServiceSystem;

namespace InputSystem;

public class TransactionService : IService {
	private MerchandiseService _merchandiseService;
	private PlayerDataSerivce _playerDataSerivce;
	private int _valuePerTier = 100;

	public void Initialize(MerchandiseService merchandiseService, PlayerDataSerivce playerDataSerivce) {
		_merchandiseService = merchandiseService;
		_playerDataSerivce = playerDataSerivce;
	}
	
	// Returns transaction profit
	public int SellMerchandise(CustomerSaleDto saleDto) {
		Merchandise merchandise = _merchandiseService.GetHeldMerchandise();
		_merchandiseService.SetHeldMerchandise(null);
		
		int profit = _ComputeSalesAmount(saleDto, merchandise);
		int money = _playerDataSerivce.GetMoney();
		money += profit;
		_playerDataSerivce.SetMoney(money);
		return profit;
	}

	private int _ComputeSalesAmount(CustomerSaleDto saleDto, Merchandise merchandise) {
		float amount = (merchandise.Tier + 1) * _valuePerTier;
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

		if (merchandise.Color == saleDto.colorWanted) {
			amount /= 2;
		}

		if (merchandise.Type != saleDto.merchandiseTypeWanted) {
			amount /= 2;
		}

		amount += amount * totalTipRatio;

		return Math.Max(1, (int)amount);
	}
}
