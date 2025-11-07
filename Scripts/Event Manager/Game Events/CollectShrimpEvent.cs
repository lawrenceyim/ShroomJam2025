using System;
using EvenetSystem;

namespace EventSystem;

public record CollectShrimpEvent : GameEvent {
    public GameEventId Id { get; } = GameEventId.CollectShrimpId;
    public ulong ShrimpId { get; }
    
    public CollectShrimpEvent(ulong shrimpId) {
        ShrimpId = shrimpId;
    }
}