using System;
using System.IO;
using Sungiant.Cloud;
using ServiceStack.Text;

using DjinnCommand = Sungiant.Djinn.Command;

namespace Sungiant.Djinn.Actions
{
    public class MdToolBuild
        : Action<Specification.MdToolBuild>
    {
        public MdToolBuild(Specification.MdToolBuild specification, String djinnContext) 
            : base(specification, djinnContext) {}

        String SolutionPath
        {
            get
            {
                switch(Context)
                {
                    case MachineContext.Local: return Path.Combine (DjinnContext, Specification.SolutionPath);
                    case MachineContext.Remote: default: return Specification.SolutionPath;
                }
            }
        }

        MachineContext Context
        { 
            get { return Specification.IsContextRemote ? MachineContext.Remote : MachineContext.Local; }
        }

        public override DjinnCommand[] GetRunnableCommands (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
        {
            if(!File.Exists (SolutionPath))
            {
                throw new Exception ("Solution file not found.");
            }

            String command = "/Applications/Xamarin Studio.app/Contents/MacOS/mdtool";

            var arguments = new String[]
            {
                "build",
                "\"--counfiguration:" + Specification.Configuration + "\"",
                String.Format("\"{0}\"", SolutionPath)
            }.Join(" ");

            if (Specification.Verbose)
            {
                arguments = "-v " + arguments;
            }

            String cmd = command + " " + arguments;

            var result = new DjinnCommand[] {
                new DjinnCommand {
                    MachineContext = Context,
                    Value = cmd
                }
            };

            return result;
        }

    }
}

