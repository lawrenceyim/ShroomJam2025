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

	private readonly Random _random = new Random();
	private readonly TickTimer _dayTimer = new TickTimer();
	private readonly TickTimer _customerTimer = new TickTimer();
	private readonly int _secondsPerCustomerMood = 7;
	private int _ticksPerSecond;

	private Texture2dRepository _texture2DRepository;

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

	public void Initialize(Texture2dRepository texture2DRepository) {
		_ticksPerSecond = Engine.PhysicsTicksPerSecond;
		_texture2DRepository = texture2DRepository;
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

	// TODO: Add this later
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
		_customerId = _SelectRandomCustomerId();
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


	// TODO: change this to be probabilistic based on upgrades
	private CustomerId _SelectRandomCustomerId() {
		return _random.Next(0, 2) == 1 ? CustomerId.RichMale : CustomerId.RichFemale;
		// int choice = _random.Next(0, _numberOfCustomerIds);
		// return (CustomerId)choice;
	}

	private Texture2D _GetCustomerSprite() {
		switch (_customerId) {
			case CustomerId.RichMale:
				switch (_customerMood) {
					case CustomerMood.Happy:
						return _texture2DRepository.GetTexture(Texture2dId.RichManHappy);
					case CustomerMood.Neutral:
						return _texture2DRepository.GetTexture(Texture2dId.RichManNeutral);
					default:
						return _texture2DRepository.GetTexture(Texture2dId.RichManAngry);
				}
			case CustomerId.RichFemale:
				switch (_customerMood) {
					case CustomerMood.Happy:
						return _texture2DRepository.GetTexture(Texture2dId.RichWomanHappy);
					case CustomerMood.Neutral:
						return _texture2DRepository.GetTexture(Texture2dId.RichWomanNeutral);
					default:
						return _texture2DRepository.GetTexture(Texture2dId.RichWomanAngry);
				}
			default:
				return _texture2DRepository.GetTexture(Texture2dId.CustomerPlaceholder);
		}
	}


	private enum CustomerState {
		Entering,
		Waiting,
	}
}
