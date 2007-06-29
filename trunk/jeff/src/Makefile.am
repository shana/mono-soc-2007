virtualpaperdir=$(pkglibdir)
virtualpaper_SCRIPTS = virtualpaper.exe
EXTRA_DIST = $(virtualpaper_sources) $(virtualpaper_sources_in)
CLEANFILES = virtualpaper.exe virtualpaper.exe.mdb

virtualpaper_sources_in = AssemblyInfo.cs.in
virtualpaper_generated_sources = $(virtualpaper_sources_in:.in=)
virtualpaper_sources = Handwriting.cs HigMessageDialog.cs VirtualPaperUI.cs Stroke.cs \
	 CairoStroke.cs Paper.cs NotebookPaper.cs PenStyle.cs ColorToolButton.cs

virtualpaper_build_sources = $(addprefix $(srcdir)/, $(virtualpaper_sources))
virtualpaper_build_sources += $(virtualpaper_generated_sources)
resources = $(srcdir)/../resources/UILayout.xml

embedded= $(foreach res,$(resources), $(addprefix -resource:,$(res)),$(notdir $(res)))

virtualpaper.exe: $(virtualpaper_build_sources) $(resources)
	$(MCS) $(GTKSHARP_LIBS) -r:gnome-sharp -r:Mono.Cairo $(embedded) \
		   -out:$@ $(virtualpaper_build_sources)

