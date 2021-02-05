using YAXLib;

namespace YAXLibTests.SampleClasses
{
    public class AttributeInheritanceWithPropertyOverride : AttributeInheritance
    {
        public override string Gender { get { return "Female"; } }  // should inherit the base's YAXSerializeAs attribute

        [YAXSerializeAs("CurrentAge")]   // should override the base's YAXSerializeAs attribute
        public override double Age
        {
            get { return base.Age; }
            set { base.Age = value; }
        }

        public static new AttributeInheritanceWithPropertyOverride GetSampleInstance()
        {
            return new AttributeInheritanceWithPropertyOverride()
            {
                Name = "Sally",
                Age = 38.7
            };
        }
    }

    [YAXSerializeAs("Child")]
    public class AttributeInheritance : AttributeInheritanceBase
    {
        [YAXSerializeAs("TheAge")]
        public virtual double Age { get; set; }

        public static AttributeInheritance GetSampleInstance()
        {
            return new AttributeInheritance()
            {
                Name = "John",
                Age = 30.2
            };
        }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }
    }

    [YAXSerializeAs("Base")]
    public class AttributeInheritanceBase
    {
        [YAXSerializeAs("TheName")]
        public string Name { get; set; }

        [YAXSerializeAs("TheGender")]
        public virtual string Gender { get { return "Unknown";  } }
    }
}
