namespace ExitGames.Client.Photon
{
	internal static class SerializationProtocolFactory
	{
		internal static IProtocol Create(SerializationProtocol serializationProtocol)
		{
			return new Protocol16();
		}
	}
}
