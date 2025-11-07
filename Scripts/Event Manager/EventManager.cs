using System;
using System.Collections.Generic;
using EvenetSystem;

namespace EventSystem {
    /// <summary>
    /// Refactor this to be between the UI/controller and the service layer
    /// </summary>
    public class EventManager {
        public event Func<GameEvent> GameEventReceived;
        
        private Dictionary<GameEventId, EventProcessor> _events = new Dictionary<GameEventId, EventProcessor>() { };

        public void AddProcessor(GameEventId gameEventId, EventProcessor eventProcessor) {
            _events[gameEventId] = eventProcessor;
        }

        public void ProcessEvent(GameEvent gameEvent) {
            _events[gameEvent.Id].Process(gameEvent);
        }

        public void Initialize() {
            AddProcessor(GameEventId.CollectShrimpId, new ShrimpCollectEventProcessor());
        }
    }
}