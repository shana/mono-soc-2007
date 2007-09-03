using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

namespace FieldStat.DataCollection
{
    public static class Plugins
    {
        public static string PluginDirectory = "Plugins";
        private static ArrayList plugins;
        public static void InitPlugins()
        {
            plugins = LoadPlugins();
        }
        public static void ComputeResults(Results results, ICollection files, Hashtable htBin )
        {
            
            ArrayList customVisit = new ArrayList();
            ArrayList communityVisit = new ArrayList();

            foreach (AbstractPlugin plugin in plugins)
            {
                if (plugin.UsesCommunityVisitor)
                {
                    communityVisit.Add(plugin);
                }
                else
                {
                    customVisit.Add(plugin);
                }
            }

            foreach (AbstractPlugin plugin in customVisit)
            {
                plugin.ComputeResults(results, files, htBin, results.Filters);
            }
            if (communityVisit.Count > 0)
            {
                Visit visit = new Visit();

                // Gather collectors from plugins.
                foreach (AbstractPlugin plugin in communityVisit)
                {
                    visit.Collectors.Register(plugin.Name, plugin.CreateCollector());
                }

                // Visit Assemblies.
                visit.DoScan(files, htBin, results.Filters);

                // Give Plugin results of scanning
                foreach (AbstractPlugin plugin in communityVisit)
                {
                    plugin.SetOutputTable(visit.Collectors[plugin.Name]);
                }
            }
        }

        private static ArrayList LoadPlugins()
        {
            ArrayList plugins = new ArrayList();

            DirectoryInfo dir = new DirectoryInfo(PluginDirectory);
            foreach (FileInfo file in dir.GetFiles())
            {
                string name = System.IO.Path.GetFileNameWithoutExtension( file.FullName );
                if (file.Extension.ToLower() == ".dll" && name.EndsWith("Plugin") )
                {
                    Assembly pluginAssembly = Assembly.LoadFile(file.FullName);
                    try
                    {
                        AbstractPlugin plugin = (AbstractPlugin)pluginAssembly.CreateInstance("FieldStat.Plugins."+name);
                        plugin.Name = name;
                        plugins.Add(plugin);
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine("Please name plugin class to be same as dll, ending with Plugin and inheriting from AbstractPlugin" + ex.Message);
                        continue;
                    }
                }
            }
            return plugins;
        }
    }

    public abstract class AbstractPlugin
    {
        public string Name;
        protected bool m_useCommunityVisitor;
        public bool UsesCommunityVisitor
        {
            get { return m_useCommunityVisitor; }
        }
        protected AbstractPlugin( bool useCommunityVisitor )
        {
            m_useCommunityVisitor = useCommunityVisitor;
        }
        public virtual void ComputeResults(Results results, ICollection files, Hashtable htBin, ICollection filters) { }

        // Override the following two methods if (UsesCommunityVisitor == true)
        public virtual AbstractCollector CreateCollector() { return null; }
        public virtual void SetOutputTable( object result ) { }

        public virtual void Export( Results results ){}

    }
}
