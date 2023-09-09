using System;
using FluentValidation;
using TechMastery.MarketPlace.Application.Features.Checkout.Dto;

namespace TechMastery.MarketPlace.Application.Validators
{
    public class CartItemDtoValidator : AbstractValidator<CartItemDto>
    {
        public CartItemDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithMessage("UserId cannot be empty.");

            RuleFor(x => x.ProductId)
                .NotEqual(Guid.Empty).WithMessage("ProductId cannot be empty.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity should be greater than zero.");
        }
    }
}

