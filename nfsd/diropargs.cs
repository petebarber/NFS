//
// Copyright 2004 Pete Barber, All Rights Reserved
//
using System;
using RPCV2Lib;

namespace nfsV2
{
	/// <summary>
	/// Summary description for diropargs.
	/// </summary>
	public class diropargs
	{
		fhandle fh;
		String	fileName;

		public diropargs(rpcCracker cracker)
		{
			fh			= new fhandle(cracker);
			fileName	= cracker.get_String();
		}

		public fhandle DirHandle
		{
			get
			{
				return fh;
			}

		}

		public String FileName
		{
			get
			{
				return fileName;
			}
		}
	}
}
