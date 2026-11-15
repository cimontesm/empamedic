using frontWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace frontWeb.Controllers
{
    public class ColaboradoresController : Controller
    {
        private readonly HttpClient _httpClient;

        public ColaboradoresController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            // Verifica que coincida con el puerto configurado en Program.cs de frontWeb
            _httpClient.BaseAddress = new Uri("http://localhost:5164/");
        }

        // GET: Colaboradores
        public async Task<IActionResult> Index()
        {
            var colaboradores = await _httpClient.GetFromJsonAsync<List<ColaboradorViewModel>>("api/colaboradores");
            return View(colaboradores ?? new List<ColaboradorViewModel>());
        }

        // GET: Colaboradores/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var colaborador = await _httpClient.GetFromJsonAsync<ColaboradorViewModel>($"api/colaboradores/{id}");
            if (colaborador == null) return NotFound();
            return View(colaborador);
        }

        // GET: Colaboradores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Colaboradores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ColaboradorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _httpClient.PostAsJsonAsync("api/colaboradores", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Error al guardar el colaborador en el servidor.");
            return View(model);
        }

        // GET: Colaboradores/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var colaborador = await _httpClient.GetFromJsonAsync<ColaboradorViewModel>($"api/colaboradores/{id}");
            if (colaborador == null) return NotFound();
            return View(colaborador);
        }

        // POST: Colaboradores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ColaboradorViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var response = await _httpClient.PutAsJsonAsync($"api/colaboradores/{id}", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Error al actualizar los datos.");
            return View(model);
        }

        // POST: Colaboradores/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _httpClient.DeleteAsync($"api/colaboradores/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
