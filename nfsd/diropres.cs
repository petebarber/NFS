//
// Copyright 2004 Pete Barber, All Rights Reserved
//
using System;
using RPCV2Lib;

namespace nfsV2
{
	/// <summary>
	/// Summary description for diropres.
	/// </summary>
	public class diropres
	{
		static public void PackSuccess(rpcPacker packer, fhandle fh, fattr attr)
		{
			packer.setUint32((uint)NFSStatus.NFS_OK);
			fh.Pack(packer);
			attr.Pack(packer);
		}

		static public void PackError(rpcPacker packer, NFSStatus error)
		{
			packer.setUint32((uint)error);
		}
	}
}
