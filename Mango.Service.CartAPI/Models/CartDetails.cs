using Mango.Service.CartAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Service.CartAPI.Models
{
    public class CartDetails
    {
        [Key]
        public int CartDetailId { get; set; }
        public int cartHeadersId { get; set; }
        [ForeignKey ("cartHeadersId")]
        public CartHeader cartHeader { get; set; }

        public int ProductId { get; set; }
        [NotMapped]
        public ProductDTO Product { get; set; }
        public int Count { get; set; }
    }

}
