using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JalaramTravels.ViewModel
{
    public class LoginVM
    {
        public string ReturnUrl { get; set; }
        public int LoginID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string FirstName{ get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Int32? CreateUser { get; set; }
        public Int32? UpdateUser { get; set; }
        public string Flag { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
    public class LoginResult
    {
        public string Url { get; set; }

        public bool IsError { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class ChangePasswordVM
    {
        public int LoginID { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}