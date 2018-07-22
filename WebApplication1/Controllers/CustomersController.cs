using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Web.Script.Serialization;
using WebApplication1.Models;
using WebApplication1.Models.DTO;

namespace WebApplication1.Controllers
{
    public class CustomersController : ApiController
    {
        private Database1Entities1 db = new Database1Entities1();
        Person customer1;

        // GET: api/Customers
        public IQueryable<CustomerDTO> GetPeople()
        {
            var l = from p in db.People.OfType<Customer>() select new CustomerDTO { Id = p.Id, FullName = p.Name.First + " " + p.Name.Last, BirthDay = p.BirthDay, Email = p.Email, Name_Id = p.Name_Id};
            IQueryable<CustomerDTO> result = l.AsQueryable();
            return result;
        }

        // GET: api/Customers/5
        [ResponseType(typeof(Customer))]
        public IHttpActionResult GetCustomer(long id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var l = from p in db.People.OfType<Customer>() where p.Id == id select 
                   new CustomerDTO { Id = p.Id, BirthDay = p.BirthDay, Email = p.Email, name = p.Name };
           
            CustomerDTO customer = l.FirstOrDefault();
          //  int addName = 1;
            var nameId = from n in db.Names
                         where n.First == customer.name.First && n.Last == customer.name.Last
                         select n.Id ;
           // if (nameId != null) addName = 0;
           
                if (customer == null)
            {
                return NotFound();
            }

           // customer.addName = addName;
            customer.Name_Id = nameId.FirstOrDefault(); 
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
            return Ok(json);
        }

        // PUT: api/Customers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCustomer(long id, Customer customer)
        {
            var nameId = from n in db.Names
                         where n.First == customer.Name.First && n.Last == customer.Name.Last
                         select n.Id;
            IQueryable<long> nameIdNew = null;
            if (nameId.FirstOrDefault() == 0)
            {
                Name name = new Name();
                name.First = customer.Name.First;
                name.Last = customer.Name.Last;
                db.Names.Add(name);
                db.SaveChanges();

                //db.People.Add(customer);
                //db.SaveChanges();
                nameIdNew = from n in db.Names.AsNoTracking()
                            where n.First == customer.Name.First && n.Last == customer.Name.Last
                            select n.Id;
                var newnameid = nameIdNew.FirstOrDefault();

                 customer1 = (from n in db.People where n.Id == customer.Id select n).First();
                customer1.Name_Id = newnameid;
              
                db.SaveChanges();
               
            }

            if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != customer.Id)
                {
                    return BadRequest();
                }
          

            if (nameId.FirstOrDefault() == 0)
            {
                db.People.Attach(customer1);
                db.Entry(customer1).Property(x => x.Name_Id).IsModified = true;
            }
            else
            {
                db.Entry(customer).State = EntityState.Modified;

            }

            try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //}

            
            string json = "{ \"Id\":" + customer.Id + ",\"BirthDay\":\""+ customer.BirthDay + "\",\"Email\":\"" + customer.Email + "\",\"FullName\":\"" + customer.Name.First + " " + customer.Name.Last+ "\"}";
           
      

            CustomerDTO cust = JsonConvert.DeserializeObject<CustomerDTO>(json);
            cust.BirthDay = customer.BirthDay;
           // var cust = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return Ok(cust);
           // return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Customers
        [ResponseType(typeof(Customer))]
        public IHttpActionResult PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nameId = from n in db.Names
                         where n.First == customer.Name.First && n.Last == customer.Name.Last
                         select n.Id;
            IQueryable<long> nameIdNew = null;
            if (nameId.Count() == 0)
            {
                Name name = new Name();
                name.First = customer.Name.First;
                name.Last = customer.Name.Last;
                //db.Names.Add(name);
                //db.SaveChanges();

                db.People.Add(customer);
                db.SaveChanges();
                nameIdNew = from n in db.Names.AsNoTracking()
                            where n.First == customer.Name.First && n.Last == customer.Name.Last
                            select n.Id;
                var newnameid = nameIdNew.FirstOrDefault();
            
                customer1 = (from n in db.People where n.Id == customer.Id select n).First();
                customer1.Name_Id = newnameid;

                db.People.Attach(customer1);
                db.Entry(customer1).Property(x => x.Name_Id).IsModified = true;
                db.SaveChanges();

            }
            else
            {

                db.People.Add(customer);               
                db.SaveChanges();

               
                // db.SaveChanges();

                nameIdNew = from n in db.Names.AsNoTracking()
                            where n.First == customer.Name.First && n.Last == customer.Name.Last
                            orderby n.Id 
                            select n.Id;
              var  nameIdNew1 = from n in db.Names.AsNoTracking()
                            where n.First == customer.Name.First && n.Last == customer.Name.Last
                            orderby n.Id descending
                            select n.Id;
                customer1 = (from n in db.People where n.Id == customer.Id select n).First();
                var newnameid = nameIdNew.FirstOrDefault();
                try
                {
                    var oldnameid = nameIdNew1.FirstOrDefault();
               
               // var oldnameid = nameIdNew.LastOrDefault();
                customer1.Name_Id = newnameid;
                db.People.Attach(customer1);
                db.Entry(customer1).Property(x => x.Name_Id).IsModified = true;
                db.SaveChanges();

              
                    var na = from p in db.Names where p.Id == oldnameid select p;
                    Name namedel = na.FirstOrDefault();
                    db.Entry(namedel).State = EntityState.Deleted;

                   // db.Names.Remove(namedel);

                    db.SaveChanges();
                }
                catch (Exception e) { }
            }





            //db.People.Add(customer);

            //db.SaveChanges();
            //var result = CreatedAtRoute("DefaultApi", new { id = customer.Id }, customer);
            string json = "{ \"Id\":" + customer.Id + ",\"BirthDay\":\"" + customer.BirthDay + "\",\"Email\":\"" + customer.Email + "\",\"FullName\":\"" + customer.Name.First + " " + customer.Name.Last + "\"}";



            CustomerDTO cust = JsonConvert.DeserializeObject<CustomerDTO>(json);
            cust.BirthDay = customer.BirthDay;
            // var cust = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return Ok(cust);
        }

        // DELETE: api/Customers/5
        [ResponseType(typeof(Customer))]
        public IHttpActionResult DeleteCustomer(long id)
        {
           // var l = from p in db.People.OfType<Customer>() where p.Id == id select new CustomerDTO { Id = p.Id, name = p.Name, BirthDay = p.BirthDay, Email = p.Email, Name_Id = p.Name_Id };
            var l = from p in db.People where p.Id == id select p;
            Person customer = l.FirstOrDefault();
           

            if (customer == null)
            {
                return NotFound();
            }

            db.People.Remove(customer);
            db.SaveChanges();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(long id)
        {
            return db.People.Count(e => e.Id == id) > 0;
        }
    }
}