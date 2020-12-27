using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MyFirstAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurveIntersectionsController : ControllerBase
    {
        DataContext db;
        public CurveIntersectionsController(DataContext context)
        {
            db = context;
        }
        
        //Класс линии, содержит коэффициенты уравнения линии
        public class Line
        {
            public Line(double A, double B, double C, Point pointStart, Point pointEnd)
            {
                this.A = A;
                this.B = B;
                this.C = C;
                this.pointStart = pointStart;
                this.pointEnd = pointEnd;
            }

            public Point pointStart;
            public Point pointEnd;
            public double A { get; set; }
            public double B { get; set; }
            public double C { get; set; }

        }

        //Функция возвращает линию построенную по двум точкам (A, B, C)
        public static Line getLine(Point lineStart, Point lineEnd)
        {
            //коэффициенты прямой соединиющей 
            double A = lineStart.Y - lineEnd.Y;
            double B = lineEnd.X - lineStart.X;
            double C = -((lineStart.X * lineEnd.Y) - (lineEnd.X*lineStart.Y));

            return new Line(A, B, C, lineStart, lineEnd);
        }

        //Функция ищет пересечения двух отрезков.
        //ВНИМАНИЕ: если отрезки лежат на одной прямой - это не считается пересечением
        public static Point getIntersectionLine(Line line1, Line line2)
        {
            double D = (line1.A * line2.B) - (line1.B * line2.A);
            //если линии параллельны - точки пересечения нет
            if (D == 0) return null;
            double Dx = (line1.C * line2.B) - (line1.B * line2.C);
            double Dy = (line1.A * line2.C) - (line1.C * line2.A);
            //получаем координаты точки пересечения ЛИНИЙ, округленные до сотых
            double x = Math.Round(Dx / D, 2);
            double y = Math.Round(Dy / D, 2);

            //теперь надо посмотреть находится ли точка пересечения в границах ОТРЕЗКОВ, если нет - пересечения тоже нет

            double minLine1X = Math.Min(line1.pointStart.X, line1.pointEnd.X);
            double maxLine1X = Math.Max(line1.pointStart.X, line1.pointEnd.X);
            double minLine1Y = Math.Min(line1.pointStart.Y, line1.pointEnd.Y); 
            double maxLine1Y = Math.Max(line1.pointStart.Y, line1.pointEnd.Y); 

            double minLine2X = Math.Min(line2.pointStart.X, line2.pointEnd.X);
            double maxLine2X = Math.Max(line2.pointStart.X, line2.pointEnd.X);
            double minLine2Y = Math.Min(line2.pointStart.Y, line2.pointEnd.Y);
            double maxLine2Y = Math.Max(line2.pointStart.Y, line2.pointEnd.Y);


            bool onLine1 = minLine1X <= x && x <= maxLine1X && minLine1Y <= y && y <= maxLine1Y;

            bool onLine2 = minLine2X <= x && x <= maxLine2X && minLine2Y <= y && y <= maxLine2Y;

            if (!(onLine1 && onLine2))
            {
                return null;
            }

            return new Point() { X = x, Y = y };
        }

        //Функция ищет пересечения двух ломанных       
        public static CurveIntersections FindItersections(FindIntersectionsCommand command)
        {
            //Точки кривых
            List<Point> pointCurve1 = command.Curve1.Points.ToList();
            List<Point> pointCurve2 = command.Curve2.Points.ToList();
            //Точки пересечений
            List<Point> pointIntersection = new List<Point>();

            //если одна из линий представляет собой 1 точку, то пересечений ЛИНИЙ не будет.
            //if (pointCurve1.Count < 2 || pointCurve2.Count < 2) { return null; }

            for (int i = 0; i < pointCurve1.Count - 1; i++)
            {
                var line1 = getLine(pointCurve1[i], pointCurve1[i + 1]);
                for (int j = 0; j < pointCurve2.Count - 1; j++)
                {
                    var line2 = getLine(pointCurve2[j], pointCurve2[j + 1]);
                    var pointInter = getIntersectionLine(line1, line2);
                    if (pointInter != null)
                    {
                        if (pointIntersection.Count == 0)
                            pointIntersection.Add(pointInter);
                        //чтоб не попали дубли точек
                        else if (!pointIntersection.Any(p => p.X == pointInter.X && p.Y == pointInter.Y))
                            pointIntersection.Add(pointInter);
                    };
                }
            }

            return new CurveIntersections() { Points = pointIntersection };
        }

        /// <summary>
        /// Отправляешь кривые - получаешь их пересечения :)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CurveIntersections>> Post(FindIntersectionsCommand command)
        {
            //Получаем точки пересечения
            var intersactionPoint = FindItersections(command);

            var insertData = new Data(command);
            insertData.PointIntersaction = new List<PointTable>();
            foreach(var i in intersactionPoint.Points)
            {
                insertData.PointIntersaction.Add(new PointTable() { X = i.X, Y = i.Y});
            }

            try
            {
                var existData = await db.DataTable.Include(c1 => c1.Curve1.Points).Include(c2 => c2.Curve2.Points)
                    .Include(p => p.PointIntersaction)
                    .FirstOrDefaultAsync(x => x.Guid == command.Guid);

                if (existData is null) { db.DataTable.Add(insertData); }
                else
                {
                    //вычистим старые данные
                    foreach (var i in existData.PointIntersaction)
                    {
                        db.Points.Remove(db.Points.Where(x => x.Id == i.Id).SingleOrDefault());
                    }

                    foreach (var i in existData.Curve1.Points)
                    {
                        db.Points.Remove(db.Points.Where(x => x.Id == i.Id).SingleOrDefault());
                    }

                    foreach (var i in existData.Curve2.Points)
                    {
                        db.Points.Remove(db.Points.Where(x => x.Id == i.Id).SingleOrDefault());
                    }

                    db.Curves.Remove(db.Curves.Where(x => x.Id == existData.Curve1.Id).SingleOrDefault());
                    db.Curves.Remove(db.Curves.Where(x => x.Id == existData.Curve2.Id).SingleOrDefault());
                    db.DataTable.Remove(db.DataTable.Where(x => x.Guid == existData.Guid).SingleOrDefault());

                    //добавим новые
                    db.DataTable.Add(insertData);
                }

                await db.SaveChangesAsync();
                return intersactionPoint;
            }
            catch 
            {
                db.Dispose();
                return BadRequest();
            }
        }
    }
}
