using NUnit.Framework;
#if Implementation
using System;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class DefinitionBaseTest
	{
		[Test]
		public void SharedSizeGroup ()
		{
			ColumnDefinition c = new ColumnDefinition ();
			Assert.IsNull (c.SharedSizeGroup, "1");
			try {
				c.SharedSizeGroup = string.Empty;
				Assert.Fail ("2");
			} catch (ArgumentException) {
			}
			try {
				c.SharedSizeGroup = " ";
				Assert.Fail ("3");
			} catch (ArgumentException) {
			}
			try {
				c.SharedSizeGroup = ".";
				Assert.Fail ("4");
			} catch (ArgumentException) {
			}
			try {
				c.SharedSizeGroup = "1";
				Assert.Fail ("5");
			} catch (ArgumentException) {
			}
			c.SharedSizeGroup = "_";
			c.SharedSizeGroup = "A";
			c.SharedSizeGroup = "A1";
			c.SharedSizeGroup = null;
			Assert.IsNotNull (DefinitionBase.SharedSizeGroupProperty.ValidateValueCallback, "6");
		}
	}
}