namespace Quantum.Profiling
{
    public abstract class MarkerSeriesProfiler : Profiler<MarkerSeriesGraph>
    {
		private int _offset;
		private int _samples;

		protected override void OnActivated()
		{
			_offset  = 0;
			_samples = 0;
		}

		protected void SetMarkers(params bool[] markers)
        {
			if (IsActive == false)
				return;

			int value = 0;

			for (int i = 0; i < markers.Length; ++i)
			{
				if (markers[i] == true)
				{
					value |= 1 << i;
				}
			}

			float[] values = Values;
			values[_offset] = value;

			_offset = (_offset + 1) % values.Length;
			++_samples;

			Graph.SetValues(values, _offset , _samples);
        }
    }
}
