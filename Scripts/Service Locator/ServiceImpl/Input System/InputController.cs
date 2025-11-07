using System;
using Godot;

namespace InputSystem {
    public partial class InputController : Node {
        public event Action<InputEventDto> InputFromPlayer;

        public override void _Input(InputEvent @event) {
            switch (@event) {
                case InputEventKey key:
                    if (key.Echo) {
                        return;
                    }

                    // GD.Print($"InputController KeyEventDTO");
                    InputFromPlayer?.Invoke(new KeyDto(
                        OS.GetKeycodeString(key.PhysicalKeycode),
                        key.Pressed
                    ));
                    break;
                case InputEventMouseButton mouseButton:
                    // GD.Print($"InputController MouseButton");
                    InputFromPlayer?.Invoke(new MouseButtonDto(
                        mouseButton.ButtonIndex.ToString(),
                        mouseButton.Pressed,
                        GetWindow().GetMousePosition()
                    ));
                    break;
                case InputEventMouseMotion mouseMotion:
                    // GD.Print($"InputController MouseMotion");
                    InputFromPlayer?.Invoke(new MouseMotionDto(
                        mouseMotion.Position,
                        mouseMotion.Relative
                    ));
                    break;
                case InputEventJoypadButton:
                    break;
                case InputEventJoypadMotion:
                    break;
            }
        }
    }
}