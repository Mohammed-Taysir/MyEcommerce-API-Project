using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repositry
{
    public class CategoryRepositry : GenericRepositry<Category>, ICategoryRepositry
    {
        public CategoryRepositry(ApplicationDbContext context) : base(context) { }
    }
}
