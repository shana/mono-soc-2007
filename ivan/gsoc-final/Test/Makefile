SOURCES=*/*/*.cs
BUILD_DIR=../build
ASSEMBLY=${BUILD_DIR}/Mono.Design.Test.dll
NUNIT_LIBS=`pkg-config --libs mono-nunit`
REFERENCES=System.Design,System.Windows.Forms,System.Drawing,${BUILD_DIR}/Mono.Design.dll  ${NUNIT_LIBS}

all:
	mkdir -p ${BUILD_DIR}
	gmcs -d:NET_2_0 -t:library -r:${REFERENCES} -out:${ASSEMBLY} ${SOURCES}

net-1:
	mkdir -p ${BUILD_DIR}
	mcs -t:library -r:${REFERENCES} -out:${ASSEMBLY} ${SOURCES}

msnet:
	csc -d:MS_NET,NET_2_0 -t:library -r:System.Design.dll,System.Windows.Forms.dll,System.Drawing.dll,"C:\Program Files\NUnit 2.4.1\bin\nunit.core.dll","C:\Program Files\NUnit 2.4.1\bin\nunit.framework.dll","C:\Program Files\NUnit 2.4.1\bin\nunit.core.interfaces.dll"  -out:..\build\Mono.Design.Test.dll *\*\*.cs

run-test:
	nunit-console2 ${ASSEMBLY}

run-test-1:
	nunit-console ${ASSEMBLY}
