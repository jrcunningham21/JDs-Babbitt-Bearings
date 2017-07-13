using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JDsDataModel;
using JDsDataModel.ViewModels;
using JDsWebApp.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;

namespace JDsWebApp.Areas.EmployeePortal.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class CustomerController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        //private readonly JDsDBEntities db = new JDsDBEntities();

        public ActionResult CustomerMgmt()
        {
            return View();
        }

        public ActionResult ReadCustomers()
        {
            var db = new JDsDBEntities();
            var customers = db.Customers.Include("Address").Include("Contact").Where(x => x.IsActive == true).OrderBy(x => x.CompanyName).ToList();

            var result = customers.Select(x =>
                new CustomerSummaryViewModel()
                {
                    CustomerId = x.CustomerId,
                    CompanyName = x.CompanyName,
                    CompanyCode = x.Code,
                    BillingAddressId = x.BillingAddressId,
                    BillingAddressLine1 = x.Address == null ? string.Empty : x.Address.AddressLine1,
                    BillingAddressLine2 = x.Address == null ? string.Empty : x.Address.AddressLine2,
                    BillingCity = x.Address == null ? string.Empty : x.Address.City,
                    BillingState = x.Address == null ? string.Empty : x.Address.State,
                    BillingZip = x.Address == null ? string.Empty : x.Address.Zip,
                    BillingCountry = x.Address == null ? string.Empty : x.Address.Country,
                    ContactFirstName = x.Contact == null ? string.Empty : x.Contact.FirstName,
                    ContactLastName = x.Contact == null ? string.Empty : x.Contact.LastName,
                    ContactEmail = x.Contact == null ? string.Empty : x.Contact.Email,
                    ContactWorkPhone = x.Contact == null ? string.Empty : x.Contact.WorkPhone,
                    ContactCellPhone = x.Contact == null ? string.Empty : x.Contact.CellPhone,
                    ContactFax = x.Contact == null ? string.Empty : x.Contact.Fax,
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadContacts(int customerId)
        {
            var db = new JDsDBEntities();
            var contacts = db.Contacts.Include("Customer").Where(x => x.IsActive.HasValue && x.IsActive.Value && x.CustomerId == customerId).OrderBy(x => x.FirstName).ToList();

            var result = contacts.Select(x =>
               new ContactViewModel()
               {
                   ContactId = x.ContactId,
                   CustomerId = x.CustomerId,
                   FirstName = x.FirstName?.Trim() ?? string.Empty,
                   LastName = x.LastName?.Trim() ?? string.Empty,
                   Email = x.Email?.Trim() ?? string.Empty,
                   WorkPhone = x.WorkPhone?.Trim() ?? string.Empty,
                   CellPhone = x.CellPhone?.Trim() ?? string.Empty,
                   Fax = x.Fax?.Trim() ?? string.Empty,
                   Notes = x.Notes?.Trim() ?? string.Empty,
               });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerDetails(int id = 0)
        {
            var viewmodel = GetCustomerViewModelFromDB(id);

            return PartialView("_CustomerDetails", viewmodel);
        }

        public int CreateCustomer()
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                Customer newCustomer = new Customer();
                db.Customers.Add(newCustomer);

                db.SaveChanges();

                return newCustomer.CustomerId;
            }
        }

        public CustomerViewModel GetCustomerViewModelFromDB(int id)
        {
            var viewmodel = new CustomerViewModel();

            using (var db = new JDsDBEntities())
            {
                ViewBag.Contacts = new SelectList((from c in db.Contacts
                                                   orderby c.LastName
                                                   where c.CustomerId == id
                                                   select new
                                                   {
                                                       ContactId = c.ContactId,
                                                       ContactName = c.FirstName + " " + c.LastName
                                                   }).ToList(), "ContactId", "ContactName");


                if (id == 0)
                {
                    // Create customer ID
                    var CustomerId = CreateCustomer();
                    viewmodel.CustomerId = CustomerId;
                    viewmodel.CompanyName = string.Empty;
                    viewmodel.Code = string.Empty;
                    viewmodel.Notes = string.Empty;
                    viewmodel.PrimaryContactId = null;
                    viewmodel.PrimaryContact = null;
                    viewmodel.BillingAddressId = null;
                    viewmodel.BillingAddressLine1 = string.Empty;
                    viewmodel.BillingAddressLine2 = string.Empty;
                    viewmodel.BillingCity = string.Empty;
                    viewmodel.BillingState = string.Empty;
                    viewmodel.BillingZip = string.Empty;
                    viewmodel.BillingCountry = string.Empty;
                    viewmodel.ShippingAddressId = null;
                    viewmodel.ShippingAddressLine1 = string.Empty;
                    viewmodel.ShippingAddressLine2 = string.Empty;
                    viewmodel.ShippingCity = string.Empty;
                    viewmodel.ShippingState = string.Empty;
                    viewmodel.ShippingZip = string.Empty;
                    viewmodel.ShippingCountry = string.Empty;
                }
                else
                {
                    // Existing customer
                    var customer = db.Customers.Include("Address").Include("Contact").FirstOrDefault(x => x.CustomerId == id);

                    if (customer == null)
                    {
                        _logger.Error("Customer details for CustomerId={0} not found.", id);
                    }
                    else
                    {
                        viewmodel.CustomerId = id;
                        viewmodel.CompanyName = customer.CompanyName;
                        viewmodel.Code = customer.Code;
                        viewmodel.Notes = customer.Notes;

                        // all ultimately futile since the primary contact's info is loaded afterwards
                        if (customer.Contact != null)
                        {
                            viewmodel.PrimaryContact = new ContactViewModel();
                            viewmodel.PrimaryContact.ContactId = customer.Contact.ContactId;
                            viewmodel.PrimaryContact.CustomerId = customer.CustomerId;
                            viewmodel.PrimaryContact.FirstName = "Yourmom";// customer.Contact.FirstName;
                            viewmodel.PrimaryContact.LastName = customer.Contact.LastName;
                            viewmodel.PrimaryContact.Email = customer.Contact.Email;
                            viewmodel.PrimaryContact.WorkPhone = customer.Contact.WorkPhone;
                            viewmodel.PrimaryContact.CellPhone = customer.Contact.CellPhone;
                            viewmodel.PrimaryContact.Fax = customer.Contact.Fax;

                            viewmodel.PrimaryContactId = customer.PrimaryContactId;
                        }

                        viewmodel.BillingAddressId = customer.BillingAddressId;
                        if (customer.Address != null)
                        {
                            viewmodel.BillingAddressLine1 = customer.Address.AddressLine1;
                            viewmodel.BillingAddressLine2 = customer.Address.AddressLine2;
                            viewmodel.BillingCity = customer.Address.City;
                            viewmodel.BillingState = customer.Address.State;
                            viewmodel.BillingZip = customer.Address.Zip;
                            viewmodel.BillingCountry = customer.Address.Country;
                        }

                        viewmodel.ShippingAddressId = customer.ShippingAddressId;
                        if (customer.ShippingAddressId.HasValue)
                        {
                            var address = db.Addresses.Find(customer.ShippingAddressId.Value);

                            if (address != null)
                            {
                                viewmodel.ShippingAddressLine1 = address.AddressLine1;
                                viewmodel.ShippingAddressLine2 = address.AddressLine2;
                                viewmodel.ShippingCity = address.City;
                                viewmodel.ShippingState = address.State;
                                viewmodel.ShippingZip = address.Zip;
                                viewmodel.ShippingCountry = address.Country;
                            }
                        }
                    }
                }
            }
            return viewmodel;
        }

        [HttpPost]
        public ActionResult SaveCustomer(CustomerViewModel model)
        {

            model.PrimaryContact = new ContactViewModel();

            using (var db = new JDsDBEntities())
            {
                ViewBag.Contacts = new SelectList((from c in db.Contacts
                                                   orderby c.LastName
                                                   where c.CustomerId == model.CustomerId
                                                   select new
                                                   {
                                                       ContactId = c.ContactId,
                                                       ContactName = c.FirstName + " " + c.LastName
                                                   }).ToList(), "ContactId", "ContactName");


                // Server side validation
                if (!ModelState.IsValid || string.IsNullOrEmpty(model.CompanyName.Trim()))
                {
                    ModelState.AddModelError("SaveCustomerError", "Customer name required.");

                    return Json(new { IsSuccessful = false, View = this.RenderRazorViewToString("_CustomerDetails", model )});
                }

                // Create new or edit existing
                var customer = model.CustomerId == 0 ? new Customer() : db.Customers.Find(model.CustomerId);

                if (customer == null)
                {
                    _logger.Error("Unable to update CustomerId={0} '{1}' because the customer record was not found or has been deleted.", model.CustomerId, model.CompanyName);                    
                    return CustomerDetails(0);
                }

                customer.CompanyName = model.CompanyName;
                customer.Code = model.Code;
                customer.Notes = model.Notes;
                customer.PrimaryContactId = null;
                customer.IsActive = true;
                if (model.PrimaryContactId != 0)
                    customer.PrimaryContactId = model.PrimaryContactId;

                Contact dbcontact = model.PrimaryContactId == null ? null : db.Contacts.Find(model.PrimaryContactId);

                if (dbcontact == null && model.PrimaryContactId != null &&
                    (!string.IsNullOrWhiteSpace(model.PrimaryContact.FirstName) || !string.IsNullOrWhiteSpace(model.PrimaryContact.LastName)))
                {
                    dbcontact = new Contact();
                    dbcontact.IsActive = true;
                }
                
                Address dbbillingaddress = model.BillingAddressId.HasValue ? db.Addresses.Find(model.BillingAddressId.Value) : null;

                if (dbbillingaddress == null &&
                    (!string.IsNullOrWhiteSpace(model.BillingAddressLine1) ||
                     !string.IsNullOrWhiteSpace(model.BillingCity) ||
                     !string.IsNullOrWhiteSpace(model.BillingZip)))
                {
                    dbbillingaddress = new Address();
                }

                if (dbbillingaddress != null)
                {
                    dbbillingaddress.AddressLine1 = model.BillingAddressLine1;
                    dbbillingaddress.AddressLine2 = model.BillingAddressLine2;
                    dbbillingaddress.City = model.BillingCity;
                    dbbillingaddress.State = model.BillingState;
                    dbbillingaddress.Zip = model.BillingZip;
                    dbbillingaddress.Country = model.BillingCountry;
                    customer.Address = dbbillingaddress;

                    if (dbbillingaddress.AddressId == 0)
                    {
                        db.Addresses.Add(dbbillingaddress);
                    }
                    else
                    {
                        var entry = db.Entry<Address>(dbbillingaddress);
                        entry.State = EntityState.Modified;
                    }

                }


                Address dbshippingaddress = model.ShippingAddressId.HasValue ? db.Addresses.Find(model.ShippingAddressId.Value) : null;

                // For now the shipping address will always gets its own record...
                if (model.ShippingAddressId.HasValue && model.BillingAddressId.HasValue &&
                    model.ShippingAddressId.Value == model.BillingAddressId.Value)
                {
                    dbshippingaddress = new Address();
                }

                if (dbshippingaddress == null &&
                    (!string.IsNullOrWhiteSpace(model.ShippingAddressLine1) ||
                     !string.IsNullOrWhiteSpace(model.ShippingCity) ||
                     !string.IsNullOrWhiteSpace(model.ShippingZip)))
                {
                    dbshippingaddress = new Address();
                }

                if (dbshippingaddress != null)
                {
                    dbshippingaddress.AddressLine1 = model.ShippingAddressLine1;
                    dbshippingaddress.AddressLine2 = model.ShippingAddressLine2;
                    dbshippingaddress.City = model.ShippingCity;
                    dbshippingaddress.State = model.ShippingState;
                    dbshippingaddress.Zip = model.ShippingZip;
                    dbshippingaddress.Country = model.ShippingCountry;

                    if (dbshippingaddress.AddressId == 0)
                    {
                        db.Addresses.Add(dbshippingaddress);
                    }
                    else
                    {
                        var entry = db.Entry<Address>(dbshippingaddress);
                        entry.State = EntityState.Modified;
                    }
                }

                if (customer.CustomerId == 0)
                {
                    db.Customers.Add(customer);
                }
                else
                {
                    var entry = db.Entry<Customer>(customer);
                    entry.State = EntityState.Modified;
                }

                if (db.ChangeTracker.HasChanges())
                {
                    db.SaveChanges();
                }

                // since the shipping address is not linked via a FK..
                // TODO: well, let's link it!
                if (dbshippingaddress != null && dbshippingaddress.AddressId != customer.ShippingAddressId)
                {
                    customer.ShippingAddressId = dbshippingaddress.AddressId;
                    db.SaveChanges();
                }

                ModelState.Clear();
                model = GetCustomerViewModelFromDB(customer.CustomerId);

                return Json(new { IsSuccessful = true, View = this.RenderRazorViewToString("_CustomerDetails", model )});
            }
        }

        [HttpPost]
        public ActionResult SaveContact(ContactViewModel model, bool jsonResponce = false)
        {
            // Server side validation
            if (!ModelState.IsValid)
            {
                if (jsonResponce)
                {
                    return Json(new { result = false, message = "Please enter a valid email address", partialView = this.RenderRazorViewToString("_ContactDetails", model) }, JsonRequestBehavior.AllowGet);
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, PartialView("_ContactDetails", model).ToString());
                }

                    
                else
                    return PartialView("_ContactDetails", model);
            }

            var db = new JDsDBEntities();

            var customer = db.Customers.Find(model.CustomerId);
            if (customer == null)
            {
                _logger.Error("SaveContact failed unable to find CustomerId={0}.", model.CustomerId);

                if (jsonResponce)
                    return Json(new { result = false, message = "Error Saving Contact" }, JsonRequestBehavior.AllowGet);
                else
                    return PartialView("_ContactDetails", model);
            }

            // Create new or edit existing
            var contact = model.ContactId == 0 ? new Contact() : db.Contacts.Find(model.ContactId);

            if (contact == null)
            {
                _logger.Error("Unable to update ContactId={0} '{1} {2}' because the contact record was not found or has been deleted.", model.ContactId, model.FirstName, model.LastName);
                               
                if (jsonResponce)
                    return Json(new { result = false, message = "Error Saving Contact" }, JsonRequestBehavior.AllowGet);
                else
                    return ContactDetails(0);
            }

            contact.CustomerId = model.CustomerId;
            contact.FirstName = string.IsNullOrWhiteSpace(model.FirstName) ? string.Empty : model.FirstName.Trim();
            contact.LastName = string.IsNullOrWhiteSpace(model.LastName) ? string.Empty : model.LastName.Trim();
            contact.Email = string.IsNullOrWhiteSpace(model.Email) ? string.Empty : model.Email.Trim();
            contact.WorkPhone = string.IsNullOrWhiteSpace(model.WorkPhone) ? string.Empty : model.WorkPhone.Trim();
            contact.CellPhone = string.IsNullOrWhiteSpace(model.CellPhone) ? string.Empty : model.CellPhone.Trim();
            contact.Fax = string.IsNullOrWhiteSpace(model.Fax) ? string.Empty : model.Fax.Trim();
            contact.Notes = string.IsNullOrWhiteSpace(model.Notes) ? string.Empty : model.Notes.Trim();
            contact.IsActive = true;

            if (model.ContactId == 0)
            {
                db.Contacts.Add(contact);
            }
            else
            {
                var entry = db.Entry<Contact>(contact);
                entry.State = EntityState.Modified;
            }

            if (db.ChangeTracker.HasChanges())
            {
                db.SaveChanges();
            }
            
            if (jsonResponce)
                return Json(new { result = true, message = "Contact Saved" }, JsonRequestBehavior.AllowGet);
            else
                return ContactDetails(contact.ContactId);
        }

        [HttpPost]
        public ActionResult DeleteCustomer(int id)
        {
            var db = new JDsDBEntities();
            var customer = db.Customers.Find(id);

            if (customer == null)
            {
                string error = string.Format("Delete employee for CustomerId={0} not found.", id);
                _logger.Error(error);                
                return Content(error);
            }
            else
            {
                _logger.Trace("CustomerId={0} '{1}' marked as inactive", customer.CustomerId, customer.CompanyName);
                customer.IsActive = false;
                var entry = db.Entry<Customer>(customer);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
            
            return Content("");
        }

        [HttpPost]
        public ActionResult DeleteContact(int id)
        {
            var db = new JDsDBEntities();
            var contact = db.Contacts.Find(id);

            if (contact == null)
            {
                string error = string.Format("Delete employee for ContactId={0} not found.", id);
                _logger.Error(error);
                return Content(error);
            }
            else
            {
                _logger.Trace("ContactId={0} '{1} {2}' marked as inactive", contact.ContactId, contact.FirstName, contact.LastName);
                contact.IsActive = false;
                var entry = db.Entry<Contact>(contact);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
            
            return Content("");
        }

        [HttpGet]
        public ActionResult GetCustomersRefreshDropdown()
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                SelectList customers = new SelectList(db.Customers.Where(c => c.IsActive == true).OrderBy(s => s.CompanyName), "CustomerId", "CompanyName");

                var json = "";

                foreach(var c in customers)
                {
                    json += "<option value=\""+c.Value+"\">"+c.Text+"</option>";
                }

                return Json(json, JsonRequestBehavior.AllowGet);
            }               
        }

        [HttpGet]
        public ActionResult GetContactsRefreshDropdown(int CustomerId)
        {
            using (JDsDBEntities db = new JDsDBEntities())
            {
                var contacts = db.Contacts.Where(x => x.CustomerId == CustomerId && x.IsActive == true).OrderBy(s => s.FirstName);
                var json = "";

                foreach (var c in contacts)
                {
                    json += "<option value=\"" + c.ContactId + "\">" + c.FirstName + " " + c.LastName + "</option>";
                }

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Gets the list of contacts for the customer details page to populate the
        /// list of contacts used to select the primary contact
        /// </summary>
        [HttpGet]
        public ActionResult GetContactsForDropdown(int id = 0)
        {
            var db = new JDsDBEntities();
            var contacts = db.Contacts
                .Where(x => x.CustomerId == id && x.IsActive == true)
                .Select(x =>
                  new
                  {
                      x.ContactId,
                      x.CustomerId,
                      Name = x.FirstName + " " + x.LastName
                  }).OrderBy(x => x.Name).ToList();

            if (contacts.Count == 0)
            {
                contacts.Add(new
                {
                    ContactId = 0,
                    CustomerId = id,
                    Name = "None"
                });
            }

            return Json(contacts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContactMgmt(int customerId = 0)
        {
            var db = new JDsDBEntities();
            var customer = db.Customers.Find(customerId);

            if (customer == null)
            {
                _logger.Error("ContactMgmt: Customer record for CustomerId={0} not found.", customerId);
            }

            return PartialView("_ContactMgmt", customerId);
        }

        public ActionResult ContactDetails(int id = 0)
        {
            var db = new JDsDBEntities();
            var viewmodel = new ContactViewModel();

            if (id == 0)
            {
                viewmodel.ContactId = 0;
                viewmodel.CustomerId = 0;
                viewmodel.FirstName = string.Empty;
                viewmodel.LastName = string.Empty;
                viewmodel.Email = string.Empty;
                viewmodel.WorkPhone = string.Empty;
                viewmodel.CellPhone = string.Empty;
                viewmodel.Fax = string.Empty;
                viewmodel.Notes = string.Empty;
            }
            else
            {
                // Existing Contact
                var contact = db.Contacts.Find(id);

                if (contact == null)
                {
                    _logger.Error("Contact details for ContactId={0} not found.", id);
                }
                else
                {
                    viewmodel.ContactId = id;
                    viewmodel.CustomerId = contact.CustomerId;
                    viewmodel.FirstName = contact.FirstName;
                    viewmodel.LastName = contact.LastName;
                    viewmodel.Email = contact.Email;
                    viewmodel.WorkPhone = contact.WorkPhone;
                    viewmodel.CellPhone = contact.CellPhone;
                    viewmodel.Fax = contact.Fax;
                    viewmodel.Notes = contact.Notes;
                }
            }

            return PartialView("_ContactDetails", viewmodel);
        }

        public ActionResult PrimaryContactDetails(int id = 0)
        {
            var db = new JDsDBEntities();
            var viewmodel = new ContactViewModel();

            if (id == 0)
            {
                viewmodel.ContactId = 0;
                viewmodel.CustomerId = 0;
                viewmodel.FirstName = string.Empty;
                viewmodel.LastName = string.Empty;
                viewmodel.Email = string.Empty;
                viewmodel.WorkPhone = string.Empty;
                viewmodel.CellPhone = string.Empty;
                viewmodel.Fax = string.Empty;
                viewmodel.Notes = string.Empty;
            }
            else
            {
                // Existing Contact
                var contact = db.Contacts.Find(id);

                if (contact == null)
                {
                    _logger.Error("Primary contact details for ContactId={0} not found.", id);
                }
                else
                {
                    viewmodel.ContactId = id;
                    viewmodel.CustomerId = contact.CustomerId;
                    viewmodel.FirstName = contact.FirstName;
                    viewmodel.LastName = contact.LastName;
                    viewmodel.Email = contact.Email;
                    viewmodel.WorkPhone = contact.WorkPhone;
                    viewmodel.CellPhone = contact.CellPhone;
                    viewmodel.Fax = contact.Fax;
                    viewmodel.Notes = contact.Notes;
                }
            }

            return PartialView("_PrimaryContactDetails", viewmodel);
        }
    }
}