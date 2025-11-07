using System;
using EvenetSystem;

namespace EventSystem {
    public interface GameEvent {
        public GameEventId Id { get; }
    }
}