using System;
using System.Collections.Generic;
using System.Text;

namespace WG_test.Property
{
    public class Item
    {
        public string id { get; set; }
        public string title { get; set; }
        public Item[] items { get; set; }
    }
    
    public class Item_score
    {
        public string id { get; set; }
        public double score { get; set; }
    }  
}
