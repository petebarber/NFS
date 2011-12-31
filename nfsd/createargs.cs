//
// Copyright 2004 Pete Barber, All Rights Reserved
//
using System;
using RPCV2Lib;

namespace nfsV2
{
	/// <summary>
	/// Summary description for createargs.
	/// </summary>
	public class createargs
	{
		diropargs	where;
		sattr		attributes;

		public createargs(rpcCracker cracker)
		{
			where		= new diropargs(cracker);	
			attributes	= new sattr(cracker);
		}

		public diropargs Where
		{
			get
			{
				return where;
			}
		}

		public sattr Attributes
		{
			get
			{
				return attributes;
			}
		}
	}
}
