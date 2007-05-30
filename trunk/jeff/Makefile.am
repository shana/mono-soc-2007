SUBDIRS = resources src	man
bin_SCRIPTS = virtualpaper
EXTRA_DIST = script.in
CLEANFILES = virtualpaper

REWRITE = \
        sed -e "s|\@prefix\@|$(prefix)|g"               \
            -e "s|\@pkglibdir\@|$(pkglibdir)|g"         \
            -e "s|\@bindir\@|$(bindir)|g"		\
	    -e "s|\@MONO\@|$(MONO)|g" 

$(bin_SCRIPTS): $(srcdir)/script.in Makefile
	$(REWRITE) $(srcdir)/script.in > $@.tmp
	mv $@.tmp $@


