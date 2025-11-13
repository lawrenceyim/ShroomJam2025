using Godot;

public class BlinkingComponent {
    private Color _originalColor;
    private Color _highlightedColor;
    private Node2D _node;
    private Tween _tween;

    public void Instantiate(Node2D node, float duration, Color originalColor, Color highlight) {
        _node = node;
        _originalColor = originalColor;
        _highlightedColor = highlight;
        _tween = node.GetTree().CreateTween();
        _tween.SetLoops();
        _tween.TweenProperty(_node, "self_modulate", highlight, duration);
        _tween.TweenProperty(_node, "self_modulate", originalColor, duration);
    }

    public void KillTween() {
        _tween.Kill();
    }

    public void Pause(bool paused) {
        if (paused) {
            _tween.Pause();
        } else {
            _tween.Play();
        }
    }

    public void ResetColor() {
        _node.SelfModulate = _originalColor;
    }

    public void SetHighlightedColor() {
        _node.SelfModulate = _highlightedColor;
    }
}