//
// NFS Server
//
// Copyright (c) 2004-2012 Pete Barber
//
// Licensed under the The Code Project Open License (CPOL.html)
// http://www.codeproject.com/info/cpol10.aspx 
//
using System;

namespace nfsV2
{
	/// <summary>
	/// Summary description for NFSStatusException.
	/// </summary>
	public class NFSStatusException : System.ApplicationException
	{
		private NFSStatus status;

		public NFSStatusException (NFSStatus status)
		{
			this.status = status;
		}

		public NFSStatus Status
		{
			get
			{
				return status;
			}
		}
	}
}
