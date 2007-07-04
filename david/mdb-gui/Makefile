REFS = -r:mdb.exe -r:Mono.Debugger.dll -r:Mono.Debugger.Backend.dll

# Build by SharpDevelop
#mdb-gui.exe: MdbGui.cs
#	mcs -debug -pkg:glade-sharp $(REFS) -out:$@ $<

TestApp.exe: TestApp.cs
	mcs -debug -out:$@ $<
