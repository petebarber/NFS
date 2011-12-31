//
// Copyright 2004 Pete Barber, All Rights Reserved
//
using System;
using System.Threading;

using RPCV2Lib;
using portmapperV1;
using mountV1;
using nfsV2;

namespace NFS
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class nfs
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			FileTable fileHandles = new FileTable(1024);

			Thread portMapper = new Thread(new ThreadStart(new portmapper().Run));
			Thread mountD = new Thread(new ThreadStart(new mountd().Run));
			Thread nfsD = new Thread(new ThreadStart(new nfsd().Run));

			portMapper.Start();
			mountD.Start();
			nfsD.Start();

			nfsD.Join();
			mountD.Join();
			portMapper.Join();
		}
	}
}
