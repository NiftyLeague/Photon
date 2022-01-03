namespace Quantum.Profiling
{
    public sealed class RenderTimeProfiler : ValueSeriesProfiler
    {
		protected override void OnUpdate()
        {
			AddValue(Profilers.LastRenderTime);
        }

		protected override void OnTargetFPSChaged(int fps)
		{
			float frameMs = 1.0f / fps;
			Graph.SetThresholds(frameMs * 0.75f, frameMs, frameMs * 1.5f);
		}
	}
}
