ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDevelop.Database
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.Database.ConnectionManager.dll
REFS =  \
	$(GLIB_SHARP_LIBS) \
	$(GTK_SHARP_LIBS) \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Components.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Core.Gui.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Ide.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Projects.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	-r:$(top_srcdir)/build/AddIns/MonoDevelop.Database/MonoDevelop.Database.Components.dll \
	-r:$(top_srcdir)/build/AddIns/MonoDevelop.Database/MonoDevelop.Database.Designer.dll \
	-r:$(top_srcdir)/build/AddIns/MonoDevelop.Database/MonoDevelop.Database.Query.dll \
	-r:$(top_srcdir)/build/AddIns/MonoDevelop.Database/MonoDevelop.Database.Sql.dll \
	-r:$(top_srcdir)/contrib/Mono.Addins.dll \
	-r:Mono.Posix \
	-r:System \
	-r:System.Data \
	-r:System.Xml

FILES =  \
	ConnectionManagerCommands.cs \
	ConnectionManagerPad.cs \
	FakeNodes.cs \
	NodeBuilders/AggregatesNodeBuilder.cs \
	NodeBuilders/ColumnNodeBuilder.cs \
	NodeBuilders/ColumnsNodeBuilder.cs \
	NodeBuilders/ConnectionContextCollectionNodeBuilder.cs \
	NodeBuilders/ConnectionContextNodeBuilder.cs \
	NodeBuilders/ConstraintNodeBuilder.cs \
	NodeBuilders/ConstraintsNodeBuilder.cs \
	NodeBuilders/GroupsNodeBuilder.cs \
	NodeBuilders/LanguagesNodeBuilder.cs \
	NodeBuilders/OperatorsNodeBuilder.cs \
	NodeBuilders/ParameterNodeBuilder.cs \
	NodeBuilders/ParametersNodeBuilder.cs \
	NodeBuilders/ProcedureNodeBuilder.cs \
	NodeBuilders/ProceduresNodeBuilder.cs \
	NodeBuilders/RolesNodeBuilder.cs \
	NodeBuilders/RulesNodeBuilder.cs \
	NodeBuilders/SequencesNodeBuilder.cs \
	NodeBuilders/TableNodeBuilder.cs \
	NodeBuilders/TablesNodeBuilder.cs \
	NodeBuilders/TriggersNodeBuilder.cs \
	NodeBuilders/TypesNodeBuilder.cs \
	NodeBuilders/UserNodeBuilder.cs \
	NodeBuilders/UsersNodeBuilder.cs \
	NodeBuilders/ViewNodeBuilder.cs \
	NodeBuilders/ViewsNodeBuilder.cs \
	NodeState.cs

ADDIN = MonoDevelop.Database.ConnectionManager.addin.xml

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/.

$(ADDIN_BUILD)/%: $(srcdir)/lib/%
	mkdir -p $(ADDIN_BUILD)
	cp $< $@

$(ASSEMBLY): $(FILES) $(build_resources)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(CSC_FLAGS) $(REFS) $(build_resources:%=/resource:%) $(build_sources) -out:$@ -target:library


assemblydir = $(MD_ADDIN_DIR)/MonoDevelop.Database
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = MonoDevelop.Database.ConnectionManager.addin.xml

include $(top_srcdir)/Makefile.include
