using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Models.Order {

    public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest> {

        public UpdateOrderRequestValidator() {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProductsId).NotEmpty();
        }
    }
}
