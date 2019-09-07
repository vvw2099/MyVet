using System.ComponentModel.DataAnnotations;
using MyVet.Web.Data.Entities;
using System.Collections.Generic;

namespace MyVet.Web.Data.Entities
{
    public class Owner
    {
        public int Id { get; set; }
        public User user { get; set; }

        public ICollection<Pet> Pets { get; set; }
        public ICollection<Agenda> Agendas { get; set; }
    }
}
