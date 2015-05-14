using System;

namespace Core.Net
{
	public static class IPAddress
	{
		public static long HostToNetworkOrder (long host)
		{
			#if BIGENDIAN
			return host;
			#else
			return (((long)HostToNetworkOrder ((int)host) & 0xFFFFFFFF) << 32)
			| ((long)HostToNetworkOrder ((int)(host >> 32)) & 0xFFFFFFFF);
			#endif
		}

		public static int HostToNetworkOrder (int host)
		{
			#if BIGENDIAN
			return host;
			#else
			return (((int)HostToNetworkOrder ((short)host) & 0xFFFF) << 16)
			| ((int)HostToNetworkOrder ((short)(host >> 16)) & 0xFFFF);
			#endif
		}

		public static short HostToNetworkOrder (short host)
		{
			#if BIGENDIAN
			return host;
			#else
			return (short)((((int)host & 0xFF) << 8) | (int)((host >> 8) & 0xFF));
			#endif
		}

		public static long NetworkToHostOrder (long network)
		{
			return HostToNetworkOrder (network);
		}

		public static int NetworkToHostOrder (int network)
		{
			return HostToNetworkOrder (network);
		}

		public static short NetworkToHostOrder (short network)
		{
			return HostToNetworkOrder (network);
		}

	}
}

