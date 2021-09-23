using System.Collections.Generic;

namespace Navitus
{

    // Custom list class, benefit of this class when serializing into json first entry is the count of the items in the list
    public class ObjectList<T>
    {
        public int Count { get { return List.Count; } }
        public List<T> List { get; set; }

        public ObjectList()
        {
            this.List = new List<T>();
        }


    }
}
