//
// Copyright 2004 Pete Barber, All Rights Reserved
//
using System;
using System.Net;
using System.Net.Sockets;

namespace RPCV2Lib
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public enum Progs { portmapper = 100000, mountd = 100005, nfsd = 100003 }
	public enum Ports { portmapper = 111, mountd = 635, nfsd = 2049 }
	public enum Vers { portmapper = 2, mountd = 1, nfsd = 2 }

	public abstract class rpcd
	{
		private UdpClient	conn;
		IPEndPoint			remoteHost;
		private uint		prog;

		static uint			count = 0;

		protected abstract void Proc(uint proc, rpcCracker cracker, rpcPacker reply);

		// Apparently some RPC servers pretend to be multiple server.
		// The case that prompted this was nfs which is also nfs_acl well
		// at least when mounted from a Solaris machine.  This virtual
		// function allows a test for additional RPC prog. numbers to be
		// performed.
		protected virtual bool Prog(uint prog)
		{
			return false;
		}

		public rpcd(Ports portNumber, Progs prog)
		{
			this.prog = (uint)prog;

			conn = new UdpClient(new IPEndPoint(IPAddress.Any, (int)portNumber));
		}

		public void Run()
		{
			while (true)
			{
				remoteHost = new IPEndPoint(IPAddress.Any, 0);

				Byte[] data = conn.Receive(ref remoteHost);

				Console.WriteLine("{0}: Received a connection from:{1}", prog, remoteHost.ToString());

				rpcCracker cracker = new rpcCracker(data);
				
				//cracker.Dump("Received");

				rpcPacker reply = CrackRPC(cracker);

				Console.WriteLine("{0}: Sending a reply to:{1}", prog, remoteHost.ToString());

				//reply.Dump("Sending");

				int sent = conn.Send(reply.Data, (int)reply.Length, remoteHost);

				if (sent != (int)reply.Length)
					Console.WriteLine("*** Didn't send all.  Length:{0}, sent:{1}", reply.Length, sent);
			}
		}

		private rpcPacker CrackRPC(rpcCracker cracker)
		{
			uint xid		= cracker.get_uint32();
			uint msg_type	= cracker.get_uint32();
			uint rpcvers	= cracker.get_uint32();
			uint prog		= cracker.get_uint32();
			uint vers		= cracker.get_uint32();
			uint proc		= cracker.get_uint32();

#if DEBUG
			Console.WriteLine("{0}> {1}: xid:{2}, type:{3}, rpcvers:{4}, prog:{5}, vers:{6}, proc:{7}", ++count, this.prog, xid, msg_type, rpcvers, prog, vers, proc);
#endif

			if (msg_type != 0)
				return GarbageArgsReply(xid);

			if (rpcvers != 2)
				return RPCMismatchReply(xid);

			if (this.prog != prog && Prog(prog) != true)
				return ProgMismatchReply(xid);

			CrackCredentials(cracker);
			CrackVerifier(cracker);

			try
			{
				rpcPacker reply = SuccessReply(xid);

				if (proc != 0)
					Proc(proc, cracker, reply);

				return reply;
			}
			catch (Exception e)
			{
				System.Console.WriteLine("Whoops: {0}", e);
				return ProcUnavilReply(xid);
			}
		}

		private void CrackCredentials(rpcCracker cracker)
		{
			uint flavor = cracker.get_uint32();
			uint length = cracker.get_uint32();

			//Console.WriteLine("{0}: Credentials.  flavor:{1}, length:{2}", prog, flavor, length);

			cracker.jump(length);
		}

		private void CrackVerifier(rpcCracker cracker)
		{
			uint flavor = cracker.get_uint32();
			uint length = cracker.get_uint32();

			//Console.WriteLine("{0}: Credentials.  flavor:{1}, length:{2}", prog, flavor, length);

			cracker.jump(length);
		}

		private rpcPacker NewAcceptReply(uint xid, uint acceptStatus)
		{
			rpcPacker reply = new rpcPacker();

			reply.setUint32(xid);
			reply.setUint32(1);		// rpc_msg.REPLY
			reply.setUint32(0);		// rpc_msg.reply_body.MSG_ACCEPTED
			reply.setUint32(0);		// rpc_msg.reply_body.accepted_reply.opaque_auth.NULL
			reply.setUint32(0);		// rpc_msg.reply_body.accepted_reply.opaque_auth.<datsize>

			// rpc_msg.reply_body.accepted_reply.<case>
			reply.setUint32(acceptStatus);		

			return reply;
		}

		private rpcPacker SuccessReply(uint xid)
		{
			return NewAcceptReply(xid, 0);
		}

		private rpcPacker ProgMismatchReply(uint xid)
		{
			rpcPacker reply = NewAcceptReply(xid, 2);

			reply.setUint32(prog);	// rpc_msg.reply_body.accepted_reply.mismatch_info.low
			reply.setUint32(prog);	// rpc_msg.reply_body.accepted_reply.mismatch_info.high

			return reply;
		}

		private rpcPacker ProcUnavilReply(uint xid)
		{
			return NewAcceptReply(xid, 3);
		}

		private rpcPacker GarbageArgsReply(uint xid)
		{
			return NewAcceptReply(xid, 4);
		}

		private rpcPacker RPCMismatchReply(uint xid)
		{
			rpcPacker reply = new rpcPacker();

			reply.setUint32(xid);
			reply.setUint32(1);		// rpc_msg.REPLY
			reply.setUint32(1);		// rpc_msg.reply_body.MSG_DENIED
			reply.setUint32(0);		// rpc_msg.reply_body.rejected_reply.RPC_MISMATCH
			reply.setUint32(2);		// rpc_msg.reply_body.rejected_reply.mismatch_info.low
			reply.setUint32(2);		// rpc_msg.reply_body.rejected_reply.mismatch_info.low

			return reply;
		}

	}

	public class BadProc : System.ApplicationException
	{
	}
}
