ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDevelop.Database
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.Database.ConnectionManager.dll
REFS =  \
	-pkg:glib-sharp-2.0 \
	-pkg:gtk-sharp-2.0 \
	-pkg:monodevelop \
	-r:../../contrib/Mono.Addins.dll \
	-r:../MonoDevelop.Database.Components/bin/Debug/MonoDevelop.Database.Components.dll \
	-r:../MonoDevelop.Database.Query/bin/Debug/MonoDevelop.Database.Query.dll \
	-r:../MonoDevelop.Database.Sql/bin/Debug/MonoDevelop.Database.Sql.dll \
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
