using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

//new using directives
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ToDoList.Controllers
{
  [Authorize] //new line
  public class ItemsController : Controller
  {
    private readonly ToDoListContext _db;
    private readonly UserManager<ApplicationUser> _userManager; //new line

    //updated constructor
    public ItemsController(UserManager<ApplicationUser> userManager, ToDoListContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    //updated Index method
    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userItems = _db.Items.Where(entry => entry.User.Id == currentUser.Id);
      return View(userItems);
    }

    public ActionResult Create()
    {
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View();
    }

    //updated Create post method
    [HttpPost]
    public async Task<ActionResult> Create(Item item, int CategoryId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      item.User = currentUser;
      _db.Items.Add(item);
      if (CategoryId != 0)
      {
        _db.CategoryItem.Add(new CategoryItem() { CategoryId = CategoryId, ItemId = item.ItemId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpGet("/Item/Details/{id}")]
    public ActionResult Details(int id)
    {
      var thisItem = _db.Items
          .Include(item => item.Categories)
          .ThenInclude(join => join.Category)
          .FirstOrDefault(item => item.ItemId == id);
      return View(thisItem);
    }

    // [HttpGet()]
    // public ActionResult Details(Item i)
    // {
    //    var thisItem = _db.Items
    //       .Include(item => item.Categories)
    //       .ThenInclude(join => join.Category)
    //       .FirstOrDefault(item => item.ItemId == i.ItemId);
    //   return View(thisItem);
    // }

    public ActionResult Edit(int id)
    {
      var thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View(thisItem);
    }

    [HttpPost]
    public ActionResult Edit(Item item, int CategoryId)
    {
      if (CategoryId != 0)
      {
        _db.CategoryItem.Add(new CategoryItem() { CategoryId = CategoryId, ItemId = item.ItemId });
      }
      _db.Entry(item).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddCategory(int id)
    {
      var thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View(thisItem);
    }

    [HttpPost]
    public ActionResult AddCategory(Item item, int CategoryId)
    {
      if (CategoryId != 0)
      {
        _db.CategoryItem.Add(new CategoryItem() { CategoryId = CategoryId, ItemId = item.ItemId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
      return View(thisItem);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
      _db.Items.Remove(thisItem);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteCategory(int joinId)
    {
      var joinEntry = _db.CategoryItem.FirstOrDefault(entry => entry.CategoryItemId == joinId);
      _db.CategoryItem.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost("/Items/Finished/{id}")]
    public ActionResult Finished(int id)
    {
      Item foundItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
      foundItem.Marked = true;
      _db.Entry(foundItem).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new {id=id});
    }

    [HttpPost("/Items/Unfinished/{id}")]
    public ActionResult Unfinished(int id)
    {
      Item foundItem = _db.Items.FirstOrDefault(name => name.ItemId == id);
      foundItem.Marked = false;
      _db.Entry(foundItem).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new {id=id});
    }
    [HttpGet]
    public async Task<ActionResult> Alphabetize()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userList = _db.Items.Where(entry => entry.User.Id == currentUser.Id);
      var userItems = userList.OrderBy(order => order.Date);
      return View("Index", userItems);
    }
  }
}