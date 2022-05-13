using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebService.Data;
using static WebService.Models.API;
using static WebService.Models.FilmModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;




namespace WebServices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string? userId;


        public HomeController(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            if (httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != null)
            {
                userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var InfoAPI = new ResultsAPI();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.themoviedb.org/");
                var reponseTask = client.GetAsync("3/discover/movie?api_key=" + _configuration["TokenMovie"]);
                reponseTask.Wait();
                var result = reponseTask.Result;

                if (result.IsSuccessStatusCode)
                {

                    string readTask = await result.Content.ReadAsStringAsync();
                    InfoAPI = JsonConvert.DeserializeObject<ResultsAPI>(readTask);
                    for (int i = 0; i < InfoAPI.results.Length; i++)
                    {
                        if (_context.Film.FirstOrDefault(c => c.Id == InfoAPI.results[i].Id) == null)
                        {
                            var film = new Film()
                            {
                                Id = InfoAPI.results[i].Id,
                                Like = 0
                            };
                            _context.Add(film);
                            await _context.SaveChangesAsync();
                        }
                        InfoAPI.results[i].Like = _context.Film.Where(w => w.Id == InfoAPI.results[i].Id).Select(s => s.Like).SingleOrDefault();
                    }
                    return View(InfoAPI.results.OrderBy(o => o.Id).ToList());
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erreur dans l'appel d'API.");
                }
                return View();
            }
            
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (_context.Film.FirstOrDefault(f => f.Id == id) == null)
            {
                return NotFound();
            }
            var film = await _context.Film.FirstOrDefaultAsync(c => c.Id == id);
            return View(film);
        }
        public int? AddLike(int id)
        {
            var film = _context.Film.FirstOrDefault(f => f.Id == id);
            var user = _context.UserFavoris.Include(c => c.Client).FirstOrDefault(f => f.Client.Id == userId);
            if (film.UserFavoris == null)
            {
                film.UserFavoris = new List<UserFavoris>();
            }
            try
            {
                film.UserFavoris.Add(user);
                film.Like++;
                _context.Update(film);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return _context.Film.Where(w => w.Id == id).Select(s => s.Like).SingleOrDefault();

        }

    }
}
