SOURCES=*/*/*.cs
BUILD_DIR=../build
ASSEMBLY=${BUILD_DIR}/Mono.Design.dll
REFERENCES=System.Design,System.Windows.Forms,System.Drawing

all:
	mkdir -p ${BUILD_DIR}
	gmcs -d:NET_2_0 -target:library -debug -r:${REFERENCES} -out:${ASSEMBLY} ${SOURCES}

net-1:
	mkdir -p ${BUILD_DIR}
	mcs -t:library -debug -r:${REFERENCES} -out:${ASSEMBLY} ${SOURCES}

msnet:
	csc -debug -d:NET_2_0 -t:library -r:System.Design.dll,System.Windows.Forms.dll,System.Drawing.dll,System.Data.dll, -out:..\build\Mono.Design.dll *\*\*.cs
