using System.Configuration;

namespace CustomNestedConfigSections.NestedConfig
{
    public class PersonalInfo : ConfigurationElement
    {
        [ConfigurationProperty("ssn", IsKey = true)]
        public string SSN { get { return (string)this["ssn"]; } set { this["ssn"] = value; } }

        [ConfigurationProperty("height")]
        public int Height { get { return (int)this["height"]; } set { this["height"] = value; } }

        [ConfigurationProperty("weight")]
        public int Weight { get { return (int)this["weight"]; } set { this["weight"] = value; } }
    }
}