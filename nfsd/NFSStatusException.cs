//
// Copyright 2004 Pete Barber, All Rights Reserved
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
