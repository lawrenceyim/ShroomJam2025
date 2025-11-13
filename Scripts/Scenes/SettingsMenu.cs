using Godot;
using System;
using InputSystem;

public partial class SettingsMenu : Node2D, IInputState {
	public event Action ResumeDay;
	public event Action RestartDay;
	public event Action AudioVolumeChange;

	[Export]
	private Sprite2D _volumeCheckMark;

	[Export]
	private Area2D _restartArea2D;

	[Export]
	private Area2D _resumeArea2D;

	[Export]
	private Area2D _volumeArea2D;

	private const string Unpause = "Escape";
	private HoveredArea _hoveredArea = HoveredArea.None;

	public override void _Ready() {
		_restartArea2D.MouseEntered += () => { _hoveredArea = HoveredArea.Restart; };
		_restartArea2D.MouseExited += () => { _hoveredArea = HoveredArea.None; };
		_resumeArea2D.MouseEntered += () => { _hoveredArea = HoveredArea.Resume; };
		_resumeArea2D.MouseExited += () => { _hoveredArea = HoveredArea.None; };
		_volumeArea2D.MouseEntered += () => { _hoveredArea = HoveredArea.Mute; };
		_volumeArea2D.MouseExited += () => { _hoveredArea = HoveredArea.None; };
	}

	public void ProcessInput(InputEventDto eventDto) {
		switch (eventDto) {
			case KeyDto keyDto:
				_ProcessKeyDto(keyDto);
				break;
			case MouseButtonDto mouseButtonDto:
				_ProcessMouseButtonInput(mouseButtonDto);
				break;
		}
	}

	public void Display() {
		_SetVolumeCheckmark();
		Visible = true;
	}

	public void Hide() {
		Visible = false;
	}

	private void _SetVolumeCheckmark() {
		_volumeCheckMark.Visible = !GlobalSettings.MuteVolume;
	}

	private void _ProcessKeyDto(KeyDto keyDto) {
		if (!keyDto.Pressed) {
			return;
		}

		switch (keyDto.Identifier) {
			case Unpause:
				ResumeDay?.Invoke();
				break;
		}
	}

	private void _ProcessMouseButtonInput(MouseButtonDto dto) {
		if (!dto.Pressed) {
			return;
		}

		switch (_hoveredArea) {
			case HoveredArea.Restart:
				RestartDay?.Invoke();
				break;
			case HoveredArea.Mute:
				GlobalSettings.MuteVolume = !GlobalSettings.MuteVolume;
				_SetVolumeCheckmark();
				AudioVolumeChange?.Invoke();
				break;
			case HoveredArea.Resume:
				ResumeDay?.Invoke();
				break;
		}
	}

	private enum HoveredArea {
		None = 0,
		Mute = 1,
		Restart = 2,
		Resume = 3,
	}
}
