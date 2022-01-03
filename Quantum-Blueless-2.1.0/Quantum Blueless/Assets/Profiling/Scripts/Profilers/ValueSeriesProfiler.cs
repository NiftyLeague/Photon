namespace Quantum.Profiling
{
    public abstract class ValueSeriesProfiler : Profiler<ValueSeriesGraph>
    {
		private int _offset;
		private int _samples;

		protected override void OnActivated()
		{
			_offset  = 0;
			_samples = 0;
		}

		protected void AddValue(float value)
        {
			if (IsActive == false)
				return;

			float[] values = Values;
			values[_offset] = value;

			_offset = (_offset + 1) % values.Length;
			++_samples;

			Graph.SetValues(values, _offset , _samples);
        }
    }
}
