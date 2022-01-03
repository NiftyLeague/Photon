#pragma warning disable 649

namespace Quantum.Profiling
{
	using UnityEngine;

    public sealed class MarkerSeriesGraph : Graph
    {
		public void SetColors(params Color[] colors)
		{
			for (int i = 0; i < colors.Length; ++i)
			{
				_material.SetColor(string.Format("_Marker{0}Color", i + 1), colors[i]);
			}
		}
	}
}
