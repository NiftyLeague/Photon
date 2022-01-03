namespace Quantum.Profiling
{
	using Photon.Realtime;

	public static class ProfilersUtility
	{
		public static LoadBalancingPeer GetNetworkPeer()
		{
			QuantumRunner quantumRunner = QuantumRunner.Default;
			if (quantumRunner != null && quantumRunner.NetworkClient != null)
			{
				return quantumRunner.NetworkClient.LoadBalancingPeer;
			}

			return null;
		}
	}
}
