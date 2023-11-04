﻿using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInInput signInInput)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            var response = await _identityService.SignIn(signInInput);
            if (response.IsSuccessful)
            {
                response.Errors.ForEach(x =>
                {
                    ModelState.AddModelError(String.Empty, x);
                });

            }

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
