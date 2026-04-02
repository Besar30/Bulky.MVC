using Bulky.Data.Mapping;
using Bulky.Data.Models;
using Bulky.Data.ViewModel;
using Bulky.infrastructure.Repository;
using Bulky.infrastructure.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]

    public class CompanyController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult Index()
        {
            List<Company> Companys = _unitOfWork.CompanyRepository.GetAll().ToList();

            return View(Companys);
        }
        public IActionResult Upsert(int? id)
        {
          
            if(id == null)
            return View(new Company());
            else
            {
               Company company=_unitOfWork.CompanyRepository.Get(x=>x.Id==id);
                return View(company);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company company)
        {
          
            if (ModelState.IsValid)
            {
               
                if (company.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(company);
                    TempData["success"] = "Company created successfully.";
                }
                else
                {
                    Company comp = _unitOfWork.CompanyRepository.Get(x => x.Id == company.Id);
                    company.ToEntity(comp);
                    TempData["success"] = "Product updated successfully.";
                }

                _unitOfWork.save();
                return RedirectToAction("Index");
            }

           
            return View("Upsert", company);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? company = _unitOfWork.CompanyRepository.Get(x => x.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }
        [HttpPost()]
        public IActionResult DeleteCompany(int id)
        {
            Company? company = _unitOfWork.CompanyRepository.Get(x => x.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            _unitOfWork.CompanyRepository.Remove(company);
            _unitOfWork.save();
            TempData["success"] = "Company deleted successflly.";
            return RedirectToAction("Index");
        }
    }
}
