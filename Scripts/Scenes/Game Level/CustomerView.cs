#nullable enable
using System;
using Godot;
using InputSystem;

public partial class CustomerView : Node2D, ITick {
	[Export]
	private Label _dayTimerLabel;

	[Export]
	private Area2D _merchandiseSellSlot;

	[Export]
	private Label _profitLabel;

	private readonly int _numberOfCustomerIds = Enum.GetNames(typeof(CustomerId)).Length;

	private readonly CustomerGeneratorComponent _customerGenerator = new();
	private readonly Random _random = new Random();
	private readonly TickTimer _dayTimer = new TickTimer();
	private readonly TickTimer _customerTimer = new TickTimer();
	private readonly int _secondsPerCustomerMood = 7;
	private readonly int _ticksPerSecond = Engine.PhysicsTicksPerSecond;

	private Texture2dRepository _texture2DRepository;
	private PlayerDataSerivce _playerDataService;

	private MerchandiseColor _colorWanted;
	private MerchandiseType _merchandiseTypeWanted;
	private CustomerId _customerId;
	private CustomerMood _customerMood;

	private Vector2I _customerSpawnPoint = new Vector2I(-800, -150);
	private int _customerStopPointX = 400;
	private int _customerExitPointX = 900;

	private Sprite2D? _currentCustomerSprite;
	private Sprite2D? _leavingCustomerSprite;
	private CustomerState _customerState;
	private CustomerMovement? _currentCustomerMovement;
	private CustomerMovement? _leavingCustomerMovement;
	private bool _merchandiseSellSlotHovered = false;

	private bool _customerReadytoPurchase = false;

	public void Initialize(Texture2dRepository texture2DRepository, PlayerDataSerivce playerDataService) {
		_texture2DRepository = texture2DRepository;
		_playerDataService = playerDataService;
		_dayTimer.StartFixedTimer(false, 90 * _ticksPerSecond);
		_dayTimer.TimedOut += () => { }; // TODO: END OF DAY
		_customerTimer.TimedOut += _ChangeCustomerMood;
		_merchandiseSellSlot.MouseEntered += () => { _merchandiseSellSlotHovered = true; };
		_merchandiseSellSlot.MouseExited += () => { _merchandiseSellSlotHovered = false; };
		_GenerateCustomer();
	}

	public CustomerSaleDto GetCustomerSale() {
		return new CustomerSaleDto(
			_customerId,
			_customerMood,
			_colorWanted,
			_merchandiseTypeWanted
		);
	}

	public void MerchandiseSold(int amount) {
		_profitLabel.Text = $"${amount}";
		_GenerateCustomer();
	}

	public bool IsMerchandiseSellSlotHovered() {
		return _merchandiseSellSlotHovered;
	}

	public bool IsCustomerReadytoPurchase() {
		return _customerReadytoPurchase;
	}

	public void PhysicsTick(double delta) {
		_dayTimer.PhysicsTick(delta);
		_customerTimer.PhysicsTick(delta);
		_UpdateCustomer();
	}

	private void _ChangeCustomerMood() {
		GD.Print("Change CustomerMood");
		switch (_customerMood) {
			case CustomerMood.Happy:
				_customerMood = CustomerMood.Neutral;
				_currentCustomerSprite.Texture = _GetCustomerSprite();
				break;
			case CustomerMood.Neutral:
				_customerMood = CustomerMood.Angry;
				_currentCustomerSprite.Texture = _GetCustomerSprite();
				break;
			case CustomerMood.Angry:
				_customerTimer.Stop();
				_GenerateCustomer();
				break;
		}

		GD.Print("Exiting ChangeCustomerMood");
	}

	private void _UpdateDayTimer() {
		int secondsLeft = _dayTimer.GetTicksLeft() % _ticksPerSecond;
		_dayTimerLabel.Text = secondsLeft.ToString();
	}

	private void _UpdateCustomer() {
		if (_leavingCustomerSprite is not null) {
			_leavingCustomerSprite.Position += _leavingCustomerMovement.Movement();
			if (Math.Abs(_leavingCustomerSprite.Position.X - _customerExitPointX) < 25) {
				_leavingCustomerSprite.QueueFree();
				_leavingCustomerSprite = null;
			}
		}

		if (_currentCustomerSprite is not null) {
			switch (_customerState) {
				case CustomerState.Entering:
					// TODO : move sprite to position
					_currentCustomerSprite.Position += _currentCustomerMovement.Movement();
					if (Math.Abs(_currentCustomerSprite.Position.X - _customerStopPointX) < 25) {
						_customerState = CustomerState.Waiting;
						_customerReadytoPurchase = true;
						_customerTimer.StartFixedTimer(true, _secondsPerCustomerMood * _ticksPerSecond);
					}

					break;
				case CustomerState.Waiting:
					// TODO: display what the customer wants
					break;
			}
		}
	}

	private void _GenerateCustomer() {
		_colorWanted = MerchandiseUtil.GetRandomMerchandiseColor();
		_merchandiseTypeWanted = MerchandiseUtil.GetRandomMerchandiseType();
		_customerId = _customerGenerator.GetRandomCustomerId(_playerDataService.GetCustomerRarityUpgradeLevel());
		_customerMood = CustomerMood.Happy;
		Sprite2D sprite = new Sprite2D();
		sprite.Texture = _GetCustomerSprite();
		sprite.Position = _customerSpawnPoint + Position;
		sprite.ZIndex = -1;
		AddChild(sprite);
		_leavingCustomerSprite = _currentCustomerSprite;
		_currentCustomerSprite = sprite;
		_customerState = CustomerState.Entering;
		_leavingCustomerMovement = _currentCustomerMovement;
		_currentCustomerMovement = new CustomerMovement();
		_customerReadytoPurchase = false;
	}

	private Texture2D _GetCustomerSprite() {
		return _customerId switch {
			CustomerId.RichMale => _customerMood switch {
				CustomerMood.Happy => _texture2DRepository.GetTexture(Texture2dId.RichManHappy),
				CustomerMood.Neutral => _texture2DRepository.GetTexture(Texture2dId.RichManNeutral),
				_ => _texture2DRepository.GetTexture(Texture2dId.RichManAngry)
			},
			CustomerId.RichFemale => _customerMood switch {
				CustomerMood.Happy => _texture2DRepository.GetTexture(Texture2dId.RichWomanHappy),
				CustomerMood.Neutral => _texture2DRepository.GetTexture(Texture2dId.RichWomanNeutral),
				_ => _texture2DRepository.GetTexture(Texture2dId.RichWomanAngry)
			},
			CustomerId.RegularMale => _customerMood switch {
				CustomerMood.Happy => _texture2DRepository.GetTexture(Texture2dId.AverageManHappy),
				CustomerMood.Neutral => _texture2DRepository.GetTexture(Texture2dId.AverageManNeutral),
				_ => _texture2DRepository.GetTexture(Texture2dId.AverageManAngry)
			},
			CustomerId.RegularFemale => _customerMood switch {
				CustomerMood.Happy => _texture2DRepository.GetTexture(Texture2dId.AverageWomanHappy),
				CustomerMood.Neutral => _texture2DRepository.GetTexture(Texture2dId.AverageWomanNeutral),
				_ => _texture2DRepository.GetTexture(Texture2dId.AverageWomanAngry)
			},
			CustomerId.PoorMale => _customerMood switch {
				CustomerMood.Happy => _texture2DRepository.GetTexture(Texture2dId.PoorManHappy),
				CustomerMood.Neutral => _texture2DRepository.GetTexture(Texture2dId.PoorManNeutral),
				_ => _texture2DRepository.GetTexture(Texture2dId.PoorManAngry)
			},
			CustomerId.PoorFemale => _customerMood switch {
				CustomerMood.Happy => _texture2DRepository.GetTexture(Texture2dId.PoorWomanHappy),
				CustomerMood.Neutral => _texture2DRepository.GetTexture(Texture2dId.PoorWomanNeutral),
				_ => _texture2DRepository.GetTexture(Texture2dId.PoorWomanAngry)
			},
			_ => _texture2DRepository.GetTexture(Texture2dId.CustomerPlaceholder)
		};
	}

	private enum CustomerState {
		Entering,
		Waiting,
	}
}
