AC_INIT(src/Mono.FastCgi/Server.cs)
AC_CANONICAL_SYSTEM

VERSION="0.0.0.1"

AC_SUBST(VERSION)

AM_INIT_AUTOMAKE("fastcgi-mono-server", $VERSION)
AM_MAINTAINER_MODE

AC_PROG_INSTALL

MONO_REQUIRED=1.1.9

PKG_CHECK_MODULES(MONO, mono >= $MONO_REQUIRED)

dnl Check for Mono
AC_PATH_PROG(MCS, mcs)
if test x$MCS = x; then
	AC_MSG_ERROR(You need mcs)
fi

AC_PATH_PROG(GMCS, gmcs)
if test x$GMCS = x; then
	AC_MSG_ERROR(You need gmcs)
fi

AC_PATH_PROG(MONO, mono)
if test x$MONO = x; then
	AC_MSG_ERROR(You need mono)
fi

dnl GAC
AC_PATH_PROG(GACUTIL, gacutil)
if test x$GACUTIL = x; then
	AC_MSG_ERROR(You need gacutil)
fi

MCS_FLAGS="-codepage:utf8 -debug"
MONO_FLAGS=

if test $USE_MAINTAINER_MODE = yes; then
	MONO_FLAGS="$MONO_FLAGS --debug"
fi

AC_SUBST(MCS_FLAGS)
AC_SUBST(GACUTIL_FLAGS)
AC_SUBST(MONO_FLAGS)

AC_OUTPUT([
Makefile
src/Makefile
src/AssemblyInfo.cs
fastcgi-mono-server
fastcgi-mono-server2
])
