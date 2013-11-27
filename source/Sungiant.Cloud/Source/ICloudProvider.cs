using System;
using System.Collections.Generic;

namespace Sungiant.Cloud
{
	// this can be used to identify a cloud entity
	public class CloudDeploymentIdentity
		: IEquatable<CloudDeploymentIdentity>
	{
		// A collection of tags that can each, individually, uniquely identify
		// members of a running cloud deployment.
		public List<CloudDeploymentIdentifierTag> IdentiferTags { get; set; }

		// A collection of tags groups that can each, individually, uniquely identify
		// members of a running cloud deployment.
		public List<CloudDeploymentIdentifierTagGroup> IdentiferTagGroups { get; set; }

		// A collection of tags holding informational data about this
		// cloud deployment.
		public List<CloudDeploymentIdentifierTag> InformationalTags { get; set; }
		
		
		public override string ToString ()
		{
			string result = string.Empty;
			
			IdentiferTags.ForEach(x => result += "[" + x.Value + "] " );
			
			IdentiferTagGroups.ForEach(x => x.PartialIdentiferTags.ForEach(y => result += "(" + y.Value + ") " ));
			
			InformationalTags.ForEach(x => result += "{" + x.Value + "} " );
			
			return result;	
		}
		
		public Boolean Equals(CloudDeploymentIdentity other)
		{
			if( (Object) other == null)
				return false;
				
			return other == this;
		}
		
		public override int GetHashCode ()
		{
			return IdentiferTags.GetHashCode () ^ IdentiferTagGroups.GetHashCode() ^ InformationalTags.GetHashCode();
		}
		
		public override Boolean Equals (object obj)
		{
			if( obj != null )
			{
				var other = obj as CloudDeploymentIdentity;
			
				if( other != null )
				{
					return other == this;
				}
			}
			
			return false;
		}
		
		public static Boolean operator != (CloudDeploymentIdentity one, CloudDeploymentIdentity other)
		{
			return !(one == other);
		}
		
		public static Boolean operator == (CloudDeploymentIdentity one, CloudDeploymentIdentity other)
		{		
			if(one.IdentiferTags.Count != other.IdentiferTags.Count)
				return false;
				
			if(one.IdentiferTagGroups.Count != other.IdentiferTagGroups.Count)
				return false;
				
			if(one.InformationalTags.Count != other.InformationalTags.Count)
				return false;
				
			for(int i = 0; i < one.IdentiferTags.Count; ++i)
			{
				if( one.IdentiferTags[i] != other.IdentiferTags[i] )
					return false;
			}
			
			for(int i = 0; i < one.IdentiferTagGroups.Count; ++i)
			{
				if( one.IdentiferTagGroups[i] != other.IdentiferTagGroups[i] )
					return false;
			}
			
			for(int i = 0; i < one.InformationalTags.Count; ++i)
			{
				if( one.InformationalTags[i] != other.InformationalTags[i] )
					return false;
			}
			
			return true;
		}
	}

	public class CloudDeploymentIdentifierTag
		: IEquatable<CloudDeploymentIdentifierTag>
	{
		// Tag name.
		public string Name { get; set; } 

		// Tag value.
		public string Value { get; set; }
		
		public Boolean Equals(CloudDeploymentIdentifierTag other)
		{
			if( other == null)
				return false;
				
			return other == this;
		}
		
		public override int GetHashCode ()
		{
			return Name.GetHashCode () ^ Value.GetHashCode();
		}
		
		public override Boolean Equals (object obj)
		{
			if( obj != null )
			{
				var other = obj as CloudDeploymentIdentifierTag;
			
				if( other != null )
				{
					return other == this;
				}
			}
			
			return false;
		}
		
		public static Boolean operator != (CloudDeploymentIdentifierTag one, CloudDeploymentIdentifierTag other)
		{
			return !(one == other);
		}
		
		public static Boolean operator == (CloudDeploymentIdentifierTag one, CloudDeploymentIdentifierTag other)
		{
			if( one.Name != other.Name )
				return false;
				
			if( one.Value !=  other.Value )
				return false;
				
			return true;
				
		}
	}

	public class CloudDeploymentIdentifierTagGroup
		: IEquatable<CloudDeploymentIdentifierTagGroup>
	{
		// A collection of tags, which when used all together can uniquely identify
		// members of a running cloud deployment.
		public List<CloudDeploymentIdentifierTag> PartialIdentiferTags { get; set; }
		
		public Boolean Equals(CloudDeploymentIdentifierTagGroup other)
		{
			if( other == null)
				return false;
				
			return other == this;
		}
		
		public override int GetHashCode ()
		{
			return PartialIdentiferTags.GetHashCode ();
		}
		
		public override Boolean Equals (object obj)
		{
			if( obj != null )
			{
				var other = obj as CloudDeploymentIdentifierTagGroup;
			
				if( other != null )
				{
					return other == this;
				}
			}
			
			return false;
		}
		
		public static Boolean operator != (CloudDeploymentIdentifierTagGroup one, CloudDeploymentIdentifierTagGroup other)
		{
			return !(one == other);
		}
		
		public static Boolean operator == (CloudDeploymentIdentifierTagGroup one, CloudDeploymentIdentifierTagGroup other)
		{
			if(one.PartialIdentiferTags.Count != other.PartialIdentiferTags.Count)
				return false;
				
			for(int i = 0; i < one.PartialIdentiferTags.Count; ++i)
			{
				if( one.PartialIdentiferTags[i] != other.PartialIdentiferTags[i] )
					return false;
			}
			
			return true;
				
		}
	}

	public class CloudSecurityGroup
	{
		public List<CloudSecurityGroupRule> Rules { get; set; }
		public String Name { get; set; }
		public String Description { get; set; }
	}

	public class CloudSecurityGroupRule
	{
		public enum TransportMode
		{
			TCP,
			UDP,
		}

		public Int32 Port { get; set; }
		public TransportMode Mode { get; set; }
	}
	
	public enum CloudDeploymentStatus
	{
		Online,
		Busy,
		Offline
	}

	public interface ICloudDeployment
	{
		Int32 VerticleScale { get; }
		Int32 HorizontalScale { get; }
		List<String> Endpoints { get; }
		CloudDeploymentIdentity Identifier { get; }
		CloudDeploymentStatus Status { get; }
	}

	public interface ICloudProvider
	{
		void Launch(
			CloudDeploymentIdentity identifier,
			CloudSecurityGroup securityGroup,
			Int32 verticleScale,
			Int32 horizontalScale
			);

		void Destroy(CloudDeploymentIdentity identifier);

		void RunCommand(ICloudDeployment deployment, String command, Boolean ignoreFailure = false);

		void RunCommands(ICloudDeployment deployment, String[] commands, Boolean ignoreFailures = false);

		ICloudDeployment Describe(CloudDeploymentIdentity identifier);

		String PrivateKeyPath { get; }

		String User { get; }

		void PrintStatus();
	}
}

