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
using System.Collections;

namespace RPCV2Lib
{
	public class HandleTable
	{
		private Object[]	objects;
		private Stack		free = new Stack();
		private uint		next = 1;

		public HandleTable(int size)
		{
			objects = new Object[size];
		}

		public Object this[uint i]
		{
			get
			{
				return objects[i];
			}
		}

		public uint Add(Object obj)
		{
			uint i;

			if (free.Count > 0)
			{
				i = (uint)free.Pop();
			}
			else
			{
				if (next == objects.Length)
				{
					Object[] newObjects = new Object[next * 2];

					objects.CopyTo(newObjects, 0);

					objects = newObjects;
				}

				i = next;
				++next;
			}

			objects[i] = obj;

#if DEBUG
			Console.WriteLine("HandleTable.Add:{0}", i);
#endif

			return (uint)i;
		}

		public void Remove(uint fh)
		{
#if DEBUG
			Console.WriteLine("HandleTable.Remove:{0}", fh);
#endif
			objects[fh] = null;
			free.Push(fh);
		}

		public uint Length
		{
			get
			{
				return next;
			}
		}

	}

	public class fhandle
	{
		private uint index;

		public fhandle(uint index)
		{
			this.index = index;
		}

		public fhandle(rpcCracker cracker)
		{
			index	= cracker.get_uint32();

			cracker.get_uint32();
			cracker.get_uint32();
			cracker.get_uint32();
			cracker.get_uint32();
			cracker.get_uint32();
			cracker.get_uint32();
			cracker.get_uint32();
		}

		public void Pack(rpcPacker packer)
		{
			packer.setUint32(index);

			// Pad
			packer.setUint32(0);
			packer.setUint32(0);
			packer.setUint32(0);
			packer.setUint32(0);
			packer.setUint32(0);
			packer.setUint32(0);
			packer.setUint32(0);
		}

		public uint Index
		{
			get
			{
				return index;
			}
		}
	}

	public class FileEntry
	{
		private String			name;

		public FileEntry(String name)
		{
			this.name = name;
		}

		public String Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
	}

	public class FileTable
	{
		static private HandleTable files;

		public FileTable(int size)
		{
			files = new HandleTable(size);
		}

		public static FileEntry LookupFileEntry(fhandle fh)
		{
			try
			{
				return (FileEntry)files[fh.Index];
			}
			catch (IndexOutOfRangeException)
			{
#if DEBUG
				Console.WriteLine("LookupFileEntry({0}) failed", fh.Index);
#endif
				return null;
			}
		}

		public static fhandle LookupFileHandle(string name)
		{
			for (uint i = 0; i < files.Length; ++i)
			{
				Object o = files[i];

				if (o != null && name == ((FileEntry)o).Name)
					return new fhandle(i);
			}

#if DEBUG
			Console.WriteLine("LookupFileHandle({0}) failed", name);
#endif

			return null;
		}

		public static fhandle Add(FileEntry file)
		{
			return new fhandle(files.Add(file));
		}

		public static void Rename(string from, string to)
		{
			fhandle fhFrom	= LookupFileHandle(from);
			fhandle fhTo	= LookupFileHandle(to);

			if (fhFrom != null && fhTo == null)
			{
				// Most likely
				LookupFileEntry(fhFrom).Name = to;
			}
			else if (fhFrom != null && fhTo != null)
			{
				// Next most likely
				Remove(fhTo);
				LookupFileEntry(fhFrom).Name = to;
			}
			if (fhFrom == null && fhTo != null)
			{
				// Nothing to do
			}
			else if (fhFrom == null && fhTo == null)
			{
				Add(new FileEntry(to));
			}
		}

		public static void Remove(fhandle fh)
		{
			files.Remove(fh.Index);
		}
	}
}
