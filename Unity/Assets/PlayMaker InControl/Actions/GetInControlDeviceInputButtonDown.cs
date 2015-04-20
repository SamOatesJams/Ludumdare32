// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.

using UnityEngine;
using InControl;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("InControl")]
	[Tooltip("Sends an Event when the specified Incontrol control Axis for a given Device is pressed. Optionally store the control state in a bool variable.")]
	public class GetInControlDeviceInputButtonDown : FsmStateAction
	{
        public FsmEvent sendEvent;

        private CharacterControlActions m_actionSet = null;

        public override void Reset()
		{			
			sendEvent = null;
		}
		
		public override void OnEnter()
		{
            m_actionSet = new CharacterControlActions();
            m_actionSet.Setup();
        }

		public override void OnUpdate()
		{
            var wasPressed = m_actionSet.Interact.WasPressed;
            if (wasPressed)
			{
				Fsm.Event(sendEvent);				
			}
		}
	}
}