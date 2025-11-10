using Godot;

public class CustomerMovement {
    private const int _customerVerticalTicks = 20;
    private const float _customerHorizontalSpeed = 7; // Per tick
    private const float _customerVerticalSpeed = 1; // Per tick
    private bool _customerVerticallyUp = true;
    private int _customerVerticalTicksLeft = _customerVerticalTicks;

    public Vector2 Movement() {
        _customerVerticalTicksLeft--;
        if (_customerVerticalTicksLeft == 0) {
            _customerVerticallyUp = !_customerVerticallyUp;
            _customerVerticalTicksLeft = _customerVerticalTicks;
        }

        return new Vector2(_customerHorizontalSpeed, _customerVerticallyUp ? -_customerVerticalSpeed : _customerVerticalSpeed);
    }
}