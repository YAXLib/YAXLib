using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication(SortKey = "003")]

    [YAXComment("This example demonstrates serailizing a very simple struct")]
    public class BookClassWithOrdering
    {
        private string _title;
        private string _author;
        private int _publishYear;
        private double _price;
        private int currentElement = 0;

        [YAXDontSerialize]
        public Dictionary<int, string> DecentralizationOrder = new Dictionary<int, string>();
            
        [YAXElementOrder(1)]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                DecentralizationOrder.Add(currentElement++, "Title");
            }
        }

        [YAXElementOrder(0)]
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                DecentralizationOrder.Add(currentElement++, "Author");
            }
        }

        [YAXElementOrder(3)]
        public int PublishYear
        {
            get { return _publishYear; }
            set
            {
                _publishYear = value;
                DecentralizationOrder.Add(currentElement++, "PublishYear");
            }
        }

        [YAXElementOrder(2)]
        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                DecentralizationOrder.Add(currentElement++, "Price");
            }
        }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static BookClassWithOrdering GetSampleInstance()
        {
            return new BookClassWithOrdering()
            {
                Title = "Reinforcement Learning an Introduction",
                Author = "R. S. Sutton & A. G. Barto",
                PublishYear = 1998,
                Price = 38.75
            };
        }
    }
}
