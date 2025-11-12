#nullable enable
using System;
using System.Threading.Tasks;
using Godot;

public partial class CustomerView : Node2D, ITick {
	[Export]
	private Label _dayTimerLabel;

	[Export]
	private Area2D _merchandiseSellSlotArea;

	[Export]
	private Polygon2D _merchandiseSellSlotHighlight;

	[Export]
	private Label _profitLabel;

	[Export]
	private Sprite2D _desiredMerchandiseSprite;

	[Export]
	private Sprite2D _thoughtBubble;

	private readonly int _numberOfCustomerIds = Enum.GetNames(typeof(CustomerId)).Length;
	private readonly int _ticksPerSeconds = Engine.PhysicsTicksPerSecond;
	private readonly CustomerGeneratorComponent _customerGenerator = new();
	private readonly Random _random = new Random();
	private readonly TickTimer _customerTimer = new TickTimer();
	private readonly int _secondsPerCustomerMood = 7;
	private readonly int _ticksPerSecond = Engine.PhysicsTicksPerSecond;

	private Texture2dRepository _texture2DRepository;
	private PlayerDataSerivce _playerDataService;
	private readonly Color _originalSlotColor = new Color(1, 1, 0, 0);
	private readonly Color _highlightedSlotColor = new Color(1, 1, 0, .5f);

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
	private bool _customerReadyToPurchase = false;

	private bool _highlightActive = false;
	private bool _highlightLerpingIn = true;
	private int _ticksPerLerp = 60;
	private int _tickLerpCounter = 0;

	public void Initialize(Texture2dRepository texture2DRepository, PlayerDataSerivce playerDataService) {
		_texture2DRepository = texture2DRepository;
		_playerDataService = playerDataService;
		_customerTimer.TimedOut += _ChangeCustomerMood;
		_merchandiseSellSlotArea.MouseEntered += () => { _merchandiseSellSlotHovered = true; };
		_merchandiseSellSlotArea.MouseExited += () => { _merchandiseSellSlotHovered = false; };
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

	public bool IsCustomerReadyToPurchase() {
		return _customerReadyToPurchase;
	}

	public void PhysicsTick(double delta) {
		_customerTimer.PhysicsTick(delta);
		_UpdateCustomer();
		_LerpHighlight();
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

	public void UpdateDayTimer(int ticksLeft) {
		int seconds = ticksLeft / _ticksPerSeconds;
		int minutes = seconds / 60;
		seconds %= 60;

		_dayTimerLabel.Text = $"{minutes:D2}:{seconds:D2}";
	}

	private void _LerpHighlight() {
		if (!_highlightActive || _customerState != CustomerState.Waiting) {
			return;
		}

		float t = (float)_tickLerpCounter / _ticksPerLerp;
		_merchandiseSellSlotHighlight.Color = _highlightLerpingIn
			? _originalSlotColor.Lerp(_highlightedSlotColor, t)
			: _highlightedSlotColor.Lerp(_originalSlotColor, t);

		_tickLerpCounter++;
		if (_tickLerpCounter >= _ticksPerLerp) {
			_highlightLerpingIn = !_highlightLerpingIn;
			_tickLerpCounter = 0;
		}
	}

	public void HighlightSellSlot(bool highlighted) {
		if (!highlighted) {
			GD.Print("Highlight SellSlot Invisible");
			_merchandiseSellSlotHighlight.Visible = false;
			_highlightActive = false;
			_merchandiseSellSlotHighlight.Color = _originalSlotColor;
			return;
		}

		GD.Print("Highlight SellSlot Visible");
		_merchandiseSellSlotHighlight.Visible = true;
		_tickLerpCounter = 0;
		_highlightLerpingIn = true;
		_highlightActive = true;
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
						_customerReadyToPurchase = true;
						_customerTimer.StartFixedTimer(true, _secondsPerCustomerMood * _ticksPerSecond);
					}

					break;
				case CustomerState.Waiting:
					_ToggleVisibilityOfDesiredMerchandise(true);
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
		_customerReadyToPurchase = false;
		_UpdateDesiredMerchandise();
		_ToggleVisibilityOfDesiredMerchandise(false);
	}

	private void _ToggleVisibilityOfDesiredMerchandise(bool visible) {
		_desiredMerchandiseSprite.Visible = visible;
		_thoughtBubble.Visible = visible;
	}

	private void _UpdateDesiredMerchandise() {
		switch (_colorWanted) {
			case MerchandiseColor.Blue:
				_desiredMerchandiseSprite.Texture = _texture2DRepository.GetTexture(Texture2dId.BlueDvdBlank);
				break;
			case MerchandiseColor.Red:
				_desiredMerchandiseSprite.Texture = _texture2DRepository.GetTexture(Texture2dId.RedDvdBlank);
				break;
			case MerchandiseColor.Green:
				_desiredMerchandiseSprite.Texture = _texture2DRepository.GetTexture(Texture2dId.GreenDvdBlank);
				break;
			default:
				break;
		}
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
