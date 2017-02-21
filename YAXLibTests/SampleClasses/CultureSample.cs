using System;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXComment("This class contains fields that are vulnerable to culture changes!")]
    public class CultureSample : IComparable<CultureSample>
    {
        public double Number1 { get; set; }

        [YAXAttributeForClass]
        public double Number2 { get; set; }

        public double Number3 { get; set; }

        public double[] Numbers { get; set; }

        public decimal Dec1 { get; set; }

        [YAXAttributeForClass]
        public decimal Dec2 { get; set; }

        public DateTime Date1 { get; set; }

        [YAXAttributeForClass]
        public DateTime Date2 { get; set; }

        public static CultureSample GetSampleInstance()
        {
            return new CultureSample
                       {
                           Date1 = new DateTime(2010, 10, 11, 18, 20, 30),
                           Date2 = new DateTime(2011, 9, 20, 4, 10, 30),
                           Dec1 = 192389183919123.18232131m,
                           Dec2 = 19232389.18391912318232131m,
                           Number1 = 123123.1233,
                           Number2 = 32243.67676,
                           Number3 = 21313.123123,
                           Numbers = new [] { 23213.2132, 123.213, 123.23e32}
                       };
        }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(CultureSample other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var numbersComparison = Numbers.Length.CompareTo(other.Numbers.Length);
            if (numbersComparison != 0) return numbersComparison;
            for (var i = 0; i < Numbers.Length; i++)
            {
                numbersComparison = Numbers[i].CompareTo(other.Numbers[i]);
                if (numbersComparison != 0) return numbersComparison;
            }
            var number1Comparison = Number1.CompareTo(other.Number1);
            if (number1Comparison != 0) return number1Comparison;
            var number2Comparison = Number2.CompareTo(other.Number2);
            if (number2Comparison != 0) return number2Comparison;
            var number3Comparison = Number3.CompareTo(other.Number3);
            if (number3Comparison != 0) return number3Comparison;
            var dec1Comparison = Dec1.CompareTo(other.Dec1);
            if (dec1Comparison != 0) return dec1Comparison;
            var dec2Comparison = Dec2.CompareTo(other.Dec2);
            if (dec2Comparison != 0) return dec2Comparison;
            var date1Comparison = Date1.CompareTo(other.Date1);
            return date1Comparison != 0 ? date1Comparison : Date2.CompareTo(other.Date2);
        }
    }
}
