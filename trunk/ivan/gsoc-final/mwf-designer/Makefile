SOURCES=src/*.cs
BUILD_DIR=../build
ASSEMBLY=${BUILD_DIR}/mwf-designer.exe
REFERENCES=System.Design,System.Windows.Forms,System.Drawing,System.Data,${BUILD_DIR}/Mono.Design.dll,${BUILD_DIR}/ICSharpCode.NRefactory.dll
DEPS = deps/*
DIST_DIR = ${BUILD_DIR}/mwf-designer

all:
	mkdir -p ${BUILD_DIR}
	cp ${DEPS} ${BUILD_DIR}
	gmcs -debug -r:${REFERENCES} -out:${ASSEMBLY} ${SOURCES}
run:
	cd ${BUILD_DIR};mono --debug mwf-designer.exe

dist:
	mkdir -p ${DIST_DIR}
	cp ${BUILD_DIR}/Mono.Design.* ${DIST_DIR}
	cp ${BUILD_DIR}/ICSharpCode.NRefactory.* ${DIST_DIR}
	cp ${BUILD_DIR}/mwf-designer.* ${DIST_DIR}
	cp ${BUILD_DIR}/Form1* ${DIST_DIR}
	cd ${BUILD_DIR};tar -c mwf-designer | bzip2 -c > mwf-designer.tar.bz2

msnet:
	csc -debug -d:NET_2_0 -t:library -r:System.Design.dll,System.Windows.Forms.dll,System.Drawing.dll,System.Data.dll,..\build\Mono.Design.dll,..\build\ICSharpCode.NRefactory.dll -out:..\build\mwf-designer.exe *.cs



