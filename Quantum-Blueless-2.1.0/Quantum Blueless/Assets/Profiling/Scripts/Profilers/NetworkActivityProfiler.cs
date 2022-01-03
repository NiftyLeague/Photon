namespace Quantum.Profiling
{
	using ExitGames.Client.Photon;
	using Photon.Realtime;

    public sealed class NetworkActivityProfiler : ValueSeriesProfiler
    {
		protected override void OnUpdate()
		{
			float interval = 0;

			LoadBalancingPeer peer = ProfilersUtility.GetNetworkPeer();
			if (peer != null)
			{
				interval = SupportClass.GetTickCount() - peer.TimestampOfLastSocketReceive;
				if (interval > 9999)
				{
					interval = default;
				}
			}

			AddValue(interval * 0.001f);
		}

		protected override void OnTargetFPSChaged(int fps)
		{
			float frameMs = 1.0f / fps;
			Graph.SetThresholds(frameMs * 2.0f, frameMs * 4.0f, frameMs * 8.0f);
		}
	}
}
