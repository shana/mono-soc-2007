
ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings
ASSEMBLY = $(ADDIN_BUILD)/CBinding.dll

REFS = \
	-r:System \
	-r:System.Xml \
	-r:mscorlib \
	-r:Mono.Posix \
	-r:$(top_builddir)/contrib/Mono.Addins.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Core.Gui.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Projects.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Projects.Gui.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Ide \
	$(GTK_SHARP_LIBS)
	

FILES = \
	Project/CCompilationParameters.cs \
	Project/CProject.cs \
	Project/CProjectBinding.cs \
	Project/CProjectConfiguration.cs \
	Gui/CodeGenerationPanel.cs \
	Gui/CompilerPanel.cs \
	Gui/AddLibraryDialog.cs \
	Gui/AddPathDialog.cs \
	gtk-gui/generated.cs \
	gtk-gui/CBinding.CodeGenerationPanel.cs \
	gtk-gui/CBinding.CompilerPanel.cs \
	gtk-gui/CBinding.AddLibraryDialog.cs \
	gtk-gui/CBinding.AddPathDialog.cs \
	Compiler/ICompiler.cs \
	Compiler/CCompiler.cs \
	Compiler/GNUCompiler.cs \
	Compiler/GccCompiler.cs \
	Compiler/GppCompiler.cs

RES = \
	icons/C.File.Empty \
	icons/C.Project.Empty \
	icons/Cpp.Project.Empty \
	templates/EmptyCProject.xpt.xml \
	templates/EmptyCFile.xft.xml \
	templates/EmptyCppProject.xpt.xml

ADDIN = CBinding.addin.xml

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/$(ADDIN)

$(ASSEMBLY): $(build_sources) $(build_resources)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(CSC_FLAGS) -debug -out:$@ -target:library $(REFS) \
		$(build_resources:%=/resource:%) $(build_sources)


cbindinglibdir = $(MD_ADDIN_DIR)/BackendBindings
cbindinglib_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

EXTRA_DIST = $(FILES) $(ADDIN) $(RES)

include $(top_srcdir)/Makefile.include