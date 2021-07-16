#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System.Collections.Generic;

namespace CZToolKit.Core.SimpleFSM
{
    public class FSM
    {
        private Dictionary<string, IFSMState> states = new Dictionary<string, IFSMState>();
        private IFSMState currentState;

        public virtual void Update()
        {
            if (currentState != null)
                currentState.OnUpdate();
        }

        public virtual void PushState(string stateName, IFSMState state)
        {
            states[stateName] = state;
        }

        public virtual void ChangeTo(string stateName)
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
        FSM Owner { get; }

        void OnStart();

        void OnUpdate();

        void OnExit();
    }
}