using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_ViewModelSample.Models;

namespace MVC_ViewModelSample.ViewModels
{
    public class CategoryDetailsViewModel
    {
        public Categories CategoryData { get; set; }
        public IEnumerable<Products> ProductCollection { get; set; }
    }
}