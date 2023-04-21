using DotNetCoreSqlDb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DotNetCoreSqlDb.Controllers;

public class TodosController : Controller
{
	private readonly MyDatabaseContext _context;

	public TodosController(MyDatabaseContext context)
	{
		_context = context;
	}

	// GET: Todos
	public async Task<IActionResult> Index()
	{
		return View(await _context.Todo.ToListAsync());
	}

	// GET: Todos/Details/5
	public async Task<IActionResult> Details(int? id)
	{
		return await ReturnViewForId(id);
	}

	// GET: Todos/Create
	public IActionResult Create()
	{
		return View(new Todo { CreatedDate = DateTime.Now });
	}

	// POST: Todos/Create
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("ID,Description,CreatedDate")] Todo todo)
	{
		if (ModelState.IsValid)
		{
			_context.Add(todo);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		return View(todo);
	}

	// GET: Todos/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var todo = await _context.Todo.FindAsync(id);
		return todo == null ? NotFound() : View(todo);
	}

	// POST: Todos/Edit/5
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, [Bind("ID,Description,CreatedDate")] Todo todo)
	{
		if (id != todo.ID)
		{
			return NotFound();
		}

		if (ModelState.IsValid)
		{
			try
			{
				_context.Update(todo);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				var exists = await TodoExistsAsync(todo.ID);
				if (!exists)
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		return View(todo);
	}

	// GET: Todos/Delete/5
	public async Task<IActionResult> Delete(int? id)
	{
		return await ReturnViewForId(id);
	}

	private async Task<IActionResult> ReturnViewForId(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var todo = await _context.Todo.FirstOrDefaultAsync(m => m.ID == id);

		return todo == null ? NotFound() : View(todo);
	}

	// POST: Todos/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var todo = await _context.Todo.FindAsync(id);
		_context.Todo.Remove(todo);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private async Task<bool> TodoExistsAsync(int id)
	{
		return await _context.Todo.AnyAsync(e => e.ID == id);
	}
}
