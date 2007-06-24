REFS = -r:mdb.exe -r:Mono.Debugger.dll -r:Mono.Debugger.Backend.dll

mdb-gui.exe: MdbGui.cs
	mcs -debug -pkg:glade-sharp $(REFS) -out:$@ $<
