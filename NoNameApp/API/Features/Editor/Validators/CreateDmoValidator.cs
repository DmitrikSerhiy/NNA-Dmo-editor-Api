using FluentValidation;
using Model;
using Model.DTOs.Editor;

namespace API.Features.Editor.Validators {
    public class CreateDmoValidator : AbstractValidator<CreateDmoDto> {
        public CreateDmoValidator() {
            RuleFor(d => d.Name)
                .NotEmpty()
                .WithMessage("Dmo name is missing")
                .MaximumLength(ApplicationConstants.MaxEntityNameLength)
                .WithMessage($"Maximum dmo name length is {ApplicationConstants.MaxEntityNameLength}");

            RuleFor(d => d.MovieTitle)
                .NotEmpty()
                .WithMessage("Movie title is missing")
                .MaximumLength(ApplicationConstants.MaxEntityNameLength)
                .WithMessage($"Maximum movie title length is {ApplicationConstants.MaxEntityNameLength}");

            RuleFor(d => d.ShortComment)
                .MaximumLength(ApplicationConstants.MaxShortCommentLength)
                .WithMessage($"Maximum comment length is {ApplicationConstants.MaxShortCommentLength}");
        }
    }
}
