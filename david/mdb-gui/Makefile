REFS = -r:mdb.exe -r:Mono.Debugger.dll -r:Mono.Debugger.Backend.dll
SRCS = \
	MdbGui.cs \
	Pads/CallstackPad.cs \
	Pads/LocalsPad.cs \
	Pads/ThreadPad.cs \


all: mdb-gui.exe TestApp.exe

clean:
	rm mdb-gui.exe mdb-gui.exe.mdb mdb-gui.pdb

mdb-gui.exe: $(SRCS)
	mcs -debug -pkg:glade-sharp-2.0 $(REFS) -out:$@ $(SRCS)

TestApp.exe: TestApp.cs
	mcs -debug -out:$@ $<
