SUBDIRS=Test

security_rulesdir=$(pkglibdir)
security_rules_SCRIPTS = Gendarme.Rules.Security.dll
EXTRA_DIST = $(security_rules_sources) $(security_rules_sources_in) Gendarme.Rules.Security.xml \
	Gendarme.Rules.Security.mdp Gendarme.Rules.Security.csproj
CLEANFILES = Gendarme.Rules.Security.dll Test.Rules.Security.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Security.xml TestResult.xml

security_rules_sources_in = ../AssemblyInfo.cs.in
security_rules_generated_sources = $(security_rules_sources_in:.in=)
security_rules_sources = *.cs

security_rules_build_sources = $(addprefix $(srcdir)/, $(security_rules_sources))
security_rules_build_sources += $(security_rules_generated_sources)

Gendarme.Rules.Security.dll: $(security_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../lib/Gendarme.Framework.dll -out:$@ $(security_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Security.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Security.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

security_test_sources = *.cs
security_test_build_sources = $(addprefix $(srcdir)/Test/, $(security_test_sources))

Test.Rules.Security.dll: $(security_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:System.Data.dll -r:../lib/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Security.dll -out:$@ $(security_test_build_sources)

test: Gendarme.Rules.Security.dll Test.Rules.Security.dll

run-test: Gendarme.Rules.Security.dll Test.Rules.Security.dll
	MONO_PATH=../lib/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Security.dll

self-test: test
	gendarme Gendarme.Rules.Security.dll
