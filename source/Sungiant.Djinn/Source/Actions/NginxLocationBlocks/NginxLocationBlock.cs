using System;

namespace Sungiant.Djinn
{
	public abstract class NginxLocationBlock
	{
		public abstract String[] GetConfig();

		public static NginxLocationBlock CreateFromSpecification(Specification.INginxLocationBlock specification)
		{
			Type t = Type.GetType ("Sungiant.Djinn." + specification.Type + ", Sungiant.Djinn");

			Object o = Activator.CreateInstance (t, specification);

			return o as NginxLocationBlock;
		}
	}

	public abstract class NginxLocationBlock<T>
		: NginxLocationBlock
		where T
		: Specification.INginxLocationBlock
	{
		readonly T specification;

		protected T Specification { get { return specification; } }

		protected NginxLocationBlock (T specification)
		{
			this.specification = specification;
		}
	}
}

