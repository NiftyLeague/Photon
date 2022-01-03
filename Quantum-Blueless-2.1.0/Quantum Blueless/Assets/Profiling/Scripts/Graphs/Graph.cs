#pragma warning disable 649

namespace Quantum.Profiling
{
	using UnityEngine;
	using UnityEngine.UI;

    public abstract class Graph : MonoBehaviour
    {
		private const string SHADER_PROPERTY_VALUES  = "_Values";
		private const string SHADER_PROPERTY_SAMPLES = "_Samples";

		public int Samples { get { return _samples; } }

		[SerializeField]
		protected Image _targetImage;
		[SerializeField][Range(60, 540)]
		protected int _samples = 300;

        protected float[] _values;
		protected Material _material;
        protected int _valuesShaderPropertyID;

		protected virtual void OnInitialize() {}
		protected virtual void OnRestore()    {}

		public void Initialize()
        {
            _valuesShaderPropertyID = Shader.PropertyToID(SHADER_PROPERTY_VALUES);

			_values = new float[_samples];

			_material = new Material(_targetImage.material);
            _targetImage.material = _material;

			Restore();

			OnInitialize();
        }

		public virtual void SetValues(float[] values, int offset, int samples)
		{
			if (_values == null || values == null || _values.Length != values.Length)
				return;

			for (int i = 0; i < _samples; ++i, ++offset)
			{
				offset %= _samples;

				_values[i] = values[offset];
			}

			_material.SetFloatArray(_valuesShaderPropertyID, _values);
		}

		public void Restore()
		{
			if (_material != null)
			{
				_material.SetInt(SHADER_PROPERTY_SAMPLES, _samples);
				_material.SetFloatArray(_valuesShaderPropertyID, _values);
			}

			OnRestore();
		}
	}
}
