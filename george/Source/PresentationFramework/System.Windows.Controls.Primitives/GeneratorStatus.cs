#if Implementation
namespace Mono.System.Windows.Controls.Primitives {
#else
namespace System.Windows.Controls.Primitives {
#endif
	public enum GeneratorStatus {
		NotStarted,
		GeneratingContainers,
		ContainersGenerated,
		Error
	}
}
