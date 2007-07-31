ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDevelop.Database
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.Database.Sql.SqlServer.dll
REFS =  \
	-pkg:gtk-sharp-2.0 \
	-pkg:monodevelop \
	-r:../MonoDevelop.Database.Components/bin/Debug/MonoDevelop.Database.Components.dll \
	-r:../MonoDevelop.Database.Designer/bin/Debug/MonoDevelop.Database.Designer.dll \
	-r:../MonoDevelop.Database.Sql/bin/Debug/MonoDevelop.Database.Sql.dll \
	-r:System \
	-r:System.Data

FILES =  \
	MonoDevelop.Database.Sql.SqlServer.addin.xml \
	SqlServerConnectionProvider.cs \
	SqlServerDbFactory.cs \
	SqlServerDialect.cs \
	SqlServerGuiProvider.cs \
	SqlServerPooledDbConnection.cs \
	SqlServerSchemaProvider.cs

ADDIN = MonoDevelop.Database.Sql.SqlServer.addin.xml

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
EXTRA_DIST =

include $(top_srcdir)/Makefile.include
