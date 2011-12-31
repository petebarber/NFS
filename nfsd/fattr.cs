//
// NFS Server
//
// Copyright (c) 2004-2012 Pete Barber
//
// Licensed under the The Code Project Open License (CPOL.html)
// http://www.codeproject.com/info/cpol10.aspx 
//
using System;
using System.IO;
using RPCV2Lib;

namespace nfsV2
{
	/// <summary>
	/// Summary description for fattr.
	/// </summary>
	public class fattr
	{
		public enum ftype : uint
		{
			NFNON = 0,
			NFREG = 1,
			NFDIR = 2,
			NFBLK = 3,
			NFCHR = 4,
			NFLNK = 5
		}

		[Flags]
		public enum modes : uint
		{
			DIR = 16384,
			CHR = 8192,
			BLK = 24576,
			REG = 32768,
			LNK = 40960,
			NON = 49152,
			SUID	= 2048,
			SGID	= 1024,
			SWAP	= 512,
			ROWN	= 256,
			WOWN	= 128,
			XOWN	= 64,
			RGRP	= 32,
			WGRP	= 16,
			XGRP	= 8,
			ROTH	= 4,
			WOTH	= 2,
			XOTH	= 1
		}

		ftype	type;
		uint	mode		= (uint)(modes.XOWN | modes.XGRP | modes.XOTH | modes.ROWN | modes.RGRP | modes.ROTH);
		uint	nlink		= 1;
		uint	uid			= 0;
		uint	gid			= 0;
		uint	size		= 0;
		uint	blocksize	= 0;
		uint	rdev		= 0;
		uint	blocks		= 0;
		uint	fsid		= 1;
		uint	fileid		= 0;
		timeval	atime;
		timeval mtime;

		public fattr(fhandle fh)
		{
			FileEntry file = FileTable.LookupFileEntry(fh);

			if (file == null)
			{
				Console.WriteLine("fattr on invalid file handle:{0}", fh.Index);
				throw new NFSStatusException(NFSStatus.NFSERR_STALE);
			}

			FileInfo fileInfo = new FileInfo(file.Name);

			if (fileInfo.Exists == false)
				if (new DirectoryInfo(file.Name).Exists == false)
					throw new System.IO.FileNotFoundException();

			if ((fileInfo.Attributes & FileAttributes.Directory) != 0)
			{
				type = ftype.NFDIR;
				mode |= (uint)modes.DIR;
				size = 4096;
				blocksize = 4096;
				blocks = 8;
				atime = new timeval(fileInfo.LastAccessTime);
				mtime = new timeval(fileInfo.LastWriteTime);
			}
			else
			{
				if (fileInfo.Extension == ".sl")
				{
					type |= ftype.NFLNK;
					mode = (uint)modes.LNK;
				}
				else
				{
					type = ftype.NFREG;
					mode |= (uint)modes.REG;
				}

				size	= (uint)fileInfo.Length;
				blocks	= (size / 4096) + (4096 - (size % 4096));
				atime	= new timeval(fileInfo.LastAccessTime);
				mtime	= new timeval(fileInfo.LastWriteTime);
			}

			if ((fileInfo.Attributes & FileAttributes.ReadOnly) == 0)
				mode |= (uint)modes.WOWN;

			fileid = fh.Index;

			//Console.WriteLine("fattr name:{0}, fileid:{1}, attrs:{2}, readonly:{3}", file.Name, fileid, fileInfo.Attributes, (fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
		}

		public bool IsFile()
		{
			return type != ftype.NFDIR;
		}

		public void Pack(rpcPacker packer)
		{
			packer.setUint32((uint)type);
			packer.setUint32(mode);
			packer.setUint32(nlink);
			packer.setUint32(uid);
			packer.setUint32(gid);
			packer.setUint32(size);
			packer.setUint32(blocksize);
			packer.setUint32(rdev);
			packer.setUint32(blocks);
			packer.setUint32(fsid);
			packer.setUint32(fileid);
			atime.Pack(packer);
			mtime.Pack(packer);
			mtime.Pack(packer);
		}
	}
}
