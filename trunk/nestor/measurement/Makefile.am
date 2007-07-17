SUBDIRS = src
bin_SCRIPTS = measurement
EXTRA_DIST = measurement.in
DISTCLEANFILES = Makefile.in

REWRITE = \
        sed -e "s|\@prefix\@|$(prefix)|g"               \
            -e "s|\@pkglibdir\@|$(pkglibdir)|g"         \
            -e "s|\@bindir\@|$(bindir)|g"		\
	    -e "s|\@MONO\@|$(MONO)|g" 

$(bin_SCRIPTS): $(srcdir)/script.in Makefile
	$(REWRITE) $(srcdir)/script.in > $@.tmp
	mv $@.tmp $@


