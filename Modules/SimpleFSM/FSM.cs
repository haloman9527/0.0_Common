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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using System.Collections.Generic;

namespace CZToolKit.SimpleFSM
{
    public class FSM : IFSM
    {
        private Dictionary<string, IFSMState> states = new Dictionary<string, IFSMState>();
        private IFSMState currentState;
        
        private IFSMAgent agent;

        public IFSMAgent Agent
        {
            get { return agent; }
        }

        public void Init(IFSMAgent agent)
        {
            this.agent = agent;
        }

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

    public interface IFSMAgent
    {
        
    }
    
    public interface IFSM
    {
        IFSMAgent Agent { get; }
        
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