using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class EmailTemplateController : Controller
    {
        private readonly IEMAILTEMPLATEService _emailtemplateservice;
        public EmailTemplateController(IEMAILTEMPLATEService emailtemplateservice)
        {
            _emailtemplateservice = emailtemplateservice;
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllTemplates()
        {
            var emailtemplates = await _emailtemplateservice.GetAllAsync();
            return View(emailtemplates);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> EditTemplate(string id)
        {
            var emailtemplate = await _emailtemplateservice.GetByIdAsync(id);
            var vm = new EditTemplateVM() 
            {
                TemplateId=id,
                Name = emailtemplate.Name,
                Content = emailtemplate.Content,
                NeedToLogin=emailtemplate.NeedToLogin,
            };
            return View(vm);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> EditTemplate (EditTemplateVM editvm)
        {
            if (ModelState.IsValid)
            {
                var newtemplate = new EMAILTEMPLATE()
                {
                    Name = editvm.Name,
                    NeedToLogin = editvm.NeedToLogin,
                    Content = editvm.Content,
                    Id = editvm.TemplateId
                };
                await _emailtemplateservice.UpdateAsync(editvm.TemplateId, newtemplate);
                var successMessage = "You successfully edited the content of template " + editvm.Name;
                return RedirectToAction("IndexParam", "Home", new { successMessage });
            }
            else
            {
                return View(editvm);
            }
        }
    }
}
