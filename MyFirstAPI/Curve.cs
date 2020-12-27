using System.Collections.Generic;

namespace MyFirstAPI
{
    /// <summary>
    /// Ломанная линия(кривая)
    /// </summary>
    public class Curve
    {
        /// <summary>
        /// Имя кривай
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Список точек кривой
        /// </summary>
        public IList<Point> Points { get; set; }
    }
}
