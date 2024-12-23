using Native.Tool.IniConfig.Linq;
using System.IO;
using WinDownLoad;

namespace MsTool.IniFolder
{
    public sealed class LoadIni
    {
        public static ISection Load(string nodename) 
        {
            
            if (File.Exists(MTComon.Path))
            {
                IObject iobj = IObject.Load(MTComon.Path);
                return iobj[nodename];
            }
            else
            {
                return null;
            }
        }
        public static IObject Load()
        {
            
            if (File.Exists(MTComon.Path))
            {
                IObject iobj = IObject.Load(MTComon.Path);
                return iobj;
            }
            else
            {
                return null;
            }
        }
    }
}
