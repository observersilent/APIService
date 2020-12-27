using System;


namespace MyFirstAPI
{
    /// <summary>
    /// Команда поиска точек пересечения двух кривых
    /// </summary>
    public class FindIntersectionsCommand
    {
        /// <summary>
        /// Уникальный идентификатор запроса
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// Первая кривая в запроса
        /// </summary>
        public Curve Curve1 { get; set; }
        /// <summary>
        /// Вторая кривая в запрсе
        /// </summary>
        public Curve Curve2 { get; set; }
    }
}
