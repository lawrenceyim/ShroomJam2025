using System;
using Godot;
using ServiceSystem;

public class MerchandiseService : IService {
	private MerchandiseRepository _merchandiseRepository;
	private PlayerDataRepository _playerDataRepository;
	private int _merchandiseRollPerLevel = 20;
	private int _baseMerchandiseRoll = 100;
	private Random _random = new();

	public MerchandiseService(MerchandiseRepository merchandiseRepository, PlayerDataRepository playerDataRepository) {
		_merchandiseRepository = merchandiseRepository;
		_playerDataRepository = playerDataRepository;
	}

	public Vector2I GetShelfSize() {
		return _merchandiseRepository.GetShelfSize();
	}

	public Merchandise GetMerchandiseFromShelf(Vector2I position) {
		return _merchandiseRepository.GetMerchandise(position);
	}

	public Merchandise GetHeldMerchandise() {
		return _merchandiseRepository.GetHeldMerchandise();
	}

	public void SetHeldMerchandise(Merchandise merchandise) {
		_merchandiseRepository.SetHeldMerchandise(merchandise);
	}

	public void SetShelfMerchandise(Merchandise merchandise, Vector2I position) {
		_merchandiseRepository.SetMerchandise(merchandise, position);
	}

	public void RestockMerchandise() {
		Merchandise merchandise = GetHeldMerchandise();
		SetHeldMerchandise(null);
		Vector2I shelfSize = GetShelfSize();

		for (int c = 0; c < shelfSize.X; c++) {
			for (int r = 0; r < shelfSize.Y; r++) {
				Vector2I position = new Vector2I(c, r);
				merchandise ??= _GenerateRandomMerchandise(_playerDataRepository.GetMerchandiseRarityUpgradeLevel());

				if (_merchandiseRepository.GetMerchandise(position) is null) {
					_merchandiseRepository.SetMerchandise(merchandise, position);
				}
			}
		}
	}

	private Merchandise _GenerateRandomMerchandise(int rarityUpgradeLevel) {
		int merchandiseRollIncrease = rarityUpgradeLevel * _merchandiseRollPerLevel;
		int threshold = _baseMerchandiseRoll + merchandiseRollIncrease;
		int rolledRarity = _random.Next(0, threshold + 1) / _baseMerchandiseRoll;

		return new Merchandise(
			rolledRarity,
			MerchandiseUtil.GetRandomMerchandiseColor(),
			MerchandiseUtil.GetRandomMerchandiseType()
		);
	}
}
