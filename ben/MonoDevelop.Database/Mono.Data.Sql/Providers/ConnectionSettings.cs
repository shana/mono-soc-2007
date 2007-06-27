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
using System.Collections.Generic;

namespace Mono.Data.Sql
{
	[Serializable]
	public class ConnectionSettings
	{
		private string name;
		private string providerName;
		
		private string server;
		private int port;
		private string username;
		private string password;
		private bool savePassword;
		
		private string connectionString;
		private bool useConnectionString;
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public string ProviderName {
			get { return providerName; }
			set { providerName = value; }
		}
		
		public string Server {
			get { return server; }
			set { server = value; }
		}
		
		public int Port {
			get { return port; }
			set { port = value; }
		}
		
		public string Username {
			get { return username; }
			set { username = value; }
		}
		
		public string Password {
			get { return password; }
			set { password = value; }
		}
		
		public bool SavePassword {
			get { return savePassword; }
			set { savePassword = value; }
		}
		
		public string ConnectionString {
			get { return connectionString; }
			set { connectionString = value; }
		}
		
		public bool UseConnectionString {
			get { return useConnectionString; }
			set { useConnectionString = value; }
		}
	}
}
