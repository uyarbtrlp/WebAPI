using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public class ProductModel
    {
        
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string UserId { get; set; }
        public string CategoryId { get; set; }
        public Category Category{ get; set; }
    }
}
