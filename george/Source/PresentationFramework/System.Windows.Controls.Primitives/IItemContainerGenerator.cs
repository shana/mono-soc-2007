#if Implementation
using System;
using System.Windows;
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public interface IItemContainerGenerator {
		DependencyObject GenerateNext();
		DependencyObject GenerateNext(out bool isNewlyRealized);
		GeneratorPosition GeneratorPositionFromIndex(int itemIndex);
		ItemContainerGenerator GetItemContainerGeneratorForPanel(Panel panel);
		int IndexFromGeneratorPosition(GeneratorPosition position);
		void PrepareItemContainer(DependencyObject container);
		void Remove(GeneratorPosition position, int count);
		void RemoveAll();
		IDisposable StartAt(GeneratorPosition position, GeneratorDirection direction);
		IDisposable StartAt(GeneratorPosition position, GeneratorDirection direction, bool allowStartAtRealizedItem);
	}
}