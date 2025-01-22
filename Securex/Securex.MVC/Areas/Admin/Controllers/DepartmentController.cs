using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Securex.BL.VM.Department;
using Securex.Core.Entities;
using Securex.DAL.Context;

namespace Securex.MVC.Areas.Admin.Controllers;
[Area("Admin")]
public class DepartmentController : Controller
{
    readonly AppDbContext _context;
    public DepartmentController(AppDbContext context)
    {
        _context = context;
    }

    private async Task<IActionResult> ToggleDepartmentVisibility(int? id, bool visible)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Departments.FindAsync(id);
        if (data == null || data.IsDeleted) return NotFound();

        data.IsVisible = !visible;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index)); 
    }

    private async Task<IActionResult> ChangeDepartmentDeletionStatus(int? id, bool isDeleted)
    {
        if (!id.HasValue) return BadRequest();

        var data = await _context.Departments.FindAsync(id);
        if (data == null) return NotFound();

        data.IsDeleted = isDeleted;
        data.IsVisible = true; 
        await _context.SaveChangesAsync();

        return RedirectToAction(isDeleted ? nameof(Index) : nameof(Deleted));
    }

    private IActionResult HandleModelErrorAsync<T>(T vm, string msg, string key = "")
    {
        ModelState.AddModelError(key, msg);
        return View(vm); 
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Departments.Where(x=> !x.IsDeleted).ToListAsync());
    }

    public async Task<IActionResult> Deleted()
    {
        return View(await _context.Departments.Where(x => x.IsDeleted).ToListAsync());
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(DepartmentCreateVM vm)
    {
        if (!ModelState.IsValid)
            return HandleModelErrorAsync(vm, "Model is not valid");

        if (await _context.Departments.AnyAsync(x => x.Name == vm.Name))
            return HandleModelErrorAsync(vm, "A department with that name is already exists");

        Department department = new Department
        {
            Name = vm.Name,
            CreatedTime = DateTime.UtcNow,
        };

        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index)); 
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (!id.HasValue) return BadRequest();

        var data = await _context.Departments
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new DepartmentUpdateVM
            {
                Name = x.Name
            }).FirstOrDefaultAsync(); 

        return View(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(DepartmentUpdateVM vm, int? id)
    {
        if (!ModelState.IsValid)
            return HandleModelErrorAsync(vm, "Model is not valid");

        if (!id.HasValue) return BadRequest();
        var data = await _context.Departments.FindAsync(id);
        if (data == null || data.IsDeleted) return NotFound();

        data.Name = vm.Name;
        data.UpdatedTime = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Show(int? id)
        => await ToggleDepartmentVisibility(id, true);

    public async Task<IActionResult> Hide(int? id)
        => await ToggleDepartmentVisibility(id, false);

    public async Task<IActionResult> Delete(int? id)
    => await ChangeDepartmentDeletionStatus(id, true);

    public async Task<IActionResult> Repair(int? id)
        => await ChangeDepartmentDeletionStatus(id, false);

    public async Task<IActionResult> Remove(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Departments.FindAsync(id);
        if (data == null || !data.IsDeleted) return NotFound();

        _context.Remove(data);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Deleted)); 
    }
    public async Task<IActionResult> RemoveAll(string ids )
    {
        if (string.IsNullOrWhiteSpace(ids))
            return BadRequest();

        int[] idArray = ids.Split(',').Select(int.Parse).ToArray();

        var removeDepartments = await _context.Departments.Where(x => idArray.Contains(x.Id)).ToListAsync();

        if (removeDepartments.Count == 0) return NotFound();

        _context.RemoveRange(removeDepartments);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Deleted));
    }
}
