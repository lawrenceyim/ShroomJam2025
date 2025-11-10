#nullable enable
using System;
using Godot;

public partial class CustomerView : Node2D, ITick {
	[Export]
	private Label _dayTimerLabel;

	private readonly int _numberOfColors = Enum.GetNames(typeof(MerchandiseColor)).Length;
	private readonly int _numberOfMerchandiseTypes = Enum.GetNames(typeof(MerchandiseType)).Length;
	private readonly int _numberOfCustomerIds = Enum.GetNames(typeof(CustomerId)).Length;

	private readonly Random _random = new Random();
	private readonly TickTimer _dayTimer = new TickTimer();
	private readonly TickTimer _customerTimer = new TickTimer();
	private readonly int _secondsPerCustomerMood = 3;
	private Texture2dRepository _texture2DRepository;
	private int _ticksPerSecond;

	private MerchandiseColor _colorWanted;
	private MerchandiseType _merchandiseTypeWanted;
	private CustomerId _customerId;

	private Vector2I _customerSpawnPoint = new Vector2I(-800, -150);
	private int _customerStopPointX = 400;
	private int _customerExitPointX = 900;

	private Sprite2D? _currentCustomerSprite;
	private Sprite2D? _leavingCustomerSprite;
	private CurrentCustomerState _currentCustomerState;
	private CurrentCustomerMood _currentCustomerMood;
	private CustomerMovement _currentCustomerMovement;
	private CustomerMovement _leavingCustomerMovement;

	public void Initialize(Texture2dRepository texture2DRepository) {
		_ticksPerSecond = Engine.PhysicsTicksPerSecond;
		_texture2DRepository = texture2DRepository;
		_dayTimer.StartFixedTimer(false, 90 * _ticksPerSecond);
		_dayTimer.TimedOut += () => { }; // TODO: END OF DAY
		_customerTimer.TimedOut += _ChangeCustomerMood;
		_GenerateCustomer();
	}

	private void _ChangeCustomerMood() {
		GD.Print("Change CustomerMood");
		switch (_currentCustomerMood) {
			case CurrentCustomerMood.Happy:
				_currentCustomerMood = CurrentCustomerMood.Neutral;
				_currentCustomerSprite.Texture = _GetCustomerSprite();
				break;
			case CurrentCustomerMood.Neutral:
				_currentCustomerMood = CurrentCustomerMood.Angry;
				_currentCustomerSprite.Texture = _GetCustomerSprite();
				break;
			case CurrentCustomerMood.Angry:
				_customerTimer.Stop();
				_GenerateCustomer();
				break;
		}
		GD.Print("Exiting ChangeCustomerMood");
	}

	public void PhysicsTick(double delta) {
		_dayTimer.PhysicsTick(delta);
		_customerTimer.PhysicsTick(delta);
		_UpdateCustomer();
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
			switch (_currentCustomerState) {
				case CurrentCustomerState.Entering:
					// TODO : move sprite to position
					_currentCustomerSprite.Position += _currentCustomerMovement.Movement();
					if (Math.Abs(_currentCustomerSprite.Position.X - _customerStopPointX) < 25) {
						_currentCustomerState = CurrentCustomerState.Waiting;
						_customerTimer.StartFixedTimer(true, _secondsPerCustomerMood * _ticksPerSecond);
					}

					break;
				case CurrentCustomerState.Waiting:
					// TODO: display what the customer wants
					break;
			}
		}
	}

	private void _GenerateCustomer() {
		_colorWanted = _SelectRandomColor();
		_merchandiseTypeWanted = _SelectRandomMerchandisesType();
		_customerId = _SelectRandomCustomerId();
		_currentCustomerMood = CurrentCustomerMood.Happy;
		Sprite2D sprite = new Sprite2D();
		sprite.Texture = _GetCustomerSprite();
		sprite.Position = _customerSpawnPoint + Position;
		sprite.ZIndex = -1;
		AddChild(sprite);
		_leavingCustomerSprite = _currentCustomerSprite;
		_currentCustomerSprite = sprite;
		_currentCustomerState = CurrentCustomerState.Entering;
		_leavingCustomerMovement = _currentCustomerMovement;
		_currentCustomerMovement = new CustomerMovement();
	}

	private MerchandiseColor _SelectRandomColor() {
		int choice = _random.Next(0, _numberOfColors);
		return (MerchandiseColor)choice;
	}

	private MerchandiseType _SelectRandomMerchandisesType() {
		int choice = _random.Next(0, _numberOfMerchandiseTypes);
		return (MerchandiseType)choice;
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
				switch (_currentCustomerMood) {
					case CurrentCustomerMood.Happy:
						return _texture2DRepository.GetTexture(Texture2dId.RichManHappy);
					case CurrentCustomerMood.Neutral:
						return _texture2DRepository.GetTexture(Texture2dId.RichManNeutral);
					default:
						return _texture2DRepository.GetTexture(Texture2dId.RichManAngry);
				}
			case CustomerId.RichFemale:
				switch (_currentCustomerMood) {
					case CurrentCustomerMood.Happy:
						return _texture2DRepository.GetTexture(Texture2dId.RichWomanHappy);
					case CurrentCustomerMood.Neutral:
						return _texture2DRepository.GetTexture(Texture2dId.RichWomanNeutral);
					default:
						return _texture2DRepository.GetTexture(Texture2dId.RichWomanAngry);
				}
			default:
				return _texture2DRepository.GetTexture(Texture2dId.CustomerPlaceholder);
		}
	}


	private enum CurrentCustomerState {
		Entering,
		Waiting,
	}

	private enum CurrentCustomerMood {
		Happy,
		Neutral,
		Angry
	}
}
