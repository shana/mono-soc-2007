MCS=gmcs
SOURCES=*.cs
BUILD_DIR=../build
ASSEMBLY=${BUILD_DIR}/mwf-designer.exe
REFERENCES=System.Design,System.Windows.Forms,System.Drawing,${BUILD_DIR}/Mono.Design.dll

all:
	mkdir -p ${BUILD_DIR}
	${MCS} -r:${REFERENCES} ${SOURCES} -out:${ASSEMBLY}
run:
	mono ${ASSEMBLY}

