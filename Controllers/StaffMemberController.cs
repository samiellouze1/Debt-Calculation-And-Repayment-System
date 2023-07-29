using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class StaffMemberController : Controller
    {
        private readonly ISTAFFMEMBERService _staffmemberService;
        private readonly ISTUDENTService _studentService;
        private readonly IDEBTREGISTERService _deptRegisterService;
        private readonly IUSERService _userService;
        public StaffMemberController(ISTAFFMEMBERService staffmemberService, ISTUDENTService studentService, IDEBTREGISTERService deptRegisterService, IUSERService userService)
        {
            _staffmemberService = staffmemberService;
            _studentService = studentService;
            _deptRegisterService = deptRegisterService;
            _userService = userService;
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AllStaffMembers ()
        {
            var staffmembers = await _staffmemberService.GetAllAsync(sm => sm.Students);
            return View("StaffMembers", staffmembers);
        }
        [Authorize(Roles ="StaffMember, Admin")]
        public async Task<IActionResult> MyProfile(int type)
        {
            var lstStudents = new List<STUDENT>();
            var id = User.FindFirstValue("Id");
            var user = await _userService.GetByIdAsync(id);
            
            if (User.IsInRole("StaffMember"))
            {
                var staffMember = await _staffmemberService.GetByIdAsync(id, s => s.Students);
                
                lstStudents = staffMember.Students;
            }
            if (User.IsInRole("Admin"))
            {
                var students = (await _studentService.GetAllAsync()).ToList();
                lstStudents = students;
            }
            


            //var borcHesabiYapilacakDurumlar = new List<string>() { "Yeni Kayıt" };
            var BorcHesabiYapilacak = new List<STUDENT>();// staff.Students.Where(k => borcHesabiYapilacakDurumlar.Contains(k.Status)).ToList();
            var IlkTaksitOdemesiYapan = new List<STUDENT>();
            var TaksikOdemesiniGeciren = new List<STUDENT>();
            var BorcuBiten = new List<STUDENT>();
            var GeriOdemeKarariIptalEdilen = lstStudents.Where(k => k.Status== "İade Kararı İptal Edildi").ToList();
            var GeriOdemeSureciDurdurulan = lstStudents.Where(k => k.Status == "İade İşlemi Askıya Alındı").ToList();
            var HukugaSevk = lstStudents.Where(k => k.Status == "Hukuk Hizmetleri Başkanlığı'na sevk edildi").ToList();

           


            foreach (var s in lstStudents) //Where(k=> !BorcHesabiYapilacak.Any(a=>a.Id==k.Id)
            {
                var stu = await _studentService.GetByIdAsync(s.Id, x => x.DebtRegister,x=>x.ProgramType);
                var dept = await _deptRegisterService.GetByIdAsync(stu.DebtRegister.Id,a=>a.Payments);
                if (dept != null)
                {
                    if (dept.Payments==null || dept.Payments.Count == 0)
                    {
                        BorcHesabiYapilacak.Add(s);
                    }
                    if (dept.Payments.Any(k => k.Paid && k.Type== "Taksit") && dept.Payments.Any(k=>!k.Paid && k.Type == "Taksit"))
                    {
                        IlkTaksitOdemesiYapan.Add(s);
                    }
                    if (dept.Payments.Any(k => !k.Paid   && k.PaymentDate<DateTime.Now))
                    {
                        TaksikOdemesiniGeciren.Add(s);
                    }
                    if (dept.Payments.Count>1 && dept.Payments.All(k => k.Paid))
                    {
                        BorcuBiten.Add(s);
                    }
                }
            }
            //var BorcHesabiYapilacak = staff.Students.Where(k => borcHesabiYapilacDurumlar.Contains(k.Status));
            if (type== (int)FilterTypeStudendt.BorcHesabiYapilacak)
            {
                lstStudents = BorcHesabiYapilacak.ToList();
                ViewBag.FilterName = "Borç Hesaplaması yapılacak Bursiyerler";
            }
            else if (type == (int)FilterTypeStudendt.IlkTaksitOdemesiYapan)
            {
                lstStudents = IlkTaksitOdemesiYapan.ToList();
                ViewBag.FilterName = "İlk taksit ödemesini yapan bursiyerler";
            }
            else if (type == (int)FilterTypeStudendt.TaksikOdemesiniGeciren)
            {
                lstStudents = TaksikOdemesiniGeciren.ToList();
                ViewBag.FilterName = "Taksit ödemesini geçiren bursiyerler";
            }
            else if (type == (int)FilterTypeStudendt.BorcuBiten)
            {
                lstStudents = BorcuBiten.ToList();
                ViewBag.FilterName = "Borcu biten bursiyerler";
            }
            else if (type == (int)FilterTypeStudendt.GeriOdemeKarariIptalEdilen)
            {
                lstStudents = GeriOdemeKarariIptalEdilen.ToList();
                ViewBag.FilterName = "Geri ödeme kararı iptal edilen bursiyerler";
            }
            else if (type == (int)FilterTypeStudendt.GeriOdemeSureciDurdurulan)
            {
                lstStudents = GeriOdemeSureciDurdurulan.ToList();
                ViewBag.FilterName = "Geri ödeme süreci durdurulan bursiyerler";
            }
            else if (type == (int)FilterTypeStudendt.HukugaSevk)
            {
                lstStudents = HukugaSevk.ToList();
                ViewBag.FilterName = "Hukuk Hizmetleri Başkanlığı'na sevk edilen bursiyerler";
            }
            else
            {
                ViewBag.FilterName = "Bursiyerlerim";
            }
            ViewBag.Ogrenciler = lstStudents;
            ViewBag.BorcHesabiYapilacak = BorcHesabiYapilacak;
            ViewBag.IlkTaksitOdemesiYapan = IlkTaksitOdemesiYapan;
            ViewBag.TaksikOdemesiniGeciren = TaksikOdemesiniGeciren;
            ViewBag.BorcuBiten = BorcuBiten;
            ViewBag.GeriOdemeKarariIptalEdilen = GeriOdemeKarariIptalEdilen;
            ViewBag.GeriOdemeSureciDurdurulan = GeriOdemeSureciDurdurulan;
            ViewBag.HukugaSevk = HukugaSevk;
            return View("StaffMember", user);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> StaffMemberById(string id)
        {
            var staffmember = await _staffmemberService.GetByIdAsync(id, sm => sm.Students);
            return View("StaffMember", staffmember);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> StaffMemberByStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id, s => s.StaffMember, s => s.DebtRegister);
            var staffmember = student.StaffMember;
            return View("StaffMember", staffmember);
        }
        public enum FilterTypeStudendt
        {
            BorcHesabiYapilacak=1,
            IlkTaksitOdemesiYapan=2,
            TaksikOdemesiniGeciren=3,
            BorcuBiten=4,
            GeriOdemeKarariIptalEdilen=5,
            GeriOdemeSureciDurdurulan=6,
            HukugaSevk=7
        }
    }
}
