using System.Collections.Generic;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication(SortKey = "003")]

    [YAXComment("This example demonstrates serailizing a very simple class, but with partial priority ordering.")]
    public class BookClassWithOrdering
    {
        private string _title;
        private string _author;
        private int _publishYear;
        private double _price;
        private int currentElement = 0;

        [YAXDontSerialize]
        public Dictionary<int, string> DecentralizationOrder = new Dictionary<int, string>();

        private string _review;
        private string _publisher;
        private string _editor;

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

        public int PublishYear
        {
            get { return _publishYear; }
            set
            {
                _publishYear = value;
                DecentralizationOrder.Add(currentElement++, "PublishYear");
            }
        }

        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                DecentralizationOrder.Add(currentElement++, "Price");
            }
        }

        public string Review
        {
            get { return _review; }
            set
            {
                _review = value; 
                DecentralizationOrder.Add(currentElement++, "Review");
            }
        }

        public string Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value; 
                DecentralizationOrder.Add(currentElement++, "Publisher");
            }
        }

        public string Editor
        {
            get { return _editor; }
            set
            {
                _editor = value; 
                DecentralizationOrder.Add(currentElement++, "Editor");
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
                Price = 38.75,
                Publisher = "MIT Press",
                Review = "This book is very good at being a book.",
                Editor = "MIT Productions"
            };
        }
    }
}
