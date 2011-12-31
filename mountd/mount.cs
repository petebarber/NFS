//
// NFS Server
//
// Copyright (c) 2004-2012 Pete Barber
//
// Licensed under the The Code Project Open License (CPOL.html)
// http://www.codeproject.com/info/cpol10.aspx 
//
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

using RPCV2Lib;

namespace mountV1
{
	/// <summary>
	/// Summary description for server.
	/// </summary>
	public class mountd : rpcd
	{
		private enum Procs : uint
		{
			Null		= 0,
			Mount		= 1,
			Dump		= 2,
			UMount		= 3,
			UMountAll	= 4,
			Export		= 5
		}

		public mountd() : base(Ports.mountd, Progs.mountd)
		{
		}

		protected override void Proc(uint proc, rpcCracker cracker, rpcPacker reply)
		{
			switch (proc)
			{
				case (uint)Procs.Null:
					throw new BadProc();
				case (uint)Procs.Mount:
					Mount(cracker, reply);
					break;
				case (uint)Procs.Dump:
					throw new BadProc();
				case (uint)Procs.UMount:
					UMount(cracker, reply);
					break;
				case (uint)Procs.UMountAll:
					throw new BadProc();
				case (uint)Procs.Export:
					throw new BadProc();
				default:
					throw new BadProc();
			}
		}

		private void Mount(rpcCracker cracker, rpcPacker reply)
		{
			uint length = cracker.get_uint32();

			string dirPath = "";

			for (uint i = 0; i < length; ++i)
				dirPath += cracker.get_char();

			Console.WriteLine("Mount {0}:{1}", length, dirPath);


			if (Directory.Exists(dirPath) == false)
			{
				reply.setUint32(2);	// Errno for no such file or directory
				reply.setUint32(0);	// Where fh would go
			}
			else
			{
				fhandle fh = FileTable.Add(new FileEntry(dirPath));

				reply.setUint32(0);		// Success

				fh.Pack(reply);
			}
		}

		private void UMount(rpcCracker cracker, rpcPacker reply)
		{
			uint length = cracker.get_uint32();

			string dirPath = "";

			for (uint i = 0; i < length; ++i)
				dirPath += cracker.get_char();

			Console.WriteLine("UMount {0}:{1}", length, dirPath);
#if FOO
			uint fh = fileHandles.Find(

			if (fileHandles.Remove(dirPath) == false)
				Console.WriteLine("{0} not mounted", dirPath);
#endif
		}
	}
}
