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
	public class ConnectionContext
	{
		private ConnectionSettings settings;
		private IConnectionProvider connProvider;
		private ISchemaProvider schemaProvider;

		public ConnectionContext (ConnectionSettings settings)
		{
			this.settings = settings;
		}
		
		public ConnectionSettings ConnectionSettings {
			get { return settings; }
		}
		
		public IConnectionProvider ConnectionProvider {
			get {
				if (connProvider == null)
					connProvider = DbFactoryService.CreateConnectionProvider (settings);
				return connProvider;
			}
		}
		
		public ISchemaProvider SchemaProvider {
			get {
				if (ConnectionProvider != null) {
					if (schemaProvider == null)
						schemaProvider = DbFactoryService.CreateSchemaProvider (settings, connProvider);
					return schemaProvider;
				}
				return null;
			}
		}
	}
}
