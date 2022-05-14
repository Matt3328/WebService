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
        //permet de communiquer avec la base de données
        private readonly ApplicationDbContext _context;
        //permet d'accéder aux données dans le appsettings
        private readonly IConfiguration _configuration;
        private readonly string? userId;


        public HomeController(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            //permet d'obtenir l'id de l'utilisateur connecter
            if (httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != null)
            {
                userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var user = _context.UserFavoris.FirstOrDefault(f => f.Client.Id == userId);
            var InfoAPI = new ResultsAPI();
            //bloc pour l'appel de l'API
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.themoviedb.org/");
                var reponseTask = client.GetAsync("3/discover/movie?api_key=" + _configuration["TokenMovie"]);
                reponseTask.Wait();
                var result = reponseTask.Result;

                if (result.IsSuccessStatusCode)
                {

                    string readTask = await result.Content.ReadAsStringAsync();
                    //stocke les données obtenus dans une liste de film
                    InfoAPI = JsonConvert.DeserializeObject<ResultsAPI>(readTask);
                    for (int i = 0; i < InfoAPI.results.Length; i++)
                    {
                        //ajoute l'id du film dans notre BDD
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
                        //ajoute au model le nombre de like et si l'utilisateur connecté a liké pour chaque film
                        InfoAPI.results[i].Like = _context.Film.Where(w => w.Id == InfoAPI.results[i].Id).Select(s => s.Like).SingleOrDefault();
                        InfoAPI.results[i].IsLiked = _context.Film.Include(i => i.UserFavoris).AsEnumerable().Where(w => w.UserFavoris.Contains(user) && w.Id == InfoAPI.results[i].Id).Any();
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
            //vérification si l'id est null ou n'est pas présent dans notre BDD
            if (id == null)
            {
                return NotFound();
            }
            if (_context.Film.FirstOrDefault(f => f.Id == id) == null)
            {
                return NotFound();
            }

            var InfoAPI = new Movie();
            //bloc d'API pour récupérer un film spécifique
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.themoviedb.org/");
                var reponseTask = client.GetAsync("3/movie/" + id.ToString() + "?api_key=" + _configuration["TokenMovie"]);
                reponseTask.Wait();
                var result = reponseTask.Result;

                if (result.IsSuccessStatusCode)
                {

                    string readTask = await result.Content.ReadAsStringAsync();
                    InfoAPI = JsonConvert.DeserializeObject<Movie>(readTask);
                    InfoAPI.Like = _context.Film.Where(w => w.Id == id).Select(s => s.Like).SingleOrDefault();
                    var user = _context.UserFavoris.FirstOrDefault(f => f.Client.Id == userId);
                    InfoAPI.IsLiked = _context.Film.Include(i => i.UserFavoris).AsEnumerable().Where(w => w.UserFavoris.Contains(user) && w.Id == id).Any();
                    return View(InfoAPI);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erreur dans l'appel d'API.");
                }
                return View();
            }
        }
        //fonction pour ajouter ou enlever un like à un film.
        //il renvoie le nombre de like de ce film après l'ajout ou la suppression.
        //la fonction est apellé avec de l'ajax
        public int? AddOrRemoveLike(int id, string addOrRemove)
        {
            var film = _context.Film.FirstOrDefault(f => f.Id == id);
            var user = _context.UserFavoris.Include(c => c.Client).Include(c => c.FilmsLike).FirstOrDefault(f => f.Client.Id == userId);
            if (film.UserFavoris == null)
            {
                film.UserFavoris = new List<UserFavoris>();
            }
            try
            {
                if (addOrRemove == "add")
                {
                    film.UserFavoris.Add(user);
                    film.Like++;
                }
                else if (addOrRemove == "remove")
                {
                    user.FilmsLike.Remove(film);
                    film.UserFavoris.Remove(user);
                    film.Like--;
                }
                _context.Update(film);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _context.Film.Where(w => w.Id == id).Select(s => s.Like).SingleOrDefault();

        }

    }
}
