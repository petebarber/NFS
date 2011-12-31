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
	/// Summary description for attrstat.
	/// </summary>
	public class attrstat
	{
		static public void PackSuccess(rpcPacker packer, fattr attr)
		{
			packer.setUint32((uint)NFSStatus.NFS_OK);
			attr.Pack(packer);
		}

		static public void PackError(rpcPacker packer, NFSStatus error)
		{
			packer.setUint32((uint)error);
		}
	}
}
