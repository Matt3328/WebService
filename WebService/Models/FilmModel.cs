using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebService.Models
{
    public class FilmModel
    {
        public class Film
        {
            //Desactiver l'auto insert de l'id
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int Id { get; set; }
            public int? Like { get; set; }
            public ICollection<UserFavoris>? UserFavoris { get; set; }

        }
        public class UserFavoris
        {
            public int Id { get; set; }
            public ICollection<Film>? FilmsLike { get; set; }
            public IdentityUser Client { get; set; }
        }

    }
}
