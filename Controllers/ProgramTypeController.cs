using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class ProgramTypeController : Controller
    {
        private readonly IPROGRAMTYPEService _programTypeService;
        public ProgramTypeController(IPROGRAMTYPEService programTypeService)
        {
            _programTypeService = programTypeService;
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddProgramType(string id)
        {
            var vm = new ProgramTypeVM();
            if (!string.IsNullOrEmpty(id))
            {
                var ent = await _programTypeService.GetByIdAsync(id);
                vm.Id = ent.Id;
                vm.InterestRate = ent.InterestRate;
                vm.InterestRateDelay = ent.InterestRateDelay;
                vm.InterestRateInstallment = ent.InterestRateInstallment;
                vm.Currency = ent.Currency;
                vm.Type = ent.Type;
            }
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProgramType(ProgramTypeVM vm)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(vm.Type))
            {
                var programtype = new PROGRAMTYPE()
                {
                    Id = vm.Id,
                    Type = vm.Type,
                    Currency = vm.Currency,
                    InterestRate = vm.InterestRate,
                    InterestRateDelay = vm.InterestRateDelay,
                    InterestRateInstallment = vm.InterestRateInstallment
                };
                if (!string.IsNullOrEmpty(vm.Id))
                {
                    await _programTypeService.UpdateAsync(vm.Id, programtype);
                }
                else
                {
                    await _programTypeService.AddAsync(programtype);
                }
                var successMessage = !string.IsNullOrEmpty(vm.Id) ? "Program Güncellendi" : "Başarıyla yeni bir program türü eklediniz";
                return RedirectToAction("IndexParam", "Home", new { successMessage });
            }
            else
            {
                ModelState.AddModelError("Type", "Program Adı Giriniz");
                return View(vm);
            }
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllProgramTypes ()
        {
            var programtypes= await _programTypeService.GetAllAsync();
            return View(programtypes);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteProgramType(string id)
        {
            await _programTypeService.DeleteAsync(id);
            var successMessage = "Program türünü başarıyla sildiniz";
            return RedirectToAction("IndexParam","Home",new {successMessage});
        }

    }
}
