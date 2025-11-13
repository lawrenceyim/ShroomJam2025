#nullable enable
using System;
using System.Threading.Tasks;
using Godot;

public partial class CustomerView : Node2D, ITick {
    public event Action<SoundEffectId> PlaySoundEffect;

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
    private readonly TickTimer _customerTimer = new();
    private readonly int _secondsPerCustomerMood = 7;
    private readonly int _ticksPerSecond = Engine.PhysicsTicksPerSecond;

    private Texture2dRepository _texture2DRepository;
    private PlayerDataService _playerDataService;
    private readonly Color _originalSlotColor = new Color(1, 1, 1, .5f);
    private readonly Color _highlightedSlotColor = new Color(1, 1, 0, 1);
    private readonly BlinkingComponent _sellSlotBlinkingComponent = new();

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
    private bool _holdingItem = false;

    public void Initialize(Texture2dRepository texture2DRepository, PlayerDataService playerDataService) {
        _texture2DRepository = texture2DRepository;
        _playerDataService = playerDataService;
        _customerTimer.TimedOut += _ChangeCustomerMood;
        _merchandiseSellSlotArea.MouseEntered += () => { _merchandiseSellSlotHovered = true; };
        _merchandiseSellSlotArea.MouseExited += () => { _merchandiseSellSlotHovered = false; };
        _GenerateCustomer();
        _sellSlotBlinkingComponent.Instantiate(_merchandiseSellSlotHighlight, 1, _originalSlotColor, _highlightedSlotColor);
        _HighlightSellSlot();
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
    }

    private void _ChangeCustomerMood() {
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
                PlaySoundEffect?.Invoke(SoundEffectId.SaleFailed);
                _GenerateCustomer();
                break;
        }
    }

    public void UpdateDayTimer(int ticksLeft) {
        int seconds = ticksLeft / _ticksPerSeconds;
        int minutes = seconds / 60;
        seconds %= 60;

        _dayTimerLabel.Text = $"{minutes:D2}:{seconds:D2}";
    }

    public void SetHoldingItem(bool holdingItem) {
        _holdingItem = holdingItem;
        _HighlightSellSlot();
    }

    private void _HighlightSellSlot() {
        if (!_holdingItem || _customerState != CustomerState.Waiting) {
            _merchandiseSellSlotHighlight.Visible = false;
            _merchandiseSellSlotHighlight.Color = _originalSlotColor;
            _sellSlotBlinkingComponent.Pause(true);
            return;
        }

        _merchandiseSellSlotHighlight.Visible = true;
        _sellSlotBlinkingComponent.Pause(false);
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
                    _currentCustomerSprite.Position += _currentCustomerMovement.Movement();
                    if (Math.Abs(_currentCustomerSprite.Position.X - _customerStopPointX) < 25) {
                        _customerState = CustomerState.Waiting;
                        _customerReadyToPurchase = true;
                        _customerTimer.StartFixedTimer(true, _secondsPerCustomerMood * _ticksPerSecond);
                    }

                    break;
                case CustomerState.Waiting:
                    _HighlightSellSlot();
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
        Sprite2D sprite = new();
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
        }
    }

    private Texture2D _GetCustomerSprite() {
        Texture2dId textureId = CustomerUtil.GetCustomerTexture(_customerId, _customerMood);
        return _texture2DRepository.GetTexture(textureId);
    }

    private enum CustomerState {
        Entering,
        Waiting,
    }
}