//
// NFS Server
//
// Copyright (c) 2004-2012 Pete Barber
//
// Licensed under the The Code Project Open License (CPOL.html)
// http://www.codeproject.com/info/cpol10.aspx 
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
