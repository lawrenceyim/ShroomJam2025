using System;
using Godot;

public partial class DvdSlot : Sprite2D {
	[Export]
	private Area2D _area2D;

	public event Action<Vector2I> MouseEntered;
	public event Action MouseExited;

	private Vector2I _position;

	public override void _Ready() {
		_area2D.MouseEntered += () => { MouseEntered?.Invoke(_position); };
		_area2D.MouseExited += () => { MouseExited?.Invoke(); };
	}

	public void Initialize(Vector2I position) {
		_position = position;
	}

	public void SetSprite(Texture2D texture) {
		Texture = texture;
	}
}
