using System;

namespace Sungiant.Djinn.Actions
{
    public class ReturnLocationBlock
        : NginxLocationBlock<Specification.ReturnLocationBlock>
    {
        public ReturnLocationBlock(Specification.ReturnLocationBlock specification)
            : base(specification) {}

        public override String[] GetConfig()
        {
            return new String[]
            {
                "location " + Specification.Location + " {",
                Specification.Return == null ? String.Empty : "  return  " + Specification.Return + ";",
                "}",
            };
        }
    }
}

