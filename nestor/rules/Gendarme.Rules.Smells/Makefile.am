SUBDIRS=Test

exceptions_rulesdir=$(pkglibdir)
exceptions_rules_SCRIPTS = Gendarme.Rules.Smells.dll
EXTRA_DIST = $(exceptions_rules_sources) $(exceptions_rules_sources_in) Gendarme.Rules.Smells.xml \
	Gendarme.Rules.Smells.mdp Gendarme.Rules.Smells.csproj
CLEANFILES = Gendarme.Rules.Smells.dll Test.Rules.Smells.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Smells.xml TestResult.xml

exceptions_rules_sources_in = ../AssemblyInfo.cs.in
exceptions_rules_generated_sources = $(exceptions_rules_sources_in:.in=)
exceptions_rules_sources = *.cs

exceptions_rules_build_sources = $(addprefix $(srcdir)/, $(exceptions_rules_sources))
exceptions_rules_build_sources += $(exceptions_rules_generated_sources)

Gendarme.Rules.Smells.dll: $(exceptions_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../lib/Gendarme.Framework.dll -out:$@ $(exceptions_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Smells.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Smells.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

exceptions_test_sources = *.cs
exceptions_test_build_sources = $(addprefix $(srcdir)/Test/, $(exceptions_test_sources))

Test.Rules.Smells.dll: $(exceptions_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -pkg:gtk-sharp -r:../lib/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Smells.dll -r:System.Windows.Forms.dll -out:$@ $(exceptions_test_build_sources)

test: Gendarme.Rules.Smells.dll Test.Rules.Smells.dll

run-test: Gendarme.Rules.Smells.dll Test.Rules.Smells.dll
	MONO_PATH=../lib/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Smells.dll

self-test: test
	gendarme Gendarme.Rules.Smells.dll
