using System;
using Godot;
using ServiceSystem;

public class MerchandiseService : IService {
	private const int MerchandiseRollPerLevel = 20;
	private const int BaseMerchandiseRoll = 100;
	private readonly MerchandiseRepository _merchandiseRepository;
	private readonly PlayerDataRepository _playerDataRepository;
	private readonly Random _random = new();

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
		_merchandiseRepository.SetMerchandiseCount(shelfSize.X * shelfSize.Y);

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
		int merchandiseRollIncrease = rarityUpgradeLevel * MerchandiseRollPerLevel;
		int threshold = BaseMerchandiseRoll + merchandiseRollIncrease;
		int rolledRarity = Math.Clamp(_random.Next(0, threshold + 1) / BaseMerchandiseRoll + 1, 1, 5);

		return new Merchandise(
			rolledRarity,
			MerchandiseUtil.GetRandomMerchandiseColor(),
			MerchandiseUtil.GetRandomMerchandiseType()
		);
	}

	public int GetMerchandiseCount() {
		return _merchandiseRepository.GetMerchandiseCount();
	}

	public void SetMerchandiseCount(int count) {
		_merchandiseRepository.SetMerchandiseCount(count);
	}
}
