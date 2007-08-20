SOURCES=src/*.cs
BUILD_DIR=../build
MD_DIR = `pkg-config --variable libdir monodevelop`/monodevelop
INSTALL_DIR = ${MD_DIR}/AddIns
ASSEMBLY=${BUILD_DIR}/WinFormsAddin.dll
REFERENCES=System.Web,System.Design,System.Windows.Forms,System.Drawing,${BUILD_DIR}/ICSharpCode.NRefactory.dll,${BUILD_DIR}/Mono.Design.dll,${INSTALL_DIR}/MonoDevelop.DesignerSupport/MonoDevelop.DesignerSupport.dll\
	`pkg-config --libs monodevelop` `pkg-config --libs gtk-sharp-2.0` -r:${MD_DIR}/bin/Mono.Addins.dll
RESOURCES=templates/WinForms2Project.xpt.xml
ADDIN_FILE = WinForms.addin.xml
DEPS = deps/*

all:
	mkdir -p ${BUILD_DIR}
	cp ${ADDIN_FILE} ${BUILD_DIR}
	cp ${DEPS} ${BUILD_DIR}
	gmcs -d:NET_2_0 -target:library -debug -resource:${RESOURCES} -r:${REFERENCES} -out:${ASSEMBLY} ${SOURCES}

install:
	cp ${BUILD_DIR}/${ADDIN_FILE} ${INSTALL_DIR}
	cp ${BUILD_DIR}/*.dll ${INSTALL_DIR}
	cp ${BUILD_DIR}/*.mdb ${INSTALL_DIR}
run:
	make install
	monodevelop&
