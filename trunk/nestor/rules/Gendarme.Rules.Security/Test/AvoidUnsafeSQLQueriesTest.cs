//
// Unit Test for AvoidUnsafeSQLQueries Rule.
//
// Authors:
//      Néstor Salceda <nestor.salceda@gmail.com>
//
//      (C) 2007 Néstor Salceda
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Reflection;

using System.Data;
using System.Data.SqlClient;

using Mono.Cecil;
using NUnit.Framework;
using Gendarme.Framework;
using Gendarme.Rules.Security;

namespace Test.Rules.Security {

	[TestFixture]
	public class AvoidUnsafeSQLQueriesTest {
		private IMethodRule rule;
		private AssemblyDefinition assembly;
		private MethodDefinition method;
		private MessageCollection messageCollection;

		private void PrintAllValuesInReader (IDataReader reader) 
		{
			while (reader.Read ()) 
				Console.WriteLine ("User with name {0} has bank account {1}", reader.GetString (0), reader.GetString (1));
		}
		
		public void UnsafeQuery (string name) 
		{
			IDbConnection dbConnection = new SqlConnection ();
			dbConnection.Open ();
			IDbCommand dbCommand = dbConnection.CreateCommand ();
			dbCommand.CommandText = "SELECT name, bank_account FROM customers WHERE name = '" + name + "'";
			IDataReader dataReader = dbCommand.ExecuteReader ();
			PrintAllValuesInReader (dataReader);
			dataReader.Close ();
			dbCommand.Dispose ();
			dbConnection.Close ();
		}

		public void SafeQuery (string name) 
		{
			IDbConnection dbConnection = new SqlConnection ();
			dbConnection.Open ();
			IDbCommand dbCommand = dbConnection.CreateCommand ();
			dbCommand.CommandText = "SELECT name, bank_account FROM customers WHERE name = @name";
			dbCommand.Parameters.Add (new SqlParameter ("@name", DbType.String).Value = name);
			IDataReader dataReader = dbCommand.ExecuteReader ();
			PrintAllValuesInReader (dataReader);
			dataReader.Close ();
			dbCommand.Dispose ();
			dbConnection.Close ();
		}

		[TestFixtureSetUp]
		public void FixtureSetUp () 
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new AvoidUnsafeSQLQueriesRule ();
			messageCollection = null;
		}

		private MethodDefinition GetMethodForTest (string methodName) 
		{
			TypeDefinition type = assembly.MainModule.Types ["Test.Rules.Security.AvoidUnsafeSQLQueriesTest"];
			foreach (MethodDefinition method in type.Methods) {
				if (method.Name == methodName)
					return method;
			}
			return null;
		}

		[Test]
		public void UnsafeQueryTest () 
		{
			method = GetMethodForTest ("UnsafeQuery");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}

		[Test]
		public void SafeQueryTest () 
		{
			method = GetMethodForTest ("SafeQuery");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
	}
}
