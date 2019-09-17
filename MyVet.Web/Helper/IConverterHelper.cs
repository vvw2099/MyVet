using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyVet.Web.Data.Entities;
using MyVet.Web.Models;

namespace MyVet.Web.Helper
{
    public interface IConverterHelper
    {
        Task<Pet> ToPetAsync(PetViewModel model, string path, bool isnew);

        PetViewModel ToPetViewModel(Pet pet);

        Task<History> ToHistoryAsync(HistoryViewModel model, bool isnew);

        HistoryViewModel ToHistoryViewModel(History history);
    }
}
