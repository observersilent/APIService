using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MyFirstAPI.Models
{
    public class Data
    {
        [Key]
        public Guid Guid { get; set; }
        public CurveTable Curve1 { get; set; }
        public CurveTable Curve2 { get; set; }
        public IList<PointTable> PointIntersaction { get; set; }

        public Data() 
        {

        }

        public Data(FindIntersectionsCommand command) 
        {
            Guid = command.Guid;
            Curve1 = new CurveTable();
            Curve1.Name = command.Curve1.Name;
            Curve1.Points = new List<PointTable>();
            foreach(var i in command.Curve1.Points)
            {
                Curve1.Points.Add(new PointTable() { X = i.X, Y = i.Y});
            }

            Curve2 = new CurveTable();
            Curve2.Name = command.Curve2.Name;
            Curve2.Points = new List<PointTable>();
            foreach (var i in command.Curve2.Points)
            {
                Curve2.Points.Add(new PointTable() { X = i.X, Y = i.Y });
            }
        }
    }
}
