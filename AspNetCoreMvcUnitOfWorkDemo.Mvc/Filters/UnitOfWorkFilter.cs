using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreMvcUnitOfWorkDemo.Mvc.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Storage;

namespace AspNetCoreMvcUnitOfWorkDemo.Mvc.Filters
{
    /// <summary>
    /// All 'GET' request is done without tracking.
    /// All other requests is done within an transaction - you can safely use SaveChanges around your code - it will not be committed until end of request.
    /// </summary>
    public class UnitOfWorkFilter : IActionFilter
    {
        private readonly MyDatabaseContext _db;
        private IDbContextTransaction _tx;

        public UnitOfWorkFilter(MyDatabaseContext db)
        {
            _db = db;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.HttpContext.Request.Method == "GET")
            {
                _db.ChangeTracker.AutoDetectChangesEnabled = false;
                return;
            }

            _tx = _db.Database.BeginTransaction();
            
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.HttpContext.Request.Method == "GET") return;

            if (context.Exception == null)
            {
                _db.SaveChangesAsync().Wait(new TimeSpan(0, 0, 10));
                _tx.CommitAsync().Wait(new TimeSpan(0, 0, 10)); 
            }
            else
            {
                _tx.RollbackAsync().GetAwaiter().GetResult(); ;
            }
        }

    }
}
