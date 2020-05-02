using System;

namespace AuroraPatch
{
    public class AuroraVersion
    {
        public Version Version { get; }
        
        public string Checksum { get; }
        
        public string FormType { get; }

        public AuroraVersion(Version version, string checksum, string formType)
        {
            Version = version;
            Checksum = checksum;
            FormType = formType;
        }

        public bool CompareTo(AuroraVersion other)
        {
            return Version.Equals(other.Version) && Checksum.Equals(other.Checksum);
        }
    }
}