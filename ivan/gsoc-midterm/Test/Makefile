SOURCES=*/*/*.cs
BUILD_DIR=../build
ASSEMBLY=${BUILD_DIR}/Mono.Design.Test.dll
NUNIT_LIBS=/usr/lib/mono/gac/nunit.core/2.2.0.0__96d09a1eb7f44a77/nunit.core.dll,/usr/lib/mono/gac/nunit.framework/2.2.0.0__96d09a1eb7f44a77/nunit.framework.dll,/usr/lib/mono/gac/nunit.util/2.2.0.0__96d09a1eb7f44a77/nunit.util.dll
REFERENCES=System.Design,System.Windows.Forms,System.Drawing,${BUILD_DIR}/Mono.Design.dll,${NUNIT_LIBS}

all:
	mkdir -p ${BUILD_DIR}
	gmcs -d:NET_2_0 -t:library -r:${REFERENCES} -out:${ASSEMBLY} ${SOURCES}

net-1:
	mkdir -p ${BUILD_DIR}
	mcs -t:library -r:${REFERENCES} -out:${ASSEMBLY} ${SOURCES}

msnet:
	#slickedit built-in

run-test:
	nunit-console2 ${ASSEMBLY}

run-test-1:
	nunit-console ${ASSEMBLY}
