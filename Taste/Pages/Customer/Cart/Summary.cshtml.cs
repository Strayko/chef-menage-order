using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Models.ViewModels;

namespace Taste.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderDetailsCart DetailCart { get; set; }

        public IActionResult OnGet()
        {
            DetailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            DetailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                DetailCart.ListCart = cart.ToList();
            }

            foreach (var cartList in DetailCart.ListCart)
            {
                cartList.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == cartList.MenuItemId);
                DetailCart.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
            }

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);
            DetailCart.OrderHeader.PickUpName = applicationUser.FullName;
            DetailCart.OrderHeader.PickUpTime = DateTime.Now;
            DetailCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            return Page();
        }
    }
}