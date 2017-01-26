using System.Threading.Tasks;
using Authorization.Infrastructure.Authorization;
using Authorization.Models;
using Authorization.Services;
using Authorization.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Controllers
{
    [Authorize(Policies.Sales)]
    public class ProductsController : Controller
    {
        private readonly IProductsStore _store;
        private readonly IAuthorizationService _authz;

        public ProductsController(
            IProductsStore productsStore,
            IAuthorizationService authorizationService)
        {
            _store = productsStore;
            _authz = authorizationService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _store.GetAllAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id.HasValue == false)
            {
                return NotFound();
            }

            Product product = await _store.GetByIdAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(new EditProductViewModel
            {
                Product = product,
                Discount = 5
            });
        }

        // POST: Products/Discount/5
        [HttpPost]
        public async Task<IActionResult> Discount(ProductDiscountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", new { model.Id });
            }

            Product product = await _store.GetByIdAsync(model.Id);

            if (product == null)
            {
                return NotFound();
            }

            var operation = ProductOperations.GiveDiscount(model.Discount);

            if (await _authz.AuthorizeAsync(User, product, operation))
            {
                product.Price -= model.Discount;
                await _store.UpdateAsync(product);
                return RedirectToAction("Index");
            }

            return new ChallengeResult();
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        public async Task<IActionResult> Create(Product product, string name)
        {
            if (ModelState.IsValid)
            {
                await _store.AddAsync(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id.HasValue == false)
            {
                return NotFound();
            }

            Product product = await _store.GetByIdAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            if (await _authz.AuthorizeAsync(User, product, ProductOperations.Edit))
            {
                return View(product);
            }

            return new ChallengeResult();
        }

        // POST: Products/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                await _store.UpdateAsync(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id.HasValue == false)
            {
                return NotFound();
            }

            Product product = await _store.GetByIdAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product product = await _store.GetByIdAsync(id);

            await _store.RemoveAsync(product);

            return RedirectToAction("Index");
        }
    }
}
