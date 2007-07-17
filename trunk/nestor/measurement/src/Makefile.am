measurementdir=$(pkglibdir)
measurement_SCRIPTS = measurement.exe
EXTRA_DIST = $(measurement_sources) $(measurement_sources_in)
CLEANFILES = measurement.exe
DISTCLEANFILES = Makefile.in

measurement_sources_in = AssemblyInfo.cs.in
measurement_generated_sources = $(measurement_sources_in:.in=)
measurement_sources = Launcher.cs

measurement_build_sources = $(addprefix $(srcdir)/, $(measurement_sources))
measurement_build_sources += $(measurement_generated_sources)

measurement.exe: $(measurement_build_sources)
	$(MCS) -out:$@ $(measurement_build_sources) 

