SUBDIRS=Test

dodgy_rulesdir=$(pkglibdir)
dodgy_rules_SCRIPTS = Gendarme.Rules.Dodgy.dll
EXTRA_DIST = $(dodgy_rules_sources) $(dodgy_rules_sources_in) Gendarme.Rules.Dodgy.xml \
	Gendarme.Rules.Dodgy.mdp Gendarme.Rules.Dodgy.csproj
CLEANFILES = Gendarme.Rules.Dodgy.dll Test.Rules.Dodgy.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Dodgy.xml TestResult.xml

dodgy_rules_sources_in = ../../AssemblyInfo.cs.in
dodgy_rules_generated_sources = $(dodgy_rules_sources_in:.in=)
dodgy_rules_sources = DontDeclareProtectedFieldsInSealedClassRule.cs

dodgy_rules_build_sources = $(addprefix $(srcdir)/, $(dodgy_rules_sources))
dodgy_rules_build_sources += $(dodgy_rules_generated_sources)

Gendarme.Rules.Dodgy.dll: $(dodgy_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(dodgy_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Dodgy.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	rm -f $(DESTDIR)$(pkglibdir)/`basename Gendarme.Rules.Dodgy.xml`;
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

dodgy_test_sources = DontDeclareProtectedFieldsInSealedClassTest.cs
dodgy_test_build_sources = $(addprefix $(srcdir)/Test/, $(dodgy_test_sources))

Test.Rules.Dodgy.dll: $(dodgy_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Dodgy.dll -out:$@ $(dodgy_test_build_sources)

test: Gendarme.Rules.Dodgy.dll Test.Rules.Dodgy.dll

run-test: Gendarme.Rules.Dodgy.dll Test.Rules.Dodgy.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Dodgy.dll

self-test: test
	gendarme Gendarme.Rules.Dodgy.dll
