FLAGS = -debug -warn:0 -target:library
MONODIR = `echo \`which mono\` | sed 's|bin/mono||'`

REFS = \
	-r:$(MONODIR)/lib/mono/1.0/mdb.exe \
	-r:$(MONODIR)/lib/mono/1.0/System.Runtime.Remoting.dll \
	-r:$(MONODIR)/lib/mono/mono-debugger/Mono.Debugger.dll \
	-r:$(MONODIR)/lib/mono/mono-debugger/Mono.Debugger.Backend.dll \
	-r:$(MONODIR)/lib/monodevelop/bin/MonoDevelop.Core.dll \
	-r:$(MONODIR)/lib/monodevelop/bin/MonoDevelop.Core.Gui.dll \
	-r:$(MONODIR)/lib/monodevelop/bin/MonoDevelop.Ide.dll \
	-r:mdb-gui.exe \

SRCS = \
	DebuggingService.cs \
	DebugCommands_TMP.cs \
	DebugExecutionHandlerFactory.cs \
	ProjectCommands_TMP.cs \
	Gui/Pads/*.cs \

RES = \
#	-resource:MonoDevelop.Debugger.addin.xml

all: MonoDevelop.Debugger.dll

clean:
	rm -f MonoDevelop.Debugger.dll*

MonoDevelop.Debugger.dll: $(SRCS) MonoDevelop.Debugger.addin.xml
	cp /home/david/gsoc/mdb-gui/mdb-gui.exe .
	cp $(MONODIR)/lib/mono/1.0/mdb.exe .
	cp $(MONODIR)/lib/mono/mono-debugger/Mono.Debugger.dll .
	cp $(MONODIR)/lib/mono/mono-debugger/Mono.Debugger.Backend.dll .
	mcs $(FLAGS) -pkg:glade-sharp-2.0 $(REFS) $(RES) -out:$@ $(SRCS)
