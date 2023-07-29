using Debt_Calculation_And_Repayment_System.Data.IServices;
using Debt_Calculation_And_Repayment_System.Data.Services;
using Debt_Calculation_And_Repayment_System.Data.ViewModels;
using Debt_Calculation_And_Repayment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Debt_Calculation_And_Repayment_System.Controllers
{
    public class BaseController : Controller
    {
        public static decimal[] DivideDecimalIntoEqualParts(decimal x, int n)
        {
            try
            {
                decimal[] parts = new decimal[n];
                decimal commonPart = Math.Round(Math.Floor(x / n * 100) / 100, 0, MidpointRounding.AwayFromZero);
                decimal remaining = x - commonPart * (n - 1);
                for (int i = 0; i < n - 1; i++)
                {
                    parts[i] = commonPart;
                }
                parts[n - 1] = remaining;
                return parts;
            }
            catch
            {
                return Enumerable.Repeat(0m, n).ToArray();
            }
        }
        public static PreviewRequestVM CalculateRequest(CreateRequestVM vm, DEBTREGISTER debtregister)
        {
            var ilkOdemeTarihi = new DateTime(vm.FirstInstallmentDate.Value.Year, vm.FirstInstallmentDate.Value.Month, vm.FirstInstallmentDate.Value.Day);
            var amounttopay = debtregister.Amount;
            
            var anaparaOrani=debtregister.Amount/(debtregister.Amount+debtregister.InterestAmount);
            var anaParadanDusulecekTutar =Math.Round(vm.ToBePaidFull * anaparaOrani,2,MidpointRounding.AwayFromZero);
            var faizdenOdenenPesinat = vm.ToBePaidFull - anaParadanDusulecekTutar;
            var resttopayinstallment = vm.ToBePaidInstallment - (debtregister.InterestAmount- faizdenOdenenPesinat);
            //var interest = Math.Round(resttopayinstallment / vm.NumOfMonths, 0, MidpointRounding.AwayFromZero);
            var iatable = DivideDecimalIntoEqualParts(resttopayinstallment, vm.NumOfMonths);
            var faiztable=new decimal[vm.NumOfMonths]  ;
            decimal interestamountSum = 0m;
            decimal interestamount = 0m;
            var whatstays = debtregister.Total - vm.ToBePaidFull;
            decimal tobepaideachmonth;
            //decimal faiz = 0m; 
            var payments = new List<PAYMENT>();
            if (vm.ToBePaidFull < debtregister.Total)
            {
                whatstays = 0;
                int birAyOtele= vm.ToBePaidFull>0 ? 1: 0;
                for (int i = 1; i <= vm.NumOfMonths; i++)
                {
                    
                    
                    int nod;
                    if (i > 1)
                    {
                        nod = (ilkOdemeTarihi.AddMonths(i+ birAyOtele) - ilkOdemeTarihi.AddMonths(i+ birAyOtele - 1)).Days;
                    }
                    else
                    {
                        nod = (ilkOdemeTarihi.AddMonths(i+ birAyOtele) - debtregister.ProgramFinishDate).Days;
                    }
                    if (anaParadanDusulecekTutar == 0)
                    {
                        resttopayinstallment -= iatable[i-1 ];
                    }
                    var hesaplanan = Math.Round(nod * resttopayinstallment * debtregister.InterestRateInstallment / 365, 2, MidpointRounding.AwayFromZero);
                    interestamount += hesaplanan;

                    if (anaParadanDusulecekTutar > 0)
                    {
                        resttopayinstallment -= iatable[i - 1 ];
                    }
                    var payment = new PAYMENT()
                    {
                        Type = "Taksit",
                        
                        Sum = 0,
                        
                        PaymentDate = ilkOdemeTarihi.AddMonths(i-1),
                        PrincipalAmount = iatable[i - 1 ],
                        InterestAmount = 0,
                    };
                    payments.Add(payment);



                }
                try
                {
                    var anapara = Math.Round(((amounttopay- anaParadanDusulecekTutar) / vm.NumOfMonths), 0, MidpointRounding.AwayFromZero);
                    //faiz = Math.Round((interestamount + debtregister.InterestAmount - faizdenOdenenPesinat) / vm.NumOfMonths, 0, MidpointRounding.AwayFromZero);
                    interestamountSum = Math.Round(interestamount + debtregister.InterestAmount - faizdenOdenenPesinat, 2, MidpointRounding.AwayFromZero);
                    faiztable=DivideDecimalIntoEqualParts(interestamountSum, vm.NumOfMonths);
                    tobepaideachmonth = anapara + faiztable[0];
                }
                catch
                {
                    tobepaideachmonth = 0m;
                }
            }
            else
            {
                try
                {
                    tobepaideachmonth = Math.Round(whatstays / vm.NumOfMonths, 0, MidpointRounding.AwayFromZero);
                }
                catch
                {
                    tobepaideachmonth = 0m;
                }
            }
            int faizC = 0;
            foreach(var p in payments)
            {
                p.Sum = faiztable[faizC] +p.PrincipalAmount;
                p.InterestAmount = faiztable[faizC];
                faizC++;
            }
            if (vm.ToBePaidFull != 0)
            {
                var paymentfull = new PAYMENT() { Type = "Peşin",  Sum = vm.ToBePaidFull, PrincipalAmount=anaParadanDusulecekTutar, InterestAmount= faizdenOdenenPesinat, PaymentDate = ilkOdemeTarihi, RegDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) };
                payments.Add(paymentfull);
            }
            var newvm = new PreviewRequestVM()
            {
                Total = debtregister.Total,
                ToBePaidFull = vm.ToBePaidFull,
                NumOfMonths = vm.NumOfMonths,
                ToBePaidInstallment = amounttopay + interestamountSum,
                ToBePaidEachMonth = tobepaideachmonth,
                Payments= payments,
                FirstInstallmentDate=vm.FirstInstallmentDate,
                Description=vm.Description
            };
            return newvm;
        }
        

    }
}
