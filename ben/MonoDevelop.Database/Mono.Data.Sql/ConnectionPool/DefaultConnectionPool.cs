//
// Authors:
//	Ben Motmans  <ben.motmans@gmail.com>
//
// Copyright (c) 2007 Ben Motmans
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Data;
using System.Collections.Generic;

namespace Mono.Data.Sql
{
	public class DefaultConnectionPool : IConnectionPool
	{
		protected int minSize = 1;
		protected int maxSize = 20;
		protected int growSize = 3;
		protected int shrinkSize = 5;
		
		protected IDbFactory factory;
		protected ConnectionSettings settings;
		protected IConnectionProvider connectionProvider;
		
		protected bool isInitialized;
		
		protected List<IPooledDbConnection> connections;
		protected Queue<IPooledDbConnection> freeConnections;
		
		protected object sync = new object ();
		
		public DefaultConnectionPool (IDbFactory factory, IConnectionProvider connectionProvider, ConnectionSettings settings)
		{
			if (factory == null)
				throw new ArgumentNullException ("factory");
			if (connectionProvider == null)
				throw new ArgumentNullException ("connectionProvider");
			if (settings == null)
				throw new ArgumentNullException ("settings");
			
			this.factory = factory;
			this.connectionProvider = connectionProvider;
			this.settings = settings;
			
			connections = new List<IPooledDbConnection> ();
			freeConnections = new Queue<IPooledDbConnection> ();
		}
		
		public virtual IDbFactory DbFactory {
			get { return factory; }
		}
		
		public virtual ConnectionSettings ConnectionSettings  {
			get { return settings; }
		}
		
		public virtual IConnectionProvider ConnectionProvider {
			get { return connectionProvider; }
		}
		
		public virtual bool IsInitialized {
			get { return isInitialized; }
		}
		
		public virtual int MinSize {
			get { return minSize; }
			set {
				if (value < 1 || value > maxSize)
					throw new IndexOutOfRangeException ("MinSize");
				minSize = value;
			}
		}
		
		public virtual int MaxSize {
			get { return maxSize; }
			set {
				if (value < minSize)
					throw new IndexOutOfRangeException ("MaxSize");
				maxSize = value;
			}
		}

		public virtual int GrowSize {
			get { return growSize; }
			set {
				if (value < 1 || value > 10)
					throw new IndexOutOfRangeException ("GrowSize");
				growSize = value;
			}
		}

		public virtual int ShrinkSize {
			get { return shrinkSize; }
			set {
				if (value < 1 || value > 10)
					throw new IndexOutOfRangeException ("GrowSize");
				shrinkSize = value;
			}
		}
		
		public virtual int ConnectionCount {
			get {
				lock (sync)
					return connections.Count + freeConnections.Count;
			}
		}

		public virtual int FreeConnectionCount {
			get {
				lock (sync)
					return freeConnections.Count;
			}
		}

		public virtual bool HasFreeConnection {
			get {
				lock (sync)
					return freeConnections.Count > 0;
			}
		}
		
		public virtual IPooledDbConnection Request ()
		{
			IPooledDbConnection conn = null;
			if (HasFreeConnection) {
				lock (sync) {
					conn = freeConnections.Dequeue ();
					connections.Add (conn);
				}
			} else {
				if (Grow ())
					conn = Request ();
				else
					conn = null;
			}
			return conn;
		}

		public virtual void Release (IPooledDbConnection connection)
		{
			lock (sync) {
				if (freeConnections.Contains (connection))
					return;
				
				if (connectionProvider.CheckConnection (connection, settings))
					freeConnections.Enqueue (connection);
				connections.Remove (connection);
			}
			Shrink ();
		}
		
		public virtual bool Initialize ()
		{
			for (int i=0; i<minSize; i++) {
				IPooledDbConnection conn = connectionProvider.CreateConnection (this, settings);
				if (conn == null)
					return false;
				
				lock (sync)
					freeConnections.Enqueue (conn);
			}
			isInitialized = true;
			return true;
		}
		
		public virtual void Close ()
		{
			lock (sync) {
				foreach (IPooledDbConnection conn in connections)
					conn.Dispose ();
				foreach (IPooledDbConnection conn in freeConnections)
					conn.Dispose ();
			
				isInitialized = false;
			}
		}
					
		protected virtual bool Grow ()
		{
			for (int i=0; i<growSize; i++) {
				IPooledDbConnection conn = connectionProvider.CreateConnection (this, settings);
				if (conn == null)
					return false;
				
				lock (sync)
					freeConnections.Enqueue (conn);
			}
			return true;
		}
					
		protected virtual void Shrink ()
		{
			if (FreeConnectionCount < shrinkSize)
				return;
			
			for (int i=0; i<shrinkSize; i++) {
				if (HasFreeConnection) {
					lock (sync) {
						IPooledDbConnection conn = freeConnections.Dequeue ();
						conn.Destroy ();
					}
				}
			}
		}
	}
}