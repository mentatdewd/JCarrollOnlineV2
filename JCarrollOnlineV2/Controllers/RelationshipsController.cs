using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JCarrollOnlineV2.DataContexts;
using JCarrollOnlineV2.Entities;

namespace JCarrollOnlineV2.Controllers
{
    [Authorize]
    public class RelationshipsController : Controller
    {
        private JCarrollOnlineV2Db db = new JCarrollOnlineV2Db();

    //    // GET: Relationships
    //    public async Task<ActionResult> Index()
    //    {
    //        return View(await db.Relationships.ToListAsync());
    //    }

    //    // GET: Relationships/Details/5
    //    public async Task<ActionResult> Details(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        Relationship relationship = await db.Relationships.FindAsync(id);
    //        if (relationship == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(relationship);
    //    }

    //    // GET: Relationships/Create
    //    public ActionResult Create()
    //    {
    //        return View();
    //    }

    //    // POST: Relationships/Create
    //    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    //    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Create([Bind(Include = "Id,FollowerId,FollowedId,CreatedAt,UpdatedAt")] Relationship relationship)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            db.Relationships.Add(relationship);
    //            await db.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }

    //        return View(relationship);
    //    }

    //    // GET: Relationships/Edit/5
    //    public async Task<ActionResult> Edit(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        Relationship relationship = await db.Relationships.FindAsync(id);
    //        if (relationship == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(relationship);
    //    }

    //    // POST: Relationships/Edit/5
    //    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    //    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Edit([Bind(Include = "Id,FollowerId,FollowedId,CreatedAt,UpdatedAt")] Relationship relationship)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            db.Entry(relationship).State = EntityState.Modified;
    //            await db.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }
    //        return View(relationship);
    //    }

    //    // GET: Relationships/Delete/5
    //    public async Task<ActionResult> Delete(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        Relationship relationship = await db.Relationships.FindAsync(id);
    //        if (relationship == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(relationship);
    //    }

    //    // POST: Relationships/Delete/5
    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> DeleteConfirmed(int id)
    //    {
    //        Relationship relationship = await db.Relationships.FindAsync(id);
    //        db.Relationships.Remove(relationship);
    //        await db.SaveChangesAsync();
    //        return RedirectToAction("Index");
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            db.Dispose();
    //        }
    //        base.Dispose(disposing);
    //    }
    }
}
