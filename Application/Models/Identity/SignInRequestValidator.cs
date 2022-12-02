using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Models.Identity {

    public class SignInRequestValidator : AbstractValidator<SignInRequest> {
        
        public SignInRequestValidator() {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
