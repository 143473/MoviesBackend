using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApi.Data.Models;

public class Favorites
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int UserId { get; set; }
}