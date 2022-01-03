using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum {
  partial class RuntimePlayer {

    public AssetRefEntityPrototype PrototypeRef;
    public string PlayerName;
    public string BodyId;

    partial void SerializeUserData(BitStream stream)
    {
      stream.Serialize(ref PrototypeRef.Id.Value);
      stream.Serialize(ref PlayerName);
      stream.Serialize(ref BodyId);
    }
  }
}
