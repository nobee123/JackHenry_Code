namespace Reddit.Models
{
    public class Child
    {
        public string kind { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string after { get; set; }
      
        public List<Child> children { get; set; }
        public object before { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public int ups { get; set; }       
    }
}
