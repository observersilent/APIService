using System.Collections.Generic;


namespace MyFirstAPI.Models
{
    public class CurveTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<PointTable> Points { get; set; }
    }
}
