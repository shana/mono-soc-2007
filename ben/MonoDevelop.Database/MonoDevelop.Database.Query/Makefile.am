ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDevelop.Database
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.Database.Query.dll
REFS =  \
	-pkg:glade-sharp-2.0 \
	-pkg:glib-sharp-2.0 \
	-pkg:gtk-sharp-2.0 \
	-pkg:gtksourceview-sharp-2.0 \
	-pkg:monodevelop \
	-r:../../contrib/Mono.Addins.dll \
	-r:../MonoDevelop.Database.Components/bin/Debug/MonoDevelop.Database.Components.dll \
	-r:../MonoDevelop.Database.Sql/bin/Debug/MonoDevelop.Database.Sql.dll \
	-r:Mono.Posix \
	-r:System \
	-r:System.Data

FILES =  \
	Commands/ToolCommands.cs \
	QueryResultView.cs \
	SqlQueryView.cs

ADDIN = MonoDevelop.Database.Query.addin.xml

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
EXTRA_DIST = MonoDevelop.Database.Query.addin.xml

include $(top_srcdir)/Makefile.include
