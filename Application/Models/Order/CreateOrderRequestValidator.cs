using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Models.Order {

    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest> {

        public CreateOrderRequestValidator() {
            RuleFor(x => x.ProductsId).NotEmpty();
        }
    }
}
