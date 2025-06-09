using KSP.Localization;
using System;
using System.IO;




/* 
 * THIS IS A STATIC CLASS
 */



namespace ScienceChecklist
{
    internal static class CelestialBodyFilters
    {
        private static readonly Logger _logger = new Logger(Localizer.Format("#LOC_xSci_8"));
        public static ConfigNode Filters { get; set; }
        public static ConfigNode TextFilters { get; set; }
        static CelestialBodyFilters()
        {
            Load();
        }



        public static void Load()
        {
            try
            {

                string filePath = KSPUtil.ApplicationRootPath + "GameData/[x]_Science!/PluginData/science.cfg";

                if (File.Exists(filePath))
                {
                    var node = ConfigNode.Load(filePath);
                    var root = node.GetNode("ScienceChecklist");
                    Filters = root.GetNode("CelestialBodyFilters");
                    TextFilters = root.GetNode("TextFilters");
                }
                //				_logger.Trace( "DONE Loading settings file" );
            }
            catch (Exception e)
            {
                _logger.Info(Localizer.Format("#LOC_xSci_9") + e.ToString());
            }
        }
    }
}
