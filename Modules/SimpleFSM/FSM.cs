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
// ^\s+
// (?<Define>(\w+\s*)+)
// (?<Generic>(?:<\s*\S+\s*>)*)
// \((?<Params>[^\(\)\[\]]*)\)
namespace CZToolKit.Core.SimpleFSM
{
    public class FSM : IFSM
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
            states.Add(stateName, state);
        }

        public virtual void JumpTo(string stateName)
        {
            if (currentState == states[stateName])
                return;

            if (currentState != null)
                currentState.OnEnd();

            currentState = states[stateName];
            if (currentState != null)
                currentState.OnBegin();
        }
    }

    public interface IFSM
    {
        void Update();

        void PushState(string stateName, IFSMState state);

        void JumpTo(string stateName);
    }

    public interface IFSMState
    {
        void OnBegin();

        void OnUpdate();

        void OnEnd();
    }
}