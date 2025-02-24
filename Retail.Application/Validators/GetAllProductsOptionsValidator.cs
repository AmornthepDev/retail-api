using FluentValidation;
using Retail.Application.Models;

namespace Retail.Application.Validators
{
    public class GetAllProductsOptionsValidator : AbstractValidator<GetAllProductsOptions>
    {
        private static readonly string[] AcceptableSortColumns = { "Name", "Sku", "Price", "Quantity" };
        private static readonly string[] AcceptableOrder = { "ASC", "DESC" };

        public GetAllProductsOptionsValidator()
        {
            RuleFor(x => x.SortColumn)
                .Must(x => x is null || AcceptableSortColumns.Contains(x, StringComparer.OrdinalIgnoreCase))
                .WithMessage("You can only sort by 'Name', 'Sku', 'Price' or 'Quantity'");

            RuleFor(x => x.SortColumn)
                .Must(x => x is null || AcceptableOrder.Contains(x, StringComparer.OrdinalIgnoreCase))
                .WithMessage("You can only sort by 'Name', 'Sku', 'Price' or 'Quantity'");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 25)
                .WithMessage("You can get between 1 and 25 records per page");
        }
    }
}
