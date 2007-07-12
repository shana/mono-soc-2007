FLAGS = -debug -warn:0
MONODIR = `echo \`which mono\` | sed 's|bin/mono||'`

PIXMAPS = `echo pixmaps/* | sed -r 's|pixmaps/([^ ]*)|-resource:pixmaps/\1,Mono.Debugger.Frontend.pixmaps.\1|g'`

REFS = \
	-r:$(MONODIR)/lib/mono/1.0/mdb.exe \
	-r:$(MONODIR)/lib/mono/mono-debugger/Mono.Debugger.dll \
	-r:$(MONODIR)/lib/mono/mono-debugger/Mono.Debugger.Backend.dll \

SRCS = \
	ExceptionWindow.cs \
	MdbGui.cs \
	Pixmaps.cs \
	Pads/*.cs \
	TreeModel/*.cs \


all: mdb-gui.exe TestApp.exe

clean:
	rm -f mdb-gui.exe mdb-gui.exe.mdb mdb-gui.pdb TestApp.exe TestApp.exe.mdb

mdb-gui.exe: $(SRCS) pixmaps/*
	mcs $(FLAGS) -pkg:glade-sharp-2.0 $(REFS) $(PIXMAPS) -out:$@ $(SRCS)

TestApp.exe: TestApp.cs
	mcs $(FLAGS) -out:$@ $<
