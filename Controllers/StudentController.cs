using Debt_Calculation_And_Repayment_System.Data.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class StudentController : Controller
    {
        private readonly ISTUDENTService _studentService;
        public StudentController(ISTUDENTService studentService)
        {
            _studentService = studentService;
        }
    }
}
