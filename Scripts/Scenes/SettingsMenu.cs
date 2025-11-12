using Godot;
using System;
using InputSystem;

public partial class SettingsMenu : Node2D, IInputState {
	public event Action ResumeDay;
	public event Action RestartDay;

	[Export]
	private Sprite2D _mutedCheckMark;

	[Export]
	private Area2D _restartArea2D;

	[Export]
	private Area2D _resumeArea2D;

	[Export]
	private Area2D _muteArea2D;

	private HoveredArea _hoveredArea = HoveredArea.None;

	public override void _Ready() {
		_restartArea2D.MouseEntered += () => { _hoveredArea = HoveredArea.Restart; };
		_restartArea2D.MouseExited += () => { _hoveredArea = HoveredArea.None; };
		_resumeArea2D.MouseEntered += () => { _hoveredArea = HoveredArea.Resume; };
		_resumeArea2D.MouseExited += () => { _hoveredArea = HoveredArea.None; };
		_muteArea2D.MouseEntered += () => { _hoveredArea = HoveredArea.Mute; };
		_muteArea2D.MouseExited += () => { _hoveredArea = HoveredArea.None; };
		_mutedCheckMark.Visible = GlobalSettings.MuteVolume;
	}

	public void ProcessInput(InputEventDto eventDto) {
		if (eventDto is MouseButtonDto dto) {
			switch (_hoveredArea) {
				case HoveredArea.Restart:
					break;
				case HoveredArea.Mute:
					GlobalSettings.MuteVolume = !GlobalSettings.MuteVolume;
					_mutedCheckMark.Visible = GlobalSettings.MuteVolume;
					break;
				case HoveredArea.Resume:
					ResumeDay?.Invoke();
					break;
				default:
					break;
			}
		}
	}

	private enum HoveredArea {
		None = 0,
		Mute = 1,
		Restart = 2,
		Resume = 3,
	}
}
