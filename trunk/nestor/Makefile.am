SUBDIRS = . rules
EXTRA_DIST = AssemblyInfo.cs.in MIT.X11 

REWRITE = \
	sed -e "s|\@prefix\@|$(prefix)|g"       \
	    -e "s|\@pkglibdir\@|$(pkglibdir)|g" \
	    -e "s|\@bindir\@|$(bindir)|g"		\
	    -e "s|\@MONO\@|$(MONO)|g" 

CLEANFILES = 

DISTCLEANFILES = \
	AssemblyInfo.cs \
	configure \
	config.guess \
	config.sub \
	Makefile.in \
	install-sh \
	aclocal.m4 \
	INSTALL \
	COPYING \
	missing

test: all
	cd rules;	\
	make test;	\
	cd ..;

run-test: test
	cd rules;	\
	make run-test;	\
	cd ..;

self-test: all
	cd framework;	\
	make self-test;	\
	cd ../console;	\
	make self-test;	\
	cd ../rules;	\
	make self-test;	\
	cd ..

dist:
	make dist-zip
