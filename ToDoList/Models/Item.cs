using System.Collections.Generic;
using System;

namespace ToDoList.Models
{
    public class Item
    {
        public Item()
        {
            this.Categories = new HashSet<CategoryItem>();
            Marked = false;
        }

        public int ItemId { get; set; }
        public string Description { get; set; }
        public bool Marked { get; set; }
        public DateTime Date { get; set; }
        public virtual ApplicationUser User { get; set; }
        public ICollection<CategoryItem> Categories { get;}
    }
}