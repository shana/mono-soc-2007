monodoc_DATA = monodocer.exe monodocer1.exe monodocs2html.exe monodocs2slashdoc.exe

CLEANFILES = monodocer.exe monodocs2html.exe monodocs2slashdoc.exe 
CSC=gmcs

monodocer_sources = 	\
	monodocer.cs

monodocs2slashdoc_sources = \
	monodocs2slashdoc.cs

monodocs2html_sources = \
	monodocs2html.cs	 overview.xsl stylesheet.xsl defaulttemplate.xsl

EXTRA_DIST = $(monodocer_sources) $(monodocs2html_sources) $(monodocs2slashdoc_sources) \
	bsd-man-to-ecma.pl bsd-man-to-exception.pl

monodocer.exe: $(monodocer_sources)
	$(CSC) -debug $(srcdir)/monodocer.cs -out:$@ -r:Mono.GetOptions

monodocer1.exe: $(monodocer_sources)
	mcs -debug -d:NET_1_0 $(srcdir)/monodocer.cs -out:$@ -r:Mono.GetOptions

monodocs2html.exe: $(monodocs2html_sources)
	$(CSC) -debug $(srcdir)/monodocs2html.cs -out:monodocs2html.exe -r:Mono.GetOptions \
	-resource:$(srcdir)/overview.xsl -resource:$(srcdir)/stylesheet.xsl -resource:$(srcdir)/defaulttemplate.xsl

monodocs2slashdoc.exe: $(monodocs2slashdoc_sources)
	$(CSC) -debug $(srcdir)/monodocs2slashdoc.cs -r:Mono.GetOptions -out:monodocs2slashdoc.exe

xpath.exe: xpath.cs
	$(CSC) -debug xpath.cs

DocTest.dll: 
	$(CSC) $(TEST_CSCFLAGS) -debug -unsafe -target:library -out:$@ DocTest.cs

DocTest.dll-v1: 
	cp DocTest-v1.cs DocTest.cs
	-rm DocTest.dll
	$(MAKE) TEST_CSCFLAGS=$(TEST_CSCFLAGS) DocTest.dll

DocTest.dll-v2: 
	cp DocTest-v1.cs DocTest.cs
	patch -p0 < DocTest-v2.patch
	-rm DocTest.dll
	$(MAKE) TEST_CSCFLAGS=$(TEST_CSCFLAGS) DocTest.dll

check-monodocer-update: monodocer.exe
	find DocTest/en.expected -name \*.xml -exec rm "{}" \;
	$(MAKE) DocTest.dll-v1
	mono --debug monodocer.exe \
		-assembly:DocTest.dll -path:DocTest/en.expected -pretty

check-monodocer: monodocer.exe
	rm -Rf DocTest/en.actual
	$(MAKE) DocTest.dll-v1
	mono --debug monodocer.exe \
		-assembly:DocTest.dll -path:DocTest/en.actual -pretty
	diff --exclude=.svn -rup DocTest/en.expected DocTest/en.actual
	mono --debug monodocer.exe \
		-assembly:DocTest.dll -path:DocTest/en.actual -pretty
	diff --exclude=.svn -rup DocTest/en.expected DocTest/en.actual
	mono --debug monodocer.exe -namespace:Mono.DocTest.Generic -pretty \
		-assembly:DocTest.dll -path:DocTest/en.actual
	diff --exclude=.svn -rup DocTest/en.expected DocTest/en.actual

check-monodocer-since-update: monodocer.exe
	find DocTest/en.expected.since -name \*.xml -exec rm "{}" \;
	$(MAKE) DocTest.dll-v1
	mono --debug monodocer.exe \
		-assembly:DocTest.dll -path:DocTest/en.expected.since -pretty
	$(MAKE) DocTest.dll-v2
	mono --debug monodocer.exe -since:"Version 2.0" \
		-assembly:DocTest.dll -path:DocTest/en.expected.since -pretty

check-monodocer-since: monodocer.exe
	rm -Rf DocTest/en.actual
	$(MAKE) DocTest.dll-v1
	mono --debug monodocer.exe \
		-assembly:DocTest.dll -path:DocTest/en.actual -pretty
	$(MAKE) DocTest.dll-v2
	mono --debug monodocer.exe -since:"Version 2.0" \
		-assembly:DocTest.dll -path:DocTest/en.actual -pretty
	diff --exclude=.svn -rup DocTest/en.expected.since DocTest/en.actual

check-monodocer-importslashdoc-update: monodocer.exe
	find DocTest/en.expected.importslashdoc -name \*.xml -exec rm "{}" \;
	$(MAKE) DocTest.dll-v1 TEST_CSCFLAGS=-doc:DocTest.xml
	mono --debug monodocer.exe -importslashdoc:DocTest.xml \
		-assembly:DocTest.dll -path:DocTest/en.expected.importslashdoc -pretty

check-monodocer-importslashdoc: monodocer.exe
	rm -Rf DocTest/en.actual
	$(MAKE) DocTest.dll-v1 TEST_CSCFLAGS=-doc:DocTest.xml
	mono --debug monodocer.exe -importslashdoc:DocTest.xml \
		-assembly:DocTest.dll -path:DocTest/en.actual -pretty
	diff --exclude=.svn -rup DocTest/en.expected.importslashdoc DocTest/en.actual

check-monodocer-ignore_extra_docs-update:

check-monodocer-ignore_extra_docs: check-monodocer-since
	$(MAKE) DocTest.dll-v1
	mono --debug monodocer.exe -pretty -overrides -ignore_extra_docs \
		-assembly:DocTest.dll -path:en
	diff --exclude=.svn -rup DocTest/en.expected.since DocTest/en.actual

check-monodocs2html-update: monodocs2html.exe
	find DocTest/html.expected -name \*.html -exec rm "{}" \;
	mono --debug monodocs2html.exe \
		-source:DocTest/en.expected.importslashdoc -dest:DocTest/html.expected

check-monodocs2html: check-monodocer monodocs2html.exe
	rm -Rf DocTest/html.actual
	mono --debug monodocs2html.exe \
		-source:DocTest/en.expected.importslashdoc -dest:DocTest/html.actual
	diff --exclude=.svn -rup DocTest/html.expected DocTest/html.actual

check : check-doc-tools

check-update : check-doc-tools-update

check-doc-tools: check-monodocer-since check-monodocer-ignore_extra_docs check-monodocer-importslashdoc check-monodocer check-monodocs2html

check-doc-tools-update: check-monodocer-since-update \
		check-monodocer-ignore_extra_docs-update \
		check-monodocer-importslashdoc-update \
		check-monodocer-update \
		check-monodocs2html-update

clean:
	-rm DocTest.dll DocTest.dll.mdb
