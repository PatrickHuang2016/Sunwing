using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication1.Models;
using WebApplication1.Models.DTO;

namespace WebApplication1.Controllers
{
    public class SuppliersController : ApiController
    {
        private Database1Entities1 db = new Database1Entities1();
        Person supplier1;

        // GET: api/Suppliers
        public IQueryable<SupplierDTO> GetPeople()
        {
            var l = from p in db.People.OfType<Supplier>() select new SupplierDTO { Id = p.Id, FullName = p.Name.First + " " + p.Name.Last, Telephone = p.Telephone, Name_Id = p.Name_Id };
            IQueryable<SupplierDTO> result = l.AsQueryable();
            return result;
        }

        // GET: api/Suppliers/5
        [ResponseType(typeof(Supplier))]
        public IHttpActionResult GetSupplier(long id)
        {
           
           


            db.Configuration.ProxyCreationEnabled = false;
            var l = from p in db.People.OfType<Supplier>() where p.Id == id select new SupplierDTO { Id = p.Id, FullName = p.Name.First + " " + p.Name.Last, Telephone = p.Telephone , name = p.Name };
            SupplierDTO supplier = l.FirstOrDefault();
            var nameId = from n in db.Names
                         where n.First == supplier.name.First && n.Last == supplier.name.Last
                         select n.Id;
            if (supplier == null)
            {
                return NotFound();
            }
      try { 
            supplier.Name_Id = nameId.FirstOrDefault();
           
            } catch(Exception e) { }
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(supplier);
            return Ok(json);
        }

        // PUT: api/Suppliers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSupplier(long id, Supplier supplier)
        {
            var nameId = from n in db.Names
                         where n.First == supplier.Name.First && n.Last == supplier.Name.Last
                         select n.Id;
            IQueryable<long> nameIdNew = null;
            if (nameId.FirstOrDefault() == 0)
            {
                Name name = new Name();
                name.First = supplier.Name.First;
                name.Last = supplier.Name.Last;
                db.Names.Add(name);
                db.SaveChanges();

                //db.People.Add(customer);
                //db.SaveChanges();
                nameIdNew = from n in db.Names.AsNoTracking()
                            where n.First == supplier.Name.First && n.Last == supplier.Name.Last
                            select n.Id;
                var newnameid = nameIdNew.FirstOrDefault();

                supplier1 = (from n in db.People where n.Id == supplier.Id select n).First();
                supplier1.Name_Id = newnameid;

                db.SaveChanges();

            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supplier.Id)
            {
                return BadRequest();
            }


            if (nameId.FirstOrDefault() == 0)
            {
                db.People.Attach(supplier1);
                db.Entry(supplier1).Property(x => x.Name_Id).IsModified = true;
            }
            else
            {
                db.Entry(supplier).State = EntityState.Modified;

            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //}


            string json = "{ \"Id\":" + supplier.Id + ",\"Telephone\":\"" + supplier.Telephone + "\",\"FullName\":\"" + supplier.Name.First + " " + supplier.Name.Last + "\"}";



            SupplierDTO sup = JsonConvert.DeserializeObject<SupplierDTO>(json);
           
            return Ok(sup);
         
        }

        // POST: api/Suppliers
        [ResponseType(typeof(Supplier))]
        public IHttpActionResult PostSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nameId = from n in db.Names
                         where n.First == supplier.Name.First && n.Last == supplier.Name.Last
                         select n.Id;
            IQueryable<long> nameIdNew = null;
            if (nameId.Count() == 0)
            {
                Name name = new Name();
                name.First = supplier.Name.First;
                name.Last = supplier.Name.Last;
                //db.Names.Add(name);
                //db.SaveChanges();

                db.People.Add(supplier);
                db.SaveChanges();
                nameIdNew = from n in db.Names.AsNoTracking()
                            where n.First == supplier.Name.First && n.Last == supplier.Name.Last
                            select n.Id;
                var newnameid = nameIdNew.FirstOrDefault();

                supplier1 = (from n in db.People where n.Id == supplier.Id select n).First();
                supplier1.Name_Id = newnameid;

                db.People.Attach(supplier1);
                db.Entry(supplier1).Property(x => x.Name_Id).IsModified = true;
                db.SaveChanges();

            }
            else
            {

                db.People.Add(supplier);
                db.SaveChanges();


                // db.SaveChanges();

                nameIdNew = from n in db.Names.AsNoTracking()
                            where n.First == supplier.Name.First && n.Last == supplier.Name.Last
                            orderby n.Id
                            select n.Id;
                var nameIdNew1 = from n in db.Names.AsNoTracking()
                                 where n.First == supplier.Name.First && n.Last == supplier.Name.Last
                                 orderby n.Id descending
                                 select n.Id;
                supplier1 = (from n in db.People where n.Id == supplier.Id select n).First();
                var newnameid = nameIdNew.FirstOrDefault();
                try
                {
                    var oldnameid = nameIdNew1.FirstOrDefault();

                    // var oldnameid = nameIdNew.LastOrDefault();
                    supplier1.Name_Id = newnameid;
                    db.People.Attach(supplier1);
                    db.Entry(supplier1).Property(x => x.Name_Id).IsModified = true;
                    db.SaveChanges();


                    var na = from p in db.Names where p.Id == oldnameid select p;
                    Name namedel = na.FirstOrDefault();
                    db.Entry(namedel).State = EntityState.Deleted;

                    // db.Names.Remove(namedel);

                    db.SaveChanges();
                }
                catch (Exception e) { }
            }


            string json = "{ \"Id\":" + supplier.Id + ",\"Telephone\":\"" + supplier.Telephone + "\",\"FullName\":\"" + supplier.Name.First + " " + supplier.Name.Last + "\"}";

            SupplierDTO sup = JsonConvert.DeserializeObject<SupplierDTO>(json);    
            return Ok(sup);
           
        }

        // DELETE: api/Suppliers/5
        [ResponseType(typeof(Supplier))]
        public IHttpActionResult DeleteSupplier(long id)
        {
            var l = from p in db.People where p.Id == id select p;
            Person supplier = l.FirstOrDefault();
            if (supplier == null)
            {
                return NotFound();
            }

            db.People.Remove(supplier);
            db.SaveChanges();

            return Ok(supplier);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierExists(long id)
        {
            return db.People.Count(e => e.Id == id) > 0;
        }
    }
}