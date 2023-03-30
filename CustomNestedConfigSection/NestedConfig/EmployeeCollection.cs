using System;
using System.Configuration;

namespace CustomNestedConfigSections.NestedConfig
{
    public class EmployeeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Employee();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Employee) element).Id;
        }

        protected override string ElementName
        {
            get
            {
                return "employee";
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return !String.IsNullOrEmpty(elementName) && elementName == "employee";
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public Employee this[int index]
        {
            get
            {
                return base.BaseGet(index) as Employee;
            }
        }

        public new Employee this[string key]
        {
            get
            {
                return base.BaseGet(key) as Employee;
            }
        }
    }
}