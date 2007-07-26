measurementdir=$(pkglibdir)
measurement_SCRIPTS = measurement.exe
EXTRA_DIST = $(measurement_sources) $(measurement_sources_in)
CLEANFILES = measurement.exe Test.Measures.dll Test.Assembly.dll TestResult.xml
DISTCLEANFILES = Makefile.in

measurement_sources_in = AssemblyInfo.cs.in
measurement_generated_sources = $(measurement_sources_in:.in=)
measurement_sources = *.cs gtk-gui/*.cs 


measurement_build_sources = $(addprefix $(srcdir)/, $(measurement_sources))
#measurement_build_sources += $(measurement_generated_sources)


measurement.exe: $(measurement_build_sources)
	$(MCS) -pkg:mono-cecil -pkg:gtk-sharp-2.0 -pkg:glade-sharp-2.0 -resource:gtk-gui/measures.glade,measures.glade -out:$@ $(measurement_build_sources) 


measures_test_sources = *.cs
measures_test_build_sources = $(addprefix $(srcdir)/Test/, $(measures_test_sources))

Test.Measures.dll: measurement.exe $(measures_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit \
		-r:measurement.exe -out:$@ $(measures_test_build_sources)

assembly_test_sources = *.cs
assembly_test_build_sources = $(addprefix $(srcdir)/Test/Assembly/, $(assembly_test_sources))

Test.Assembly.dll: $(assembly_test_build_sources)
	$(MCS) -target:library -out:$@ $(assembly_test_build_sources)

Test.Assembly.dll:

test: measurement.exe Test.Measures.dll Test.Assembly.dll

run-test: measurement.exe Test.Measures.dll Test.Assembly.dll
	MONO_PATH=$(MONO_PATH) nunit-console Test.Measures.dll
