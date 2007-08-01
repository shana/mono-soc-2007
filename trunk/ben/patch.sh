#! /bin/sh

svn cat https://mono-soc-2007.googlecode.com/svn/trunk/ben/configure.patch > temp.patch
patch -p0 -i temp.patch
rm -f temp.patch

cd Extras/

svn cat https://mono-soc-2007.googlecode.com/svn/trunk/ben/makefile.patch > temp.patch
patch -p0 -i temp.patch

svn cat https://mono-soc-2007.googlecode.com/svn/trunk/ben/solution.patch > temp.patch
patch -p0 -i temp.patch
rm -f temp.patch

svn co https://mono-soc-2007.googlecode.com/svn/trunk/ben/MonoDevelop.Database MonoDevelop.Database
cd ..

echo "patched!"
echo "use --enable-database when running configure"
