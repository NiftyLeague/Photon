namespace Quantum.Profiling
{
	using System;
	using System.Diagnostics;
	using System.Runtime.CompilerServices;

	public sealed class Timer
	{
		public bool     IsRunning { get { return _isRunning; } }
		public int      Counter   { get { return _counter;   } }
		public string   Name      { get { return _name;      } }

		public TimeSpan TotalTime   { get { if (_isRunning == true) { Update(); } return new TimeSpan(_totalTicks);  } }
		public TimeSpan RecentTime  { get { if (_isRunning == true) { Update(); } return new TimeSpan(_recentTicks); } }
		public TimeSpan PeakTime    { get { if (_isRunning == true) { Update(); } return new TimeSpan(_peakTicks);   } }

		private bool   _isRunning;
		private int    _counter;
		private string _name;

		private long   _startTicks;
		private long   _baseTicks;
		private long   _peakTicks;
		private long   _recentTicks;
		private long   _totalTicks;

		public Timer() : this(null)
		{
		}

		public Timer(string name)
		{
			_name = name;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Start()
		{
			if (_isRunning == false)
			{
				_isRunning  = true;

				_startTicks = Stopwatch.GetTimestamp();
				_baseTicks  = _startTicks;

				++_counter;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Stop()
		{
			if (_isRunning == true)
			{
				_isRunning = false;

				Update();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Restart()
		{
			_isRunning    = true;
			_counter      = 1;

			_startTicks   = Stopwatch.GetTimestamp();
			_baseTicks    = _startTicks;
			_peakTicks    = 0;
			_recentTicks  = 0;
			_totalTicks   = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			_isRunning    = false;
			_counter      = 0;

			_startTicks   = 0;
			_baseTicks    = 0;
			_peakTicks    = 0;
			_recentTicks  = 0;
			_totalTicks   = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Return()
		{
			Return(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetTotalMilliseconds()
		{
			return (float)TotalTime.TotalMilliseconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetRecentMilliseconds()
		{
			return (float)RecentTime.TotalMilliseconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetPeakMilliseconds()
		{
			return (float)PeakTime.TotalMilliseconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Timer Get(bool start = false)
		{
			Timer timer = Pool<Timer>.Get();
			if (start == true)
			{
				timer.Restart();
			}
			return timer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return(Timer timer)
		{
			timer.Reset();
			Pool<Timer>.Return(timer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Update()
		{
			long ticks = Stopwatch.GetTimestamp();

			_recentTicks = ticks - _startTicks;
			if (_recentTicks > _peakTicks)
			{
				_peakTicks = _recentTicks;
			}

			_totalTicks += ticks - _baseTicks;
			_baseTicks = ticks;

			if (_totalTicks < 0L)
			{
				_totalTicks = 0L;
			}
		}
	}
}
