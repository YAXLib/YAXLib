using System;
using System.Collections.Generic;
using System.Collections;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication]

    [YAXComment("This sample demonstrates serialization of non-generic collection classes")]
    public class NonGenericCollectionsSample
    {
        public List<object> ObjList { get; set; }

        public ArrayList TheArrayList { get; set; }

        public Hashtable TheHashtable { get; set; }

        public Queue TheQueue { get; set; }

        public Stack TheStack { get; set; }

        public SortedList TheSortedList { get; set; }

        public BitArray TheBitArray { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static NonGenericCollectionsSample GetSampleInstance()
        {
            List<object> lst = new List<object>();
            lst.Add(1);
            lst.Add(3.0);
            lst.Add("Hello");
            lst.Add(new DateTime(2010, 3, 4));
            lst.Add(new Author() { Name = "Charles", Age = 50 });

            ArrayList arLst = new ArrayList();
            arLst.Add(2);
            arLst.Add(8.5);
            arLst.Add("Hi");
            arLst.Add(new Author() { Name = "Steve", Age = 30 });

            Hashtable table = new Hashtable();
            table.Add(1.0, "Tim");
            table.Add("Tom", "Sam");
            table.Add(new DateTime(2009, 2, 1), 7);

            BitArray bitArray = new BitArray(10);
            bitArray[1] = true;
            bitArray[6] = true;

            Queue queue = new Queue();
            queue.Enqueue(10);
            queue.Enqueue(20);
            queue.Enqueue(30);

            Stack stack = new Stack();
            stack.Push(100);
            stack.Push(200);
            stack.Push(300);


            SortedList sortedList = new SortedList();
            sortedList.Add(1, 2);
            sortedList.Add(5, 7);
            sortedList.Add(8, 2);

            return new NonGenericCollectionsSample()
            {
                ObjList = lst,
                TheArrayList = arLst,
                TheHashtable = table,
                TheBitArray = bitArray,
                TheQueue = queue,
                TheStack = stack,
                TheSortedList = sortedList
            };



        }

        #region Equality members

        protected bool Equals(NonGenericCollectionsSample other)
        {
            return EqualsHelpers.CollectionEquals(ObjList, other.ObjList) 
                && EqualsHelpers.CollectionEquals(TheArrayList, other.TheArrayList) 
                && EqualsHelpers.DictionaryEquals(TheHashtable, other.TheHashtable)
                && EqualsHelpers.CollectionEquals(TheQueue, other.TheQueue)
                && EqualsHelpers.CollectionEquals(TheStack, other.TheStack) 
                && EqualsHelpers.DictionaryEquals(TheSortedList, other.TheSortedList)
                && EqualsHelpers.CollectionEquals(TheBitArray, other.TheBitArray);
        }


        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NonGenericCollectionsSample) obj);
        }

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ObjList != null ? ObjList.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TheArrayList != null ? TheArrayList.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TheHashtable != null ? TheHashtable.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TheQueue != null ? TheQueue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TheStack != null ? TheStack.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TheSortedList != null ? TheSortedList.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TheBitArray != null ? TheBitArray.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
