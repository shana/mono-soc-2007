SUBDIRS=Test

badPractice_rulesdir=$(pkglibdir)
badPractice_rules_SCRIPTS = Gendarme.Rules.BadPractice.dll
EXTRA_DIST = $(badPractice_rules_sources) $(badPractice_rules_sources_in) Gendarme.Rules.BadPractice.xml \
	Gendarme.Rules.BadPractice.mdp Gendarme.Rules.BadPractice.csproj
CLEANFILES = Gendarme.Rules.BadPractice.dll Test.Rules.BadPractice.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.BadPractice.xml TestResult.xml

badPractice_rules_sources_in = ../../AssemblyInfo.cs.in
badPractice_rules_generated_sources = $(badPractice_rules_sources_in:.in=)
badPractice_rules_sources = ImplementingEqualsButNotGetHashCodeAndViceVersaRule.cs CloneMethodShouldNotReturnNullRule.cs

badPractice_rules_build_sources = $(addprefix $(srcdir)/, $(badPractice_rules_sources))
badPractice_rules_build_sources += $(badPractice_rules_generated_sources)

Gendarme.Rules.BadPractice.dll: $(badPractice_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(badPractice_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.BadPractice.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	rm -f $(DESTDIR)$(pkglibdir)/`basename Gendarme.Rules.BadPractice.xml`;
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

badPractice_test_sources = ImplementingEqualsButNotGetHashCodeAndViceVersaTest.cs CloneMethodShouldNotReturnNullTest.cs
badPractice_test_build_sources = $(addprefix $(srcdir)/Test/, $(badPractice_test_sources))

Test.Rules.BadPractice.dll: $(badPractice_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.BadPractice.dll -out:$@ $(badPractice_test_build_sources)

test: Gendarme.Rules.BadPractice.dll Test.Rules.BadPractice.dll

run-test: Gendarme.Rules.BadPractice.dll Test.Rules.BadPractice.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.BadPractice.dll

self-test: test
	gendarme Gendarme.Rules.BadPractice.dll
