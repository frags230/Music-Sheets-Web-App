using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webapp.Models;
using webapp.Data;

namespace webapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(string username, string email, string password)
        {
            var newUser = new User
            {
                Username = username,
                Email = email,
                Password = password // Remember to hash passwords in a real application
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SignIn(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                // User found, redirect to Music Sheet Finder
                return RedirectToAction("HomePage");
            }
            else
            {
                // User not found, show error message (you can customize this)
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View("Index");
            }
        }
       
        public IActionResult HomePage(string searchString, string instrumentFilter)
        {
            var songs = _context.Songs.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                songs = songs.Where(s => s.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(instrumentFilter))
            {
                songs = songs.Where(s => s.Instrument == instrumentFilter);
            }

            var songList = songs.ToList();
            return View(songList);
        }

        public IActionResult AddSong()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddSong(string songTitle, string instrument, IFormFile sheetMusic)
        {
            if (sheetMusic != null && sheetMusic.Length > 0)
            {
                var fileName = Path.GetFileName(sheetMusic.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // Ensure the directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    sheetMusic.CopyTo(stream);
                }

                var newSong = new Song
                {
                    Title = songTitle,
                    Instrument = instrument,
                    FilePath = $"/uploads/{fileName}"
                };

                _context.Songs.Add(newSong);
                _context.SaveChanges();
            }

            // Redirect to HomePage after adding the song
            return RedirectToAction("HomePage");
        }



            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
