using NUnit.Framework;
using System.Windows.Markup;
#if Implementation
using System;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[TestFixture]
	public class ImageTest {
		#region BaseUri
		[Test]
		public void BaseUri() {
			new BaseUriImage();
		}

		class BaseUriImage : Image {
			int get_calls;
			int set_calls;

			public BaseUriImage() {
				((IUriContext)this).BaseUri = ((IUriContext)this).BaseUri;
				Assert.AreEqual(get_calls, 1, "1");
				Assert.AreEqual(set_calls, 1, "2");
				object dummy = ((IUriContext)this).BaseUri;
				Assert.AreEqual(get_calls, 2, "3");
				Assert.AreEqual(set_calls, 1, "4");
			}

			protected override Uri BaseUri {
				get {
					get_calls++;
					return base.BaseUri;
				}
				set {
					set_calls++;
					base.BaseUri = value;
				}
			}
		}
		#endregion

		#region BaseUriDefaultValue
		[Test]
		public void BaseUriDefaultValue() {
			new BaseUriDefaultValueImage();
		}

		class BaseUriDefaultValueImage : Image {
			public BaseUriDefaultValueImage() {
				Assert.IsNull(BaseUri);
			}
		}
		#endregion
	}
}