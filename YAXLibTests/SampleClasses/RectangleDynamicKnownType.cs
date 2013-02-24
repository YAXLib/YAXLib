using System.Drawing;

namespace YAXLibTests.SampleClasses
{
    public class RectangleDynamicKnownType
    {
        public Rectangle Rect { get; set; }

        public static RectangleDynamicKnownType GetSampleInstance()
        {
            return new RectangleDynamicKnownType {Rect = new Rectangle(10, 20, 30, 40)};
        }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        } 
    }
}
