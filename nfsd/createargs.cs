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
