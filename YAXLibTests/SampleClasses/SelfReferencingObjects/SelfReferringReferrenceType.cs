using System.Text;

namespace YAXLibTests.SampleClasses.SelfReferencingObjects
{
    [ShowInDemoApplication(SortKey = "_")]
    public class SelfReferringReferrenceType
    {
        public string ParentDescription { get; set; }
        public ChildReferrenceType Child { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(ParentDescription);
            sb.Append("|->   ").Append(Child).AppendLine();
            return sb.ToString();
        }

        public static SelfReferringReferrenceType GetSampleInstance()
        {
            var parent = new SelfReferringReferrenceType
            {
                ParentDescription = "I'm Parent",
            };
            
            
            var child = new ChildReferrenceType
            {
                ChildDescription = "I'm Child",
                Parent = parent
            };

            parent.Child = child;
            return parent;
        }
    }

    public class ChildReferrenceType
    {
        public string ChildDescription { get; set; }
        public SelfReferringReferrenceType Parent { get; set; }

        public override string ToString()
        {
            return ChildDescription;
        }
    }
}
