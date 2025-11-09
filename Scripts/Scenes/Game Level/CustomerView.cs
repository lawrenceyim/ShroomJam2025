#nullable enable
using System;
using Godot;

public partial class CustomerView : Node2D, ITick {
    private readonly int _numberOfColors = Enum.GetNames(typeof(MerchandiseColor)).Length;
    private readonly int _numberOfMerchandiseTypes = Enum.GetNames(typeof(MerchandiseType)).Length;
    private readonly int _numberOfCustomerTypes = Enum.GetNames(typeof(CustomerType)).Length;
    private readonly Random _random = new Random();
    private readonly TickTimer _dayTimer = new TickTimer();
    private MerchandiseColor _colorWanted;
    private MerchandiseType _merchandiseTypeWanted;
    private CustomerType _customerType;
    private Vector2I _customerSpawnPoint = new Vector2I(-800, -75);
    private Vector2I _customerStopPoint = new Vector2I(-400, -75);
    private Vector2I _customerExitPoint = new Vector2I(0, -75);
    private Sprite2D? _currentCustomerSprite;
    private Sprite2D? _leavingCustomerSprite;
    private CurrentCustomerState _currentCustomerState;
    private Texture2dRepository _texture2DRepository;
    private float _customerHorizontalSpeed = 5; // Per tick
    private float _customerVerticalSpeed = 1; // Per tick
    private int _customerVerticalTicks = 20;
    private bool _customerVerticallyUp = true;
    private int _customerVerticalTicksLeft;


    public void Initialize(Texture2dRepository texture2DRepository) {
        _texture2DRepository = texture2DRepository;
        _dayTimer.StartFixedTimer(false, 60 * Engine.PhysicsTicksPerSecond);
        _dayTimer.TimedOut += () => { }; // TODO: END OF DAY
        _GenerateCustomer();
    }

    public void PhysicsTick(double delta) {
        _UpdateCustomer();
    }

    private void _UpdateCustomer() {
        if (_leavingCustomerSprite is not null) { }

        if (_currentCustomerSprite is not null) {
            switch (_currentCustomerState) {
                case CurrentCustomerState.Entering:
                    // TODO : move sprite to position
                    _currentCustomerSprite.Position += new Vector2(_customerHorizontalSpeed, _customerVerticallyUp ? -_customerVerticalSpeed : _customerVerticalSpeed);
                    _customerVerticalTicksLeft--;
                    if (_customerVerticalTicksLeft == 0) {
                        _customerVerticallyUp = !_customerVerticallyUp;
                        _customerVerticalTicksLeft = _customerVerticalTicks;
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
        _customerType = _SelectRandomCustomerType();
        Sprite2D sprite = new Sprite2D();
        // TODO: SET ITS TEXTURE
        sprite.Texture = _texture2DRepository.GetTexture(Texture2dId.CustomerPlaceholder);
        sprite.Position = _customerSpawnPoint + Position;
        sprite.ZIndex = -1;
        AddChild(sprite);
        _leavingCustomerSprite = _currentCustomerSprite;
        _currentCustomerSprite = sprite;
        _currentCustomerState = CurrentCustomerState.Entering;
        _customerVerticalTicksLeft = _customerVerticalTicks;
    }

    private MerchandiseColor _SelectRandomColor() {
        int choice = _random.Next(0, _numberOfColors);
        return (MerchandiseColor)choice;
    }

    private MerchandiseType _SelectRandomMerchandisesType() {
        int choice = _random.Next(0, _numberOfMerchandiseTypes);
        return (MerchandiseType)choice;
    }

    private CustomerType _SelectRandomCustomerType() {
        int choice = _random.Next(0, _numberOfMerchandiseTypes);
        return (CustomerType)choice;
    }

    private enum CurrentCustomerState {
        Entering,
        Waiting,
    }
}