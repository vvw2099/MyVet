﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Helper
{
    public interface IAgendaHelper
    {
        Task AddDaysAsync(int days);
    }
}
