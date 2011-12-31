//
// Copyright 2004 Pete Barber, All Rights Reserved
//
using System;

using RPCV2Lib;

namespace portmapperV1
{
	/// <summary>
	/// Summary description for TCPPortMapper.
	/// </summary>
	public class portmapper : rpcd
	{
		public portmapper() : base(Ports.portmapper, Progs.portmapper)
		{
		}

		protected override void Proc(uint proc, rpcCracker cracker, rpcPacker packer)
		{
			switch (proc)
			{
				case 3:	// GetPort
					GetPort(cracker, packer);
					break;
				default:
					throw new BadProc();
			}
		}

		private void GetPort(rpcCracker cracker, rpcPacker packer)
		{
			const uint IPPROTO_UDP = 17;

			uint prog = cracker.get_uint32();
			uint vers = cracker.get_uint32();
			uint prot = cracker.get_uint32();
			uint port = cracker.get_uint32();

			Console.WriteLine("prog:{0}, vers:{1}, prot:{2}, port:{3}", prog, vers, prot, port);

			uint registeredPort = 0;

			if (prot == IPPROTO_UDP)
			{
				if (prog == (uint)Progs.mountd && vers == (uint)Vers.mountd)
				{
					registeredPort = (uint)Ports.mountd;
				}
				else if (prog == (uint)Progs.nfsd && vers == (uint)Vers.nfsd)
				{
					registeredPort = (uint)Ports.nfsd;
				}
			}

			packer.setUint32(registeredPort);
		}
	}
}
