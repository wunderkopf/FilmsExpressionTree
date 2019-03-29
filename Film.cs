using System.Collections;
using System.Collections.Generic;

namespace FilmsExpressionTree.Models
{
    public class Film
    {
        private ICollection<Genre> genres = new List<Genre>();
        public int Id { get; set; }

        public int Year { get; set; }

        public string Title { get; set; }

        public ICollection<Genre> Genres
        {
            get
            {
                return genres;
            }
        }

        public void AddGenre(Genre genre)
        {
            this.genres.Add(genre);
        }
    }
}
