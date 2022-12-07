﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities {

    public class OrderProduct {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
    }
}
