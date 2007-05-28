monodoc_DATA = assembler.exe normalize.exe monodoc.xml mod.exe validate.exe cs2ecma.exe
noinst_DATA = monodoc.dll monodoc.dll.config
CLEANFILES = monodoc.dll assembler.exe $(monodoc_unix_DATA) normalize.exe mod.exe validate.exe cs2ecma.exe
DISTCLEANFILE = AssemblyInfo.cs

if USE_CYGPATH
GACDIR=`cygpath -w $(prefix)/lib`
GACROOT=`cygpath -w $(DESTDIR)$(prefix)/lib`
else
GACDIR=$(prefix)/lib
GACROOT=$(DESTDIR)$(prefix)/lib
endif

monodoc_sources = \
	$(srcdir)/colorizer.cs		\
	$(srcdir)/man-provider.cs 	\
	$(srcdir)/monohb-provider.cs 	\
	$(srcdir)/xhtml-provider.cs 	\
	$(srcdir)/ecma-provider.cs	\
	$(srcdir)/simple-provider.cs 	\
	$(srcdir)/html-helper.cs	\
	$(srcdir)/provider.cs 		\
	$(srcdir)/index.cs 		\
	$(srcdir)/error-provider.cs 	\
	$(srcdir)/ecmaspec-provider.cs 	\
	$(srcdir)/editing.cs 		\
	$(srcdir)/settings.cs		\
	$(srcdir)/commentservice.cs	\
	$(srcdir)/XmlNodeWriter.cs	\
	$(srcdir)/SearchableIndex.cs	\
	$(srcdir)/SearchableDocument.cs	\
	$(srcdir)/wiki2ecma.cs		\
	$(srcdir)/ecma2wiki.cs		\
	AssemblyInfo.cs	

lucene_sources = \
	$(srcdir)/Lucene.Net.dll.sources 

assembler_sources = \
	$(srcdir)/assembler.cs 

dump_sources      = \
	$(srcdir)/dump.cs 

validate_sources = \
	$(srcdir)/validate.cs

cs2ecma_sources = \
	$(srcdir)/cs2ecma.cs

EXTRA_DIST = \
	$(monodoc_sources) $(assembler_sources) \
	$(dump_sources) 			\
	$(lucene_sources)			\
	$(validate_sources) monodoc-ecma.xsd	\
	monodoc.xml mono-ecma.xsl		\
	normalize.cs monodoc.dll.config.in	\
	ecmaspec-html.xsl mod.cs		\
	AssemblyInfo.cs.in $(cs2ecma_sources) 	\
	ecmaspec-html-css.xsl ecmaspec.css 	\
	base.css mono-ecma-css.xsl 		\
	mono-ecma.css home.html

monodoc_FILES = assembler.exe normalize.exe validate.exe cs2ecma.exe

lucene_warnings = -nowarn:169,164,219,168,162

assembler.exe: $(assembler_sources) monodoc.dll
	$(CSC) -debug /out:assembler.exe $(assembler_sources) -r:ICSharpCode.SharpZipLib.dll -r:./monodoc.dll

dump.exe: $(dump_sources) monodoc.dll
	$(CSC) -debug -out:dump.exe $(dump_sources)  -r:ICSharpCode.SharpZipLib.dll -r:./monodoc.dll

normalize.exe: normalize.cs
	$(CSC) $(srcdir)/normalize.cs -out:normalize.exe

validate.exe: validate.cs $(srcdir)/monodoc-ecma.xsd
	$(CSC) $(srcdir)/validate.cs -out:validate.exe /resource:$(srcdir)/monodoc-ecma.xsd,monodoc-ecma.xsd

cs2ecma.exe: cs2ecma.cs
	$(CSC) $(srcdir)/cs2ecma.cs -out:cs2ecma.exe

mono.pub: $(top_srcdir)/mono.pub
	cp $(top_srcdir)/mono.pub .

monodoc.dll: lucene_sources $(monodoc_sources) mono-ecma.xsl mono.pub ecmaspec-html-css.xsl ecmaspec.css base.css mono-ecma-css.xsl mono-ecma.css home.html
	$(CSC) /debug /nowarn:169,164,162,168,219 -out:monodoc.dll -target:library /resource:$(srcdir)/mono-ecma.xsl,mono-ecma.xsl /resource:$(srcdir)/ecmaspec-html.xsl,ecmaspec-html.xsl /resource:$(srcdir)/ecmaspec-html-css.xsl,ecmaspec-html-css.xsl /resource:$(srcdir)/base.css,base.css /resource:$(srcdir)/ecmaspec.css,ecmaspec.css /resource:$(srcdir)/mono-ecma-css.xsl,mono-ecma-css.xsl /resource:$(srcdir)/mono-ecma.css,mono-ecma.css /resource:$(srcdir)/home.html,home.html $(monodoc_sources)  @lucene_sources -r:ICSharpCode.SharpZipLib.dll -r:System.Web -r:System.Web.Services -r:Commons.Xml.Relaxng /codepage:utf8

lucene_sources: $(srcdir)/Lucene.Net.dll.sources
	sed "s,\@srcdir\@,$(srcdir)," < $(srcdir)/Lucene.Net.dll.sources > lucene_sources

monodoc.dll.config: $(srcdir)/monodoc.dll.config.in Makefile
	if sed 's,@''monodoc_refdir@,$(monodoc_refdir),' $(srcdir)/monodoc.dll.config.in > $@t; then mv $@t $@; else rm -f $@t ; exit 1; fi

mod.exe: mod.cs monodoc.dll
	$(CSC) $(srcdir)/mod.cs -r:./monodoc.dll -out:mod.exe

dist-hook: assembler.exe
	mkdir $(distdir)/web
	mkdir $(distdir)/web/images
	mkdir $(distdir)/web/xtree
	mkdir $(distdir)/web/xtree/images
	mkdir $(distdir)/web/xtree/images/msdn
	mkdir $(distdir)/web/xtree/images/msdn2
	mkdir $(distdir)/web/xtree/images/xp
	mkdir $(distdir)/web/ptree
	cp $(srcdir)/web/index.aspx     $(srcdir)/web/monodoc.ashx $(distdir)/web
	cp $(srcdir)/web/header.aspx    $(distdir)/web
	cp $(srcdir)/web/web.config     $(distdir)/web
	cp $(srcdir)/web/common.css     $(distdir)/web
	cp $(srcdir)/web/sidebar.css    $(distdir)/web
	cp $(srcdir)/web/sidebar.js     $(distdir)/web
	cp $(srcdir)/web/images/*gif  $(distdir)/web/images
	cp $(srcdir)/web/images/*png    $(distdir)/web/images
	cp $(srcdir)/web/xtree/images/msdn/*gif $(distdir)/web/xtree/images/msdn
	cp $(srcdir)/web/xtree/images/msdn2/*gif $(distdir)/web/xtree/images/msdn2
	cp $(srcdir)/web/xtree/images/xp/*png $(distdir)/web/xtree/images/xp
	cp $(srcdir)/web/xtree/images/*png $(distdir)/web/xtree/images
	cp $(srcdir)/web/xtree/*js $(distdir)/web/xtree
	cp $(srcdir)/web/xtree/*css $(distdir)/web/xtree
	cp $(srcdir)/web/ptree/*css $(distdir)/web/ptree
	cp $(srcdir)/web/ptree/*js $(distdir)/web/ptree
	cp -a $(srcdir)/Lucene.Net $(distdir)


bin/Monodoc.Contributions.dll: server.cs 
	-mkdir bin
	mcs -g server.cs -target:library -out:bin/Monodoc.Contributions.dll -r:System.Web -r:System.Web.Services -r:./monodoc.dll -r:System.Data -r:ByteFX.Data

web: bin/Monodoc.Contributions.dll
	xsp

db:
	mysql -u admin -p 

cleandb:
	mysql -u admin -p < tables.sql

up:
	scp tables.sql server.cs server.asmx monodoc.dll root@www.go-mono.com:

install-data-local:
	$(GACUTIL) /i monodoc.dll /f /package monodoc /gacdir $(GACDIR) /root $(GACROOT)

uninstall-local:
	-$(GACUTIL) /u monodoc /package monodoc /gacdir $(GACDIR) /root $(GACROOT)

check-validate-update: validate.exe
	mono validate.exe ecma ../tools/DocTest/en.expected > validate.check.monodocer
	mono validate.exe ecma ../tools/DocTest/en.expected.importslashdoc > \
		validate.check.monodocer.importslashdoc
	mono validate.exe ecma ../tools/DocTest/en.expected.since > \
		validate.check.monodocer.since

check-validate: validate.exe
	mono validate.exe ecma ../tools/DocTest/en.expected | diff - validate.check.monodocer
	mono validate.exe ecma ../tools/DocTest/en.expected.importslashdoc | \
		diff --brief - validate.check.monodocer.importslashdoc
	mono validate.exe ecma ../tools/DocTest/en.expected.since | \
		diff --brief - validate.check.monodocer.since

check: check-validate

