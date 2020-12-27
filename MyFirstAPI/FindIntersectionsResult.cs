using System;
using System.Collections.Generic;
using MyFirstAPI.Models;

namespace MyFirstAPI
{
    /// <summary>
    /// Результат запроса о пересечений кривых
    /// </summary>
    public class FindIntersectionsResult
    {
        /// <summary>
        /// идентификатор запроса
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// Первая кривая
        /// </summary>
        public Curve Curve1 { get; set; }
        /// <summary>
        /// Вторая кривая
        /// </summary>
        public Curve Curve2 { get; set; }
        /// <summary>
        /// Точки пересечений двух кривых
        /// </summary>
        public IList<Point> Intersections { get; set; }

        public FindIntersectionsResult()
        {

        }

        public FindIntersectionsResult(Data data)
        {
            Guid = data.Guid;
            Curve1 = new Curve();
            Curve1.Name = data.Curve1.Name;
            Curve1.Points = new List<Point>();
            foreach (var i in data.Curve1.Points)
            {
                Curve1.Points.Add(new Point() { X = i.X, Y = i.Y });
            }

            Curve2 = new Curve();
            Curve2.Name = data.Curve2.Name;
            Curve2.Points = new List<Point>();
            foreach (var i in data.Curve2.Points)
            {
                Curve2.Points.Add(new Point() { X = i.X, Y = i.Y });
            }

            Intersections = new List<Point>();
            foreach(var i in data.PointIntersaction)
            {
                Intersections.Add(new Point() { X = i.X, Y = i.Y });
            }
        }
    }
}
