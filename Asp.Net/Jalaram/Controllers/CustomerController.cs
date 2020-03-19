using JalaramTravels.Filters;
using JalaramTravels.Models;
using JalaramTravels.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JalaramTravels.Controllers
{
    [CheckSessionTimeOut]
    public class CustomerController : Controller
    {
        JalaramDBEntities db = new JalaramDBEntities();

        public async Task<ActionResult> Index()
        {
            var cityList = db.Cities.ToList().OrderBy(m => m.CityName);
            var CustomersList = await Task.Run(() => db.Customers.ToList());
            List<CustomerVM> CustomersListVMList = new List<CustomerVM>();

            foreach (var item in CustomersList)
            {
                CustomerVM CustVM = new CustomerVM();
                CustVM.CustomerID = item.CustomerID;
                CustVM.CustomerName = item.CustomerName;
                CustVM.CustomerNumber = item.CustomerNumber;

                if (item.CustomerID !=0)
                {
                    var city = cityList.FirstOrDefault(t => t.CityID == item.CustomerCityID);
                    if (city !=null)
                    {
                        CustVM.CustomerCityName = city.CityName;
                    }
                }
                CustomersListVMList.Add(CustVM);
            }

            return View(CustomersListVMList);
        }

        public async Task<ActionResult> Create(int Id = 0)
        {
            CustomerVM CustomerVM;
            if (Id == 0)
            {
                CustomerVM = new CustomerVM();
                var cityActiveList = db.Cities.ToList().Where(m => m.Flag.Equals("A")).OrderBy(m => m.CityName);
                CustomerVM.CityList = cityActiveList.Select(s => new SelectListItem() { Text = s.CityName, Value = s.CityID.ToString() });
            }
            else
            {
                CustomerVM = new CustomerVM();
                Customer Customer = new Customer();
                var cityActiveList = db.Cities.ToList().OrderBy(m => m.CityName);
                CustomerVM.CityList = cityActiveList.Select(s => new SelectListItem() { Text = s.CityName, Value = s.CityID.ToString() });

                Customer = await Task.Run(() => db.Customers.Find(Id));

                if (Customer != null)
                {
                    CustomerVM.CustomerID = Customer.CustomerID;
                    CustomerVM.CustomerName = Customer.CustomerName;
                    CustomerVM.CustomerNumber = Customer.CustomerNumber;
                    CustomerVM.CustomerCityID = (Int64)Customer.CustomerCityID;
                }
            }
            return View(CustomerVM);
        }

        [HttpPost]
        public async Task<JsonResult> Create(CustomerVM CustomerVM)
        {
            var loginResult = new LoginResult();
            loginResult.IsError = false;
            loginResult.ErrorMessage = "";
            var ParcelList = await Task.Run(() => db.Customers.ToList());
            var exits = ParcelList.FirstOrDefault(t => t.CustomerName == CustomerVM.CustomerName && t.CustomerNumber == CustomerVM.CustomerNumber && t.CustomerCityID==CustomerVM.CustomerCityID);
            if (exits != null)
            {
                loginResult.IsError = true;
                loginResult.ErrorMessage = "Data already exists";
            }
            else
            {
                Customer obj = new Customer();

                if (CustomerVM.CustomerID == 0)
                {
                    obj.CustomerName = CustomerVM.CustomerName;
                    obj.CustomerNumber = CustomerVM.CustomerNumber;
                    obj.CustomerCityID = CustomerVM.CustomerCityID;
                    await Task.Run(() => db.Customers.Add(obj));
                    await Task.Run(() => db.SaveChanges());
                }
                else
                {
                    obj = await Task.Run(() => db.Customers.Find(CustomerVM.CustomerID));
                    if (obj != null)
                    {
                        obj.CustomerID = CustomerVM.CustomerID;
                        obj.CustomerName = CustomerVM.CustomerName;
                        obj.CustomerNumber = CustomerVM.CustomerNumber;
                        obj.CustomerCityID = CustomerVM.CustomerCityID;
                        await Task.Run(() => db.Entry(obj).State = System.Data.Entity.EntityState.Modified);
                        await Task.Run(() => db.SaveChanges());

                    }
                }
            }
            return Json(loginResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int Id)
        {
            Customer Obj = await Task.Run(() => db.Customers.Find(Id));

            if (Obj != null)
            {
                await Task.Run(() => db.Customers.Remove(Obj));
                await Task.Run(() => db.SaveChanges());
            }
            return Json(Obj, JsonRequestBehavior.AllowGet);
        }
    }
}