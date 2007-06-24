
ADDIN_BUILD = $(top_builddir)/build/AddIns
ASSEMBLY = $(ADDIN_BUILD)/CTagsCompletion.dll

REFS = \
	-r:System \
	-r:$(top_builddir)/contrib/Mono.Addins.dll \
	-r:$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Ide.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Projects.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Projects.Gui.dll \
	-r:$(top_builddir)/build/AddIns/MonoDevelop.Components.dll \
	$(GTK_SHARP_LIBS)
	
FILES =  \
	CTagsCompletionDataProvider.cs \
	CTagsProjectServiceExtension.cs \
	CTagsTextEditorExtension.cs \
	ITagsSpecialization.cs \
	Tag.cs

RES =

ADDIN = CTagsCompletion.addin.xml

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/$(ADDIN)

$(ASSEMBLY): $(build_sources) $(build_resources)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(CSC_FLAGS) -debug -out:$@ -target:library $(REFS) \
		$(build_resources:%=/resource:%) $(build_sources)

ctagscompletionlibdir = $(MD_ADDIN_DIR)
ctagscompletionlib_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb

EXTRA_DIST = $(FILES) $(ADDIN) $(RES)

include $(top_srcdir)/Makefile.include