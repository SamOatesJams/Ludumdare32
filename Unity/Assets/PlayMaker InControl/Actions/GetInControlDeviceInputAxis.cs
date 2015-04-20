// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.

using UnityEngine;
using InControl;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("InControl")]
	[Tooltip("Gets the value of the specified Incontrol control Axis for a given device and stores it in a Float Variable.")]
	public class GetInControlDeviceInputAxis : FsmStateAction
	{
		
		[Tooltip("The index of the device. -1 to use the active device")]
		public FsmInt deviceIndex;
		
		public InputControlType axis;
		
		[Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range.")]
		public FsmFloat multiplier;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a float variable.")]
		public FsmFloat store;
		
		[Tooltip("Repeat every frame. Typically this would be set to True.")]
		public bool everyFrame;
		
		InputDevice _inputDevice;
		
		public override void Reset()
		{
			deviceIndex = 0;
			axis = InputControlType.LeftStickLeft;
			multiplier = 1.0f;
			store = null;
			everyFrame = true;
		}
		
		public override void OnEnter()
		{	
			DoGetAxis();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoGetAxis();
		}
		
		void DoGetAxis()
		{
			if (deviceIndex.Value ==-1)
			{
				_inputDevice = InputManager.ActiveDevice;
			}else{
				_inputDevice= InputManager.Devices[deviceIndex.Value];
			}

			float axisValue = _inputDevice.GetControl(axis).Value;
			
			// if variable set to none, assume multiplier of 1
			if (!multiplier.IsNone)
			{
				axisValue *= multiplier.Value;
			}
			
			store.Value = axisValue;
		}
	}
}

