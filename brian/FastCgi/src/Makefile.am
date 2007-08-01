TARGET_DLLS = \
    $(srcdir)/bin/Mono.FastCgi.dll \
    $(srcdir)/bin/Mono.FastCgi2.dll \
    $(srcdir)/bin/Mono.WebServer.dll \
    $(srcdir)/bin/Mono.WebServer2.dll

TARGET_EXES = \
    $(srcdir)/bin/fastcgi-mono-server.exe \
    $(srcdir)/bin/fastcgi-mono-server2.exe

TARGET_EXE_CONFIGS = \
    $(srcdir)/bin/fastcgi-mono-server.exe.config \
    $(srcdir)/bin/fastcgi-mono-server2.exe.config

TARGETS = $(TARGET_DLLS) $(TARGET_EXES)

CSC1 = $(MCS)  $(MCS_FLAGS) $(CSFLAGS)
CSC2 = $(GMCS) $(MCS_FLAGS) $(CSFLAGS)



#
# Mono.FastCgi
#
include $(srcdir)/Mono.FastCgi.sources

$(srcdir)/bin/Mono.FastCgi.dll: $(FCGI_DEPS) $(srcdir)/AssemblyInfo.cs $(top_srcdir)/Mono.FastCgi.snk
	$(CSC1) /target:library $(LIBFLAGS) $(FCGI1_FLAGS) $(FCGI_CSFILES) $(srcdir)/AssemblyInfo.cs /out:$@

$(srcdir)/bin/Mono.FastCgi2.dll: $(FCGI_DEPS) $(srcdir)/AssemblyInfo.cs $(top_srcdir)/Mono.FastCgi.snk
	$(CSC2) /d:NET_2_0 /target:library $(LIBFLAGS) $(FCGI2_FLAGS) $(FCGI_CSFILES) $(srcdir)/AssemblyInfo.cs /out:$@



#
# Mono.WebServer
#
include $(srcdir)/Mono.WebServer.sources

$(srcdir)/bin/Mono.WebServer.dll: $(WEB_DEPS) $(srcdir)/AssemblyInfo.cs $(top_srcdir)/Mono.FastCgi.snk
	$(CSC1) /target:library $(LIBFLAGS) $(WEB1_FLAGS) $(WEB_CSFILES) $(srcdir)/AssemblyInfo.cs /out:$@

$(srcdir)/bin/Mono.WebServer2.dll: $(WEB_DEPS) $(srcdir)/AssemblyInfo.cs $(top_srcdir)/Mono.FastCgi.snk
	$(CSC2) /target:library $(LIBFLAGS) $(WEB2_FLAGS) $(WEB_CSFILES) $(srcdir)/AssemblyInfo.cs /out:$@



#
# Mono.WebServer.FastCgi
#
include $(srcdir)/Mono.WebServer.FastCgi.sources

$(srcdir)/bin/fastcgi-mono-server.exe:  $(WEBFCGI_DEPS) $(srcdir)/AssemblyInfo.cs $(top_srcdir)/Mono.FastCgi.snk
	$(CSC1) /target:exe $(LIBFLAGS) $(WEBFCGI1_FLAGS) $(WEBFCGI_CSFILES) $(srcdir)/AssemblyInfo.cs /out:$@

$(srcdir)/bin/fastcgi-mono-server2.exe: $(WEBFCGI_DEPS) $(srcdir)/AssemblyInfo.cs $(top_srcdir)/Mono.FastCgi.snk
	$(CSC2) /target:exe $(LIBFLAGS) $(WEBFCGI2_FLAGS) $(WEBFCGI_CSFILES) $(srcdir)/AssemblyInfo.cs /out:$@


all: $(TARGETS)

assembliesdir = $(libdir)/$(PACKAGE)
assemblies_DATA = $(TARGETS) $(TARGET_EXE_CONFIGS)

EXTRA_DIST = $(FCGI_DEPS) $(WEB_DEPS) $(WEBFCGI_DEPS) $(srcdir)/AssemblyInfo.cs.in  $(TARGET_EXE_CONFIGS)
DISTCLEANFILES = *.pidb
CLEANFILES = $(TARGETS) $(TARGETS:=.mdb)
MAINTAINERCLEANFILES = Makefile.in
