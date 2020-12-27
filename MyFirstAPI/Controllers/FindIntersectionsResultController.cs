using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MyFirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MyFirstAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FindIntersectionsResultController : ControllerBase
    {
        DataContext db;

        public FindIntersectionsResultController(DataContext context)
        {
            db = context;
        }

        /// <summary>
        /// Отправляешь GUID - получаешь информацию о кривых и их пересечениях :)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<FindIntersectionsResult>> GetIntersectionsResult(Guid guid)
        {
            try 
            {
                var existData = await db.DataTable.Include(c1 => c1.Curve1.Points).Include(c2 => c2.Curve2.Points)
                    .Include(p => p.PointIntersaction)
                    .FirstOrDefaultAsync(x => x.Guid == guid);
                if (existData is null) return NotFound();
                else return new FindIntersectionsResult(existData);

            }
            catch
            {
                return BadRequest();
            }
            
        }
    }
}
