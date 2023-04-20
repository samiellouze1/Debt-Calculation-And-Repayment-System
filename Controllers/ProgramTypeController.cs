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
        public IActionResult AddProgramType()
        {
            var vm = new ProgramTypeVM();
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProgramType(ProgramTypeVM vm)
        {
            if (ModelState.IsValid)
            {
                var programtype = new PROGRAMTYPE()
                {
                    Type = vm.Type,
                };
                await _programTypeService.AddAsync(programtype);
                var successMessage = "You successfully added a new programtype";
                return RedirectToAction("IndexParam", "Home", new { successMessage });
            }
            else
            {
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
            var successMessage = "you successfully deleted a program type";
            return RedirectToAction("AllProgramTypes");
        }

    }
}
