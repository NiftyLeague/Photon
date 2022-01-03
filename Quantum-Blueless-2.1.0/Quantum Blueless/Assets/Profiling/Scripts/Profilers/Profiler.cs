#pragma warning disable 649

namespace Quantum.Profiling
{
	using UnityEngine;

    public abstract class Profiler<TGraph> : MonoBehaviour where TGraph : Graph
    {
		protected TGraph  Graph    { get; private set; }
		protected float[] Values   { get; private set; }
		protected bool    IsActive { get; private set; }

		[SerializeField]
		private bool       _enableOnAwake;
		[SerializeField]
		private GameObject _renderObject;

		private int        _targetFPS;

		public void ToggleVisibility()
		{
			SetState(!IsActive);
		}

		protected virtual void OnInitialize()             {}
		protected virtual void OnDeinitialize()           {}
		protected virtual void OnActivated()              {}
		protected virtual void OnDeactivated()            {}
		protected virtual void OnUpdate()                 {}
		protected virtual void OnRestore()                {}
		protected virtual void OnTargetFPSChaged(int fps) {}

		private void Awake()
		{
			Graph  = GetComponentInChildren<TGraph>(true);
			Values = new float[Graph.Samples];

			Graph.Initialize();

			OnInitialize();

			SetState(_enableOnAwake);
		}

		private void Update()
		{
			if (_targetFPS != Application.targetFrameRate)
			{
				_targetFPS = Application.targetFrameRate;

				OnTargetFPSChaged(_targetFPS > 0 ? _targetFPS : 60);
			}

			OnUpdate();
		}

		private void OnDestroy()
		{
			OnDeinitialize();
		}

		private void OnApplicationFocus(bool focus)
		{
			Graph.Restore();
		}

		private void SetState(bool isActive)
		{
			IsActive = isActive;

			_renderObject.SetActive(isActive);

			if (isActive == true)
			{
				Restore();
				OnActivated();
			}
			else
			{
				OnDeactivated();
				Restore();
			}
		}

		private void Restore()
		{
			if (Values != null)
			{
				System.Array.Clear(Values, 0, Values.Length);
			}

			if (Graph != null)
			{
				Graph.Restore();
			}

			OnRestore();
		}
	}
}
