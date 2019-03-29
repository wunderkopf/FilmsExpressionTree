using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FilmsExpressionTree.Models;

namespace FilmsExpressionTree
{
    class Program
    {
        public static IEnumerable<Film> SeedData()
        {
            Genre drama = new Genre
            {
                Title = "drama"
            };

            Genre action = new Genre
            {
                Title = "action"
            };

            Genre comedy = new Genre
            {
                Title = "comedy"
            };

            Genre fantasy = new Genre
            {
                Title = "fantasy"
            };

            Genre sf = new Genre
            {
                Title = "sci-fi"
            };

            Genre thriller = new Genre
            {
                Title = "thriller"
            };

            Genre horror = new Genre
            {
                Title = "horror"
            };

            Film f1 = new Film
            {
                Title = "Fight Club",
                Year = 1999
            };

            Film f2 = new Film
            {
                Title = "The Dark Knight",
                Year = 2008
            };

            Film f3 = new Film
            {
                Title = "Star Wars: Episode V - The Empire Strikes Back",
                Year = 1980
            };

            Film f4 = new Film
            {
                Title = "Alien",
                Year = 1979
            };

            Film f5 = new Film
            {
                Title = "Trainspotting",
                Year = 1996
            };

            // ----
            f1.AddGenre(drama);
            f1.AddGenre(action);
            f1.AddGenre(thriller);
            yield return f1;

            f2.AddGenre(drama);
            f2.AddGenre(action);
            f2.AddGenre(fantasy);
            yield return f2;

            f3.AddGenre(drama);
            f3.AddGenre(action);
            f3.AddGenre(fantasy);
            f3.AddGenre(sf);
            yield return f3;

            f4.AddGenre(drama);
            f4.AddGenre(action);
            f4.AddGenre(horror);
            f4.AddGenre(sf);
            f4.AddGenre(thriller);
            yield return f4;

            f5.AddGenre(drama);
            f5.AddGenre(action);
            f5.AddGenre(comedy);
            yield return f5;
        }
        static void Main(string[] args)
        {
            var films = SeedData();
            Console.WriteLine("----");
            Console.WriteLine("Enter the request: ");
            string query = Console.ReadLine();
            string[] parts = query.Split(';');
            foreach (var part in parts)
            {
                //Console.WriteLine(part);
                string[] items = part.Split(' ');
                if (items.Length == 3)
                {
                    ParameterExpression pe = Expression.Parameter(typeof(Film), "f");

                    MemberExpression me = null;
                    if (string.Equals(items[0], "year", StringComparison.OrdinalIgnoreCase))
                        me = Expression.Property(pe, "Year");

                    int year = Int32.Parse(items[2]);
                    ConstantExpression constant = Expression.Constant(year, typeof(int));
                    
                    BinaryExpression expr = null;
                    if (items[1] == "<")
                        expr = Expression.LessThan(me, constant);
                    else if (items[1] == ">")
                        expr = Expression.GreaterThan(me, constant);
                    else if (items[1] == "=")
                        expr = Expression.Equal(me, constant);

                    var check = Expression.Lambda<Func<Film, bool>>(expr, new[] { pe }).Compile();

                    foreach (var film in films)
                    {
                        bool res = check(film);
                        int gg = 6;
                    }
                }
            }
            Console.WriteLine("---");
        }
    }
}
