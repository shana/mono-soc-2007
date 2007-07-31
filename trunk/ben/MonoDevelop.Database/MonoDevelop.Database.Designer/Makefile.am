ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDevelop.Database
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.Database.Designer.dll
REFS =  \
	-pkg:gtk-sharp-2.0 \
	-pkg:gtksourceview-sharp-2.0 \
	-pkg:monodevelop \
	-r:../../contrib/Mono.Addins.dll \
	-r:../MonoDevelop.Database.Components/bin/Debug/MonoDevelop.Database.Components.dll \
	-r:../MonoDevelop.Database.Sql/bin/Debug/MonoDevelop.Database.Sql.dll \
	-r:Mono.Posix \
	-r:System

FILES =  \
	Commands/temp.cs \
	Dialogs/CreateDatabaseDialog.cs \
	Dialogs/PreviewDialog.cs \
	Dialogs/ProcedureEditorDialog.cs \
	Dialogs/TableEditorDialog.cs \
	Dialogs/UserEditorDialog.cs \
	Dialogs/ViewEditorDialog.cs \
	gtk-gui/generated.cs \
	gtk-gui/MonoDevelop.Database.Designer.CheckConstraintEditorWidget.cs \
	gtk-gui/MonoDevelop.Database.Designer.ColumnsEditorWidget.cs \
	gtk-gui/MonoDevelop.Database.Designer.CommentEditorWidget.cs \
	gtk-gui/MonoDevelop.Database.Designer.ConstraintsEditorWidget.cs \
	gtk-gui/MonoDevelop.Database.Designer.CreateDatabaseDialog.cs \
	gtk-gui/MonoDevelop.Database.Designer.ForeignKeyConstraintEditorWidget.cs \
	gtk-gui/MonoDevelop.Database.Designer.IndicesEditorWidget.cs \
	gtk-gui/MonoDevelop.Database.Designer.PreviewDialog.cs \
	gtk-gui/MonoDevelop.Database.Designer.PrimaryKeyConstraintEditorWidget.cs \
	gtk-gui/MonoDevelop.Database.Designer.ProcedureEditorDialog.cs \
	gtk-gui/MonoDevelop.Database.Designer.TableEditorDialog.cs \
	gtk-gui/MonoDevelop.Database.Designer.TriggersEditorWidget.cs \
	gtk-gui/MonoDevelop.Database.Designer.UniqueConstraintEditorWidget.cs \
	gtk-gui/MonoDevelop.Database.Designer.UserEditorDialog.cs \
	gtk-gui/MonoDevelop.Database.Designer.ViewEditorDialog.cs \
	MonoDevelop.Database.Designer.addin.xml \
	Widgets/CheckConstraintEditorWidget.cs \
	Widgets/ColumnsEditorWidget.cs \
	Widgets/CommentEditorWidget.cs \
	Widgets/ConstraintsEditorWidget.cs \
	Widgets/ForeignKeyConstraintEditorWidget.cs \
	Widgets/IndicesEditorWidget.cs \
	Widgets/PrimaryKeyConstraintEditorWidget.cs \
	Widgets/SelectColumnConstraintWidget.cs \
	Widgets/TriggersEditorWidget.cs \
	Widgets/UniqueConstraintEditorWidget.cs

ADDIN = MonoDevelop.Database.Designer.addin.xml

all: $(ASSEMBLY) $(ADDIN_BUILD)/$(ADDIN)

$(ADDIN_BUILD)/$(ADDIN): $(srcdir)/$(ADDIN)
	mkdir -p $(ADDIN_BUILD)
	cp $(srcdir)/$(ADDIN) $(ADDIN_BUILD)/.

$(ADDIN_BUILD)/%: $(srcdir)/lib/%
	mkdir -p $(ADDIN_BUILD)
	cp $< $@

$(ASSEMBLY): $(FILES) $(build_resources)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(CSC_FLAGS) $(REFS) $(build_resources:%=/resource:%) $(build_sources) -out:$@ -target:library


assemblydir = $(MD_ADDIN_DIR)/MonoDevelop.Database
assembly_DATA = $(ASSEMBLY) $(ADDIN)

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST =

include $(top_srcdir)/Makefile.include
