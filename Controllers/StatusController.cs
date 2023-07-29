using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class StatusController : Controller
    {
        private readonly ISTUDENTSTATUSTYPEService _statusService;
        public StatusController(ISTUDENTSTATUSTYPEService statusService)
        {
            _statusService = statusService;
        }
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> AllStatuses()
        {
            var statuses = await _statusService.GetAllAsync();
            return View(statuses);
        }
        [Authorize(Roles = "Admin, StaffMember")]
        public async Task<IActionResult> DeleteStatus(string id)
        {
            bool authorize;
            var status = await _statusService.GetByIdAsync(id);
            var type = status.Type;
            var unauthorizedStatusToDelete = new List<string>() { "Giriş Yapıldı", "Bildirim Gönderildi", "Talep Aşamasında", "Yeni Kayıt", "Borcu Girildi", "Muhasebede","Tamamlandı" };
            //"Logged in", "Notified", "Waiting", "New Recorded", "In Accounting"
            authorize = !unauthorizedStatusToDelete.Contains(type);
            if (authorize)
            {
                var vm = new DeleteStatusVM() { Id = id, Type = type };
                return View(vm);
            }
            else
            {
                var errorMessage = "Silinemez bir durumu silmeye çalıştınız";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }
        [Authorize(Roles = "Admin, StaffMember")]
        [HttpPost]
        public async Task<IActionResult> DeleteStatus(DeleteStatusVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Delete)
                {
                    await _statusService.DeleteAsync(vm.Id);
                    var successMessage = "Silme işlemi başarılı";
                    return RedirectToAction("IndexParam", "Home", new { successMessage });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                var status = await _statusService.GetByIdAsync(vm.Id);
                var newvm = new DeleteStatusVM() { Id = vm.Id, Type=status.Type };
                return View(newvm);
            }
        }
        [Authorize(Roles = "Admin, StaffMember")]
        public IActionResult CreateStatus()
        {
            var vm = new CreateStatusVM();
            return View(vm);
        }
        [Authorize(Roles = "Admin, StaffMember")]
        [HttpPost]
        public async Task<IActionResult> CreateStatus(CreateStatusVM vm)
        {
            if (ModelState.IsValid)
            {
                var allstatuses = await _statusService.GetAllAsync();
                if (!allstatuses.Select(s => s.Type.ToLower()).ToList().Contains(vm.Type.ToLower()))
                {
                    var Type = vm.Type;
                    await _statusService.AddAsync(new STUDENTSTATUSTYPE() { Type = Type });
                    var successMessage = "Ekleme işlemi başarılı " + Type;
                    return RedirectToAction("IndexParam", "Home", new { successMessage });
                }
                else
                {
                    var errorMessage = "Durum " + vm.Type + " zaten ekli";
                    return RedirectToAction("Error", "Home", new { errorMessage });
                }
            }
            else
            {
                return View(vm);
            }
        }
    }
}

