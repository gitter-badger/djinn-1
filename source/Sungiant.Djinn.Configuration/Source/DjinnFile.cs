using System;
using System.IO;

namespace Sungiant.Djinn.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class DjinnFile
    {
        /// <summary>
        /// 
        /// </summary>
        public class ProjectConfiguration
        {
            public String ProjectIdentifier { get; set; }
            public String DjinnDirectory { get; set; }

            public String BlueprintsDirectory
            {
                get
                {
                    var path = Path.Combine (DjinnDirectory, "blueprints");
                    
                    var fullpath = PathHelper.ResolveDirectoryForUser(path);

                    return fullpath;
                }
            }
            
            public String ZonesDirectory
            {
                get
                {
                    var path = Path.Combine (DjinnDirectory, "zones");
                    
                    var fullpath = PathHelper.ResolveDirectoryForUser(path);

                    return fullpath;
                }
            }
        }

        /// <summary>
        /// A workgroup represents a group of 
        /// </summary>
        public class Workgroup
        {
            public String WorkgroupIdentifier { get; set; }
            public ProjectConfiguration[] ProjectConfigurations { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Workgroup[] Workgroups { get; set; }
    }
}

