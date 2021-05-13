using System;
using System.Collections.Generic;

namespace CZToolKit.Core.SimpleFSM
{
    public class FSM
    {
        private Dictionary<string, IFSMState> states = new Dictionary<string, IFSMState>();
        private IFSMState currentState;

        public void Update()
        {
            if (currentState != null)
                currentState.OnUpdate();
        }

        public void PushState(string stateName, IFSMState state)
        {
            states[stateName] = state;
        }

        public void ChangeTo(string stateName)
        {
            if (currentState == states[stateName])
                return;

            if (currentState != null)
                currentState.OnExit();

            currentState = states[stateName];
            if (currentState != null)
                currentState.OnStart();
        }
    }

    public interface IFSMState
    {
        void OnStart();

        void OnUpdate();

        void OnExit();
    }
}