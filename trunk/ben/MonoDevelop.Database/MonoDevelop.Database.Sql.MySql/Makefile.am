ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDevelop.Database
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.Database.Sql.MySql.dll
REFS =  \
	$(GTK_SHARP_LIBS) \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Components.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Core.Gui.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Ide.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Projects.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	-r:$(top_builddir)/Extras/MonoDevelop.Database/MonoDevelop.Database.Components/bin/Debug/MonoDevelop.Database.Components.dll \
	-r:$(top_builddir)/Extras/MonoDevelop.Database/MonoDevelop.Database.Designer/bin/Debug/MonoDevelop.Database.Designer.dll \
	-r:$(top_builddir)/Extras/MonoDevelop.Database/MonoDevelop.Database.Sql/bin/Debug/MonoDevelop.Database.Sql.dll \
	-r:$(top_srcdir)/contrib/Mono.Addins.dll \
	-r:System \
	-r:System.Data

FILES =  \
	MySqlConnectionProvider.cs \
	MySqlDbFactory.cs \
	MySqlDialect.cs \
	MySqlGuiProvider.cs \
	MySqlPooledDbConnection.cs \
	MySqlSchemaProvider.cs

ADDIN = MonoDevelop.Database.Sql.MySql.addin.xml

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
EXTRA_DIST = MonoDevelop.Database.Sql.MySql.addin.xml

include $(top_srcdir)/Makefile.include
