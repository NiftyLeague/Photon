namespace Quantum.Profiling
{
	using UnityEngine;
	using UnityEngine.PlayerLoop;

	public static class Profilers
	{
		public static float LastScriptsTime { get; private set; }
		public static float LastRenderTime  { get; private set; }
		public static float LastFrameTime   { get; private set; }

		private static Timer _scriptsTimer = new Timer("Scripts");
		private static Timer _renderTimer  = new Timer("Render");
		private static Timer _frameTimer   = new Timer("Frame");

		//========== PRIVATE METHODS ==================================================================================

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void InitializeSubSystem()
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
				return;
#endif
			if (PlayerLoopUtility.HasPlayerLoopSystem(typeof(Profilers)) == false)
			{
				PlayerLoopUtility.AddPlayerLoopSystem(typeof(Profilers), typeof(EarlyUpdate), EarlyUpdate, 0);
				PlayerLoopUtility.AddPlayerLoopSystem(typeof(Profilers), typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate),  BeforeFixedUpdate, AfterFixedUpdate);
				PlayerLoopUtility.AddPlayerLoopSystem(typeof(Profilers), typeof(Update.ScriptRunBehaviourUpdate),            BeforeUpdate,      AfterUpdate);
				PlayerLoopUtility.AddPlayerLoopSystem(typeof(Profilers), typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), BeforeLateUpdate,  AfterLateUpdate);
				PlayerLoopUtility.AddPlayerLoopSystem(typeof(Profilers), typeof(PostLateUpdate), PostLateUpdateFirst, 0);
				PlayerLoopUtility.AddPlayerLoopSystem(typeof(Profilers), typeof(PostLateUpdate), PostLateUpdateLast);
			}

			Application.quitting -= OnApplicationQuit;
			Application.quitting += OnApplicationQuit;
		}

		private static void EarlyUpdate()
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				PlayerLoopUtility.RemovePlayerLoopSystems(typeof(Profilers));
				return;
			}
#endif
			_frameTimer.Restart();
		}

		private static void BeforeFixedUpdate() { _scriptsTimer.Restart(); }
		private static void AfterFixedUpdate()  { _scriptsTimer.Stop();    }
		private static void BeforeUpdate()      { _scriptsTimer.Start();   }
		private static void AfterUpdate()       { _scriptsTimer.Stop();    }
		private static void BeforeLateUpdate()  { _scriptsTimer.Start();   }
		private static void AfterLateUpdate()   { _scriptsTimer.Stop();    }

		private static void PostLateUpdateFirst()
		{
			_renderTimer.Restart();
		}

		private static void PostLateUpdateLast()
		{
			_renderTimer.Stop();
			_frameTimer.Stop();

			LastScriptsTime = (float)_scriptsTimer.TotalTime.TotalSeconds;
			LastRenderTime  = (float)_renderTimer.TotalTime.TotalSeconds;
			LastFrameTime   = (float)_frameTimer.TotalTime.TotalSeconds;
		}

		private static void OnApplicationQuit()
		{
			PlayerLoopUtility.RemovePlayerLoopSystems(typeof(Profilers));
		}
	}
}
