measurementdir=$(pkglibdir)
measurement_SCRIPTS = measurement.exe
EXTRA_DIST = $(measurement_sources) $(measurement_sources_in)
CLEANFILES = measurement.exe
DISTCLEANFILES = Makefile.in

measurement_sources_in = AssemblyInfo.cs.in
measurement_generated_sources = $(measurement_sources_in:.in=)
measurement_sources = *.cs gtk-gui/*.cs 


measurement_build_sources = $(addprefix $(srcdir)/, $(measurement_sources))
#measurement_build_sources += $(measurement_generated_sources)


measurement.exe: $(measurement_build_sources)
	$(MCS) -pkg:mono-cecil -pkg:gtk-sharp-2.0 -pkg:glade-sharp-2.0 -resource:gtk-gui/measures.glade,measures.glade -out:$@ $(measurement_build_sources) 

