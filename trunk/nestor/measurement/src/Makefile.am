measurementdir=$(pkglibdir)
measurement_SCRIPTS = measurement.exe
EXTRA_DIST = $(measurement_sources) $(measurement_sources_in)
CLEANFILES = measurement-tool.exe

measurement_sources_in = AssemblyInfo.cs.in
measurement_generated_sources = $(measurement_sources_in:.in=)
measurement_sources = *.cs

measurement_build_sources = $(addprefix $(srcdir)/, $(measurement_sources))
measurement_build_sources += $(measurement_generated_sources)

embedded= $(foreach res,$(resources), $(addprefix -resource:,$(res)),$(notdir $(res)))

measurement.exe: $(monoskel_build_sources) $(resources)
	$(MCS) $(GTKSHARP_LIBS) $(embedded) -out:$@ $< 

