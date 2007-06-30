
ADDIN_BUILD = $(top_builddir)/build/AddIns/BackendBindings
ASSEMBLY = $(ADDIN_BUILD)/CBinding.dll

REFS = \
	-r:System \
	-r:System.Xml \
	-r:Mono.Posix \
	-r:$(top_builddir)/contrib/Mono.Addins.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Core.Gui.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Projects.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Projects.Gui.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Ide.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Components.dll \
	$(GTK_SHARP_LIBS)
	

FILES =  \
	CLanguageBinding.cs \
	Compiler/CCompiler.cs \
	Compiler/GccCompiler.cs \
	Compiler/GNUCompiler.cs \
	Compiler/GppCompiler.cs \
	Compiler/ICompiler.cs \
	gtk-gui/CBinding.AddLibraryDialog.cs \
	gtk-gui/CBinding.AddPathDialog.cs \
	gtk-gui/CBinding.CodeGenerationPanel.cs \
	gtk-gui/CBinding.CompilerPanel.cs \
	gtk-gui/CBinding.EditPackagesDialog.cs \
	gtk-gui/CBinding.OutputOptionsPanel.cs \
	gtk-gui/generated.cs \
	Gui/AddLibraryDialog.cs \
	Gui/AddPathDialog.cs \
	Gui/CodeGenerationPanel.cs \
	Gui/CompilerPanel.cs \
	Gui/EditPackagesDialog.cs \
	Gui/OutputOptionsPanel.cs \
	Navigation/Class.cs \
	Navigation/ClassNodeBuilder.cs \
	Navigation/Function.cs \
	Navigation/FunctionNodeBuilder.cs \
	Navigation/GlobalsNodeBuilder.cs \
	Navigation/LanguageItem.cs \
	Navigation/LanguageItemCommandHandler.cs \
	Navigation/Namespace.cs \
	Navigation/NamespaceNodeBuilder.cs \
	Navigation/ProjectNavigationInformation.cs \
	Navigation/ProjectNavigationInformationManager.cs \
	Navigation/ProjectNodeBuilderExtension.cs \
	Navigation/Structure.cs \
	Navigation/StructureNodeBuilder.cs \
	Navigation/Tag.cs \
	Navigation/TagDatabaseManager.cs \
	Project/CCompilationParameters.cs \
	Project/CProject.cs \
	Project/CProjectBinding.cs \
	Project/CProjectConfiguration.cs \
	Project/CProjectServiceExtension.cs \
	Project/ProjectPackage.cs \
	Project/ProjectPackageCollection.cs \
	Project/ProjectPackageEventArgs.cs \
	ProjectPad/ProjectNodeExtension.cs \
	ProjectPad/ProjectPackageNodeBuilder.cs \
	ProjectPad/ProjectPackagesFolderNodeBuilder.cs \
	ProjectPad/ProjectReferencesExtension.cs

RES =  \
	icons/C.File.Empty \
	icons/C.Project.Empty \
	icons/C.Source.File.Empty \
	icons/Cpp.Project.Empty \
	templates/EmptyCFile.xft.xml \
	templates/EmptyCppProject.xpt.xml \
	templates/EmptyCProject.xpt.xml

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