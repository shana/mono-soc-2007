using NUnit.Framework;
using System.Collections;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ColumnDefinitionCollectionTest {
		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage="'value' already belongs to another 'ColumnDefinitionCollection'.")]
		public void AddTwice() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c.Add(d);
			c.Add(d);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "'ColumnDefinitionCollection' must be type 'ColumnDefinition'.")]
		public void IListAddOtherType() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			((IList)c).Add(new object());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IListAddNull() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			((IList)c).Add(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "'value' already belongs to another 'ColumnDefinitionCollection'.")]
		public void AddToMultipleCollections() {
			ColumnDefinitionCollection c1 = new Grid().ColumnDefinitions;
			ColumnDefinitionCollection c2 = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c1.Add(d);
			c2.Add(d);
		}

		[Test]
		public void AddToMultipleCollectionsOne2() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c.Add(d);
			c.Remove(d);
			c = new Grid().ColumnDefinitions;
			c.Add(d);
		}

		[Test]
		public void RemoveNonexistent() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c.Remove(d);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "'ColumnDefinitionCollection' must be type 'ColumnDefinition'.")]
		public void IListRemoveOtherType() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			((IList)c).Remove(new object());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IListRemoveNull() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			((IList)c).Add(null);
		}

		[Test]
		public void IndexerSet() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			try {
				c[0] = new ColumnDefinition();
				Assert.Fail("1");
			} catch (ArgumentOutOfRangeException ex) {
				Assert.AreEqual(ex.ParamName, "Index is out of collection's boundary.", "2");
			}
		}

		[Test]
		public void IndexerGetOutOfRange() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			try {
				object dummy = c[0];
				Assert.Fail("1");
			} catch (ArgumentOutOfRangeException ex) {
				Assert.AreEqual(ex.ParamName, "Index is out of collection's boundary.", "2");
			}
		}

		[Test]
		public void IndexerSetAtIndexInRange() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c.Add(d);
			c[0] = new ColumnDefinition();
			Assert.AreNotEqual(c[0], d);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "'value' already belongs to another 'ColumnDefinitionCollection'.")]
		public void IndexerSetSame() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c.Add(d);
			c[0] = d;
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "'value' already belongs to another 'ColumnDefinitionCollection'.")]
		public void IndexerSetOutOfRangeFromAnotherCollection() {
			ColumnDefinitionCollection c1 = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c1.Add(d);
			ColumnDefinitionCollection c2 = new Grid().ColumnDefinitions;
			c2[0] = d;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IndexerSetNull() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c.Add(d);
			c[0] = null;
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IndexerSetNullOutOfRange() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			c[0] = null;
		}

		[Test]
		public void ContainsNull() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			Assert.IsFalse(c.Contains(null));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "'value' already belongs to another 'ColumnDefinitionCollection'.")]
		public void InsertTwice() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c.Insert(0, d);
			c.Insert(0, d);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "'ColumnDefinitionCollection' must be type 'ColumnDefinition'.")]
		public void IListInsertOtherType() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			((IList)c).Insert(0, new object());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InsertNull() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			c.Insert(0, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IListInsertNull() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			((IList)c).Insert(0, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "'value' already belongs to another 'ColumnDefinitionCollection'.")]
		public void InsertInMultipleCollections() {
			ColumnDefinitionCollection c1 = new Grid().ColumnDefinitions;
			ColumnDefinitionCollection c2 = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c1.Insert(0, d);
			c2.Insert(0, d);
		}

		[Test]
		public void InsertToMultipleCollectionsOne2() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c.Insert(0, d);
			c.Remove(d);
			c = new Grid().ColumnDefinitions;
			c.Insert(0, d);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void IListIndexerSetNull() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			ColumnDefinition d = new ColumnDefinition();
			c.Add(d);
			((IList)c)[0] = null;
		}

		[Test]
		public void IListCopyTo() {
			ColumnDefinitionCollection c = new Grid().ColumnDefinitions;
			c.Add(new ColumnDefinition());
			((IList)c).CopyTo(new object[1], 0);
		}
	}
}