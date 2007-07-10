REFS = -r:mdb.exe -r:Mono.Debugger.dll -r:Mono.Debugger.Backend.dll
SRCS = \
	MdbGui.cs \
	Pixmaps.cs \
	Pads/*.cs \
	TreeModel/*.cs \


all: mdb-gui.exe TestApp.exe

clean:
	rm -f mdb-gui.exe mdb-gui.exe.mdb mdb-gui.pdb TestApp.exe TestApp.exe.mdb

mdb-gui.exe: $(SRCS) pixmaps/*
	mcs -debug -pkg:glade-sharp-2.0 $(REFS) `echo pixmaps/* | sed -r 's|pixmaps/([^ ]*)|-resource:pixmaps/\1,Mono.Debugger.Frontend.pixmaps.\1|g'` -out:$@ $(SRCS)

TestApp.exe: TestApp.cs
	mcs -debug -out:$@ $<
