//
// Copyright 2004 Pete Barber, All Rights Reserved
//
using System;
using RPCV2Lib;

namespace nfsV2
{
	/// <summary>
	/// Summary description for sattr.
	/// </summary>
	public class sattr
	{
		uint	mode;
		uint	uid;
		uint	gid;
		uint	size;
		timeval	atime;
		timeval mtime;

		public sattr(rpcCracker cracker)
		{
			mode	= cracker.get_uint32();
			uid		= cracker.get_uint32();
			gid		= cracker.get_uint32();
			size	= cracker.get_uint32();
			atime	= new timeval(cracker);
			mtime	= new timeval(cracker);
		}

		public uint Mode
		{
			get
			{
				return mode;
			}
		}

		public uint Size
		{
			get
			{
				return size;
			}
		}

		public timeval AccessTime
		{
			get
			{
				return atime;
			}
		}

		public timeval ModTime
		{
			get
			{
				return mtime;
			}
		}
	}
}
