namespace Quantum.Profiling
{
    public sealed class SimulationTimeProfiler : ValueSeriesProfiler
    {
		protected override void OnUpdate()
		{
			float updateTime = 0.0f;

			QuantumRunner quantumRunner = QuantumRunner.Default;
			if (quantumRunner != null && quantumRunner.Game != null && quantumRunner.Game.Session != null)
			{
				updateTime = (float)quantumRunner.Game.Session.Stats.UpdateTime;
			}

			AddValue(updateTime);
		}

		protected override void OnTargetFPSChaged(int fps)
		{
			float frameMs = 1.0f / fps;
			Graph.SetThresholds(frameMs * 0.25f, frameMs * 0.375f, frameMs * 0.5f);
		}
    }
}
