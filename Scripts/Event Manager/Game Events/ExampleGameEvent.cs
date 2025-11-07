using System;
using EvenetSystem;

namespace EventSystem {
    public record ExampleGameEvent : GameEvent {
        public GameEventId Id { get; } =  GameEventId.ExampleEventId;
        public string Message { get; }

        public ExampleGameEvent(string message) {
            Message = message;
        }
    }
}