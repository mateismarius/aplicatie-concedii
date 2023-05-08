using FluentValidation;
using Utilitar.AppContext;
using Utilitar.Models;

namespace Utilitar.CustomValidation
{
    public class FreeDayValidatior : AbstractValidator<FreeDay>
    {
       
        public FreeDayValidatior()
        {
           
            RuleFor(x => x.RequestNumber).NotNull().WithName("Numarul cererii").WithMessage("* Acest camp nu poate fi gol!");
            RuleFor(x => x.RequestDate).NotNull().WithName("Data cererii").WithMessage("* Acest camp nu poate fi gol!");
        }
    }
}
