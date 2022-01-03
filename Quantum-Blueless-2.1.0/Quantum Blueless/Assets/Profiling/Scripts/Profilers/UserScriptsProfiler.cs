namespace Quantum.Profiling
{
    public sealed class UserScriptsProfiler : ValueSeriesProfiler
    {
		protected override void OnUpdate()
		{
			AddValue(Profilers.LastScriptsTime);
		}

		protected override void OnTargetFPSChaged(int fps)
		{
			float frameMs = 1.0f / fps;
			Graph.SetThresholds(frameMs * 0.5f, frameMs * 0.75f, frameMs);
		}
    }
}
