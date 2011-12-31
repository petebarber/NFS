//
// Copyright 2004 Pete Barber, All Rights Reserved
//
using System;
using System.IO;
using RPCV2Lib;

namespace nfsV2
{
	/// <summary>
	/// Summary description for readdirres.
	/// </summary>
	public class entry
	{
		private uint	fileid;
		private string	name;
		private uint	cookie;

		// Special constructor for "." and ".."
		public entry(string name, string fullName, uint cookie, uint fileid)
		{
			this.fileid = fileid;
			this.name	= name;
			this.cookie	= cookie;

			//Console.WriteLine("entry fileid:{0,5}, cookie:{1,5}, name:{2}", fileid, cookie, name);
		}

		public entry(FileSystemInfo info, uint cookie)
		{
			this.name = info.Name;

			if (info.Attributes != FileAttributes.Directory && 
				info.Attributes != FileAttributes.Device	&& 
				info.Extension == ".sl")
				this.name = this.name.Remove(this.name.Length - 3, 3);

			this.cookie	= cookie;

			fhandle fh;

			if ((fh = FileTable.LookupFileHandle(info.FullName)) == null)
				fh = FileTable.Add(new FileEntry(info.FullName));

			this.fileid = fh.Index;

			//Console.WriteLine("entry fileid:{0,5}, cookie:{1,5}, name:{2}", fileid, cookie, name);
		}

		public void Pack(rpcPacker packer)
		{
			//Console.WriteLine("entry pack name:{0}", name);

			packer.setUint32(fileid);
			packer.setString(name);
			packer.setUint32(cookie);
		}

		public uint Size
		{
			get
			{
				return 12 + rpcPacker.sizeOfString(name);
			}
		}
	}

	public class readdirres
	{
		entry[] entries;

		public readdirres(string dirName, uint count)
		{
			DirectoryInfo dir = new DirectoryInfo(dirName);

			FileSystemInfo[] files = dir.GetFileSystemInfos();

			entries = new entry[files.Length + 2];


			// Don't create new entries in FileTable for "." and "..".  Find the real dirs. and use those id's
			uint dirFileId = FileTable.LookupFileHandle(dirName).Index;

			entries[0] = new entry(".", dirName + @"\.", 1, dirFileId);

			if (dirFileId == 1) // root
				entries[1] = new entry("..", dirName + @"\..", files.Length == 0 ? count : 2, 1);
			else
				entries[1] = new entry("..", dirName + @"\..", files.Length == 0 ? count : 2, FileTable.LookupFileHandle(dir.Parent.FullName).Index);


			uint i = 2;

			foreach (FileSystemInfo file in files)
				if (files.Length == i - 1)
					entries[i] = new entry(file, count);
				else
					entries[i] = new entry(file, ++i);
		}

		public bool Pack(rpcPacker packer, uint cookie, uint count)
		{

			packer.setUint32((uint)NFSStatus.NFS_OK);

      		uint size = 8;	// First pointer + EOF

			if (cookie >= entries.Length)
			{
				// nothing
			}
			else
			{
				do
				{
					entry next = entries[cookie];

					if (size + next.Size > count)
						break;
					else
						size += next.Size;
	
					// true as in yes, more follows.  This is *entry.
					packer.setUint32(1);

					next.Pack(packer);
				}
				while (++cookie < entries.Length);
			}

			// false as in no more follow.  This is *entry.
			// Unlike EOF which is set only when all entries have been sent
			// *entry is reset to false following the last entry in each
			// batch.
			packer.setUint32(0);

			//Console.WriteLine("ReadDir: Pack done.  cookie:{0}, size:{1}", cookie, size);

			// EOF
			if (cookie >= entries.Length)
			{
				packer.setUint32((uint)1);	// yes
				return true;
			}
			else
			{
				packer.setUint32((uint)0);	// no
				return false;
			}
		}
	}
}
