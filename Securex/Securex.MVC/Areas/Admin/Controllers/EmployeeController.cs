using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Securex.BL.Extension;
using Securex.BL.VM.Employee;
using Securex.Core.Entities;
using Securex.DAL.Context;

namespace Securex.MVC.Areas.Admin.Controllers;
[Area("Admin")]
public class EmployeeController : Controller
{

    readonly AppDbContext _context;
    readonly IWebHostEnvironment _env; 
    public EmployeeController(AppDbContext context, IWebHostEnvironment env)
    {
        _env = env; 
        _context = context; 
    }
    private async Task PopulateAsync()
    {
        ViewBag.Departments = await _context.Departments.Where(x => !x.IsDeleted && !x.IsVisible).ToListAsync();
    }

    private async Task<IActionResult> ToggleEmployeeDeletion(int? id, bool isdeleted)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Employees.FindAsync(id);
        if (data == null) return NotFound();
        data.IsVisible = true;
        data.IsDeleted = isdeleted;
        await _context.SaveChangesAsync();
        return RedirectToAction(isdeleted ? nameof(Index) : nameof(Deleted)); 
    }

    private async Task<IActionResult> ToggleEmployeeVisible(int? id, bool visible)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Employees.FindAsync(id);
        if (data == null || data.IsDeleted) return NotFound();

        data.IsVisible = !visible;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> HandleModelErrorAsync<T>(T vm, string msg, string key = "")
    {
        ModelState.AddModelError(key, msg);
        await PopulateAsync();
        return View(vm); 
    }

    public async Task<IActionResult> Index()
    { 
        return View(await _context.Employees.Include(x=> x.Department).Where(x => !x.IsDeleted).ToListAsync());
    }
    public async Task<IActionResult> Deleted()
    {
        return View(await _context.Employees.Include(x => x.Department).Where(x => x.IsDeleted).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await PopulateAsync();
        return View(); 
    }
    [HttpPost]
    public async Task<IActionResult> Create(EmployeeCreateVM vm)
    {
        await PopulateAsync();

        if (!ModelState.IsValid)
            return await HandleModelErrorAsync(vm, "Model is not valid");

        if (await _context.Employees.AnyAsync(x => x.Fullname == vm.Fullname))
            return await HandleModelErrorAsync(vm, "An Employe with that name is already exists!");

        if(vm.Image == null || vm.Image.Length == 0)
            return await HandleModelErrorAsync(vm, "Image is Required", "Image");

        if (!vm.Image.IsValidType("image"))
            return await HandleModelErrorAsync(vm, "File type must be an Image", "Image");

        if (!vm.Image.IsValidSize(5))
            return await HandleModelErrorAsync(vm, "Image size must be less than 5MB", "Image");

        Employee employee = new Employee
        {
            Fullname = vm.Fullname,
            CreatedTime = DateTime.UtcNow,
            DepartmentId = vm.DepartmentId,
            ImageUrl = await  vm.Image.UploadAsync(_env.WebRootPath, "imgs", "employees")
        };

        await _context.Employees.AddAsync(employee); 
        await _context.SaveChangesAsync(); 
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Update(int? id)
    {
        if (!id.HasValue) return BadRequest();
        await PopulateAsync();
        var data = await _context.Employees
            .Where(x => !x.IsDeleted && !x.IsVisible)
            .Select(x => new EmployeeUpdateVM
            {
                Fullname = x.Fullname,
                DepartmentId = x.DepartmentId,
                ExistingImageUrl = x.ImageUrl
            }).FirstOrDefaultAsync();
        return View(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(EmployeeUpdateVM vm, int? id)
    {
        await PopulateAsync();

        if (!id.HasValue) return BadRequest();
        var data = await _context.Employees.FindAsync(id);
        if (data == null || data.IsDeleted) return NotFound();

        if (vm.Image != null )
        {
            if (!vm.Image.IsValidType("image"))
                return await HandleModelErrorAsync(vm, "File type must be an Image", "Image");

            if (!vm.Image.IsValidSize(5))
                return await HandleModelErrorAsync(vm, "Image size must be less than 5MB", "Image");

            await DeleteImage(data);

            string newFileName = await vm.Image.UploadAsync(_env.WebRootPath, "imgs", "employees");

            data.ImageUrl = newFileName; 
        }

        data.DepartmentId = vm.DepartmentId;
        data.Fullname = vm.Fullname;
        data.UpdatedTime = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));

    }
    public async Task<IActionResult> Show(int? id)
        => await ToggleEmployeeVisible(id, true);

    public async Task<IActionResult> Hide(int? id)
        => await ToggleEmployeeVisible(id, false);

    public async Task<IActionResult> Delete(int? id)
    => await ToggleEmployeeDeletion(id, true);

    public async Task<IActionResult> Repair(int? id)
        => await ToggleEmployeeDeletion(id, false);

    public async Task<IActionResult> Remove(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Employees.FindAsync(id);
        if (data == null || !data.IsDeleted) return NotFound();

        await DeleteImage(data); 

        _context.Remove(data);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Deleted));
    }
    public async Task<IActionResult> RemoveAll(string ids)
    {
        if (ids == null || ids.Length == 0) return BadRequest();

        if (string.IsNullOrWhiteSpace(ids))
            return NotFound();
        int[] idArray = ids.Split(',').Select(int.Parse).ToArray();
        var removeEmployees = await _context.Employees.Where(x => idArray.Contains(x.Id)).ToListAsync();
        if (removeEmployees.Count == 0) return NotFound();

        foreach (Employee employe in removeEmployees)
        {
            await DeleteImage(employe); 
        }

        _context.RemoveRange(removeEmployees);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Deleted));
    }
    private async Task DeleteImage(Employee employe)
    {

        string fileName = Path.Combine(_env.WebRootPath, "imgs", "employees", employe.ImageUrl);

        if (System.IO.File.Exists(fileName))
            System.IO.File.Delete(fileName);
    }
}
