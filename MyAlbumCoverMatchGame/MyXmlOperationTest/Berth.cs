using System.Collections.Generic;
using System.Linq;

namespace MyXmlOperationTest
{
    public class Port
    {
        public string Name { get; set; }
        public string City { get; set; }
        public List<Berth> Berths { get; set; } = new List<Berth>();
    }

    public class Berth
    {
        public string Name { get; set; }
        public bool IsSeparable { get; set; }
        public double? Length
        {
            get { return GetTotlaLenth(); }
            set { _length = value; }
        }
        public double? MaxDepth
        {
            get { return GetMaxDepth(); }
            set { _maxDepth = value; }
        }
        public string Capacity
        {
            get { return GetCapacity(); }
            set { _capacity = value; }
        }
        public Port Port { get; set; }
        public string PortName { get { return Port.Name; } }
        public List<Berth> SubBerth { get; set; } = new List<Berth>();

        private double? _length;
        private double? _maxDepth;
        private string _capacity;

        private double? GetTotlaLenth()
        {
            if (SubBerth.Count == 0 || SubBerth == null)
            {
                return _length;
            }
            double? totalLength = 0;
            SubBerth.ForEach(i =>
            {
                if (i.Length != null)
                {
                    totalLength += i.Length;
                }
            });
            return totalLength;
        }

        private double? GetMaxDepth()
        {
            if (SubBerth.Count == 0 || SubBerth == null)
            {
                return _maxDepth;
            }

            return (from berth in SubBerth
                    where berth.MaxDepth != null
                    select berth.MaxDepth)
                    .Max();
        }

        private string GetCapacity()
        {
            if (SubBerth.Count == 0 || SubBerth == null)
            {
                return _capacity;
            }

            return SubBerth.Where(b => b.MaxDepth == MaxDepth).First().Capacity;
        }
    }
}