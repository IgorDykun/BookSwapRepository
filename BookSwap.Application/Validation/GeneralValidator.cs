using FluentValidation;
using MongoDB.Bson;

namespace BookSwap.Application.Validators
{
    public static class GeneralValidator
    {
        public static IRuleBuilderOptions<T, string> ValidObjectId<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("ID не може бути порожнім")
                .Must(id => ObjectId.TryParse(id, out _))
                .WithMessage("Невірний формат ObjectId");
        }

        public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Email є обов’язковим")
                .EmailAddress().WithMessage("Невірний формат Email");
        }

        public static IRuleBuilderOptions<T, string> NotEmptyString<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string fieldName)
        {
            return ruleBuilder
                .NotEmpty().WithMessage($"{fieldName} не може бути порожнім")
                .MaximumLength(100).WithMessage($"{fieldName} не може перевищувати 100 символів");
        }
    }
}
