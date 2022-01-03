namespace Quantum.Profiling
{
	using Photon.Realtime;

    public sealed class PingProfiler : ValueSeriesProfiler
    {
		protected override void OnUpdate()
		{
			int ping = 0;

			LoadBalancingPeer peer = ProfilersUtility.GetNetworkPeer();
			if (peer != null)
			{
				ping = peer.RoundTripTime;
				if (ping > 9999)
				{
					ping = default;
				}
			}

			AddValue(ping);
		}
    }
}
