using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Voucher;

namespace BusinessLogicLayer.Viewmodels.VoucherM
{
    public class CustomDateGreaterThanTodayAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime >= DateTime.Today;
            }
            return false;
        }
    }
    public class CustomDateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public CustomDateGreaterThanAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            if (startDateProperty == null)
            {
                return new ValidationResult($"Unknown property: {_startDatePropertyName}");
            }

            var startDate = (DateTime)startDateProperty.GetValue(validationContext.ObjectInstance);

            if (value is DateTime endDate && endDate <= startDate)
            {
                return new ValidationResult(ErrorMessage ?? "Ngày kết thúc phải lớn hơn ngày bắt đầu");
            }

            return ValidationResult.Success;
        }
    }
    public class CustomDecimalGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _minimumAmountPropertyName;

        public CustomDecimalGreaterThanAttribute(string minimumAmountPropertyName)
        {
            _minimumAmountPropertyName = minimumAmountPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var minimumAmountProperty = validationContext.ObjectType.GetProperty(_minimumAmountPropertyName);
            if (minimumAmountProperty == null)
            {
                return new ValidationResult($"Unknown property: {_minimumAmountPropertyName}");
            }

            var minimumAmount = (decimal)minimumAmountProperty.GetValue(validationContext.ObjectInstance);

            if (value is decimal maximumAmount && maximumAmount <= minimumAmount)
            {
                return new ValidationResult(ErrorMessage ?? "Số tiền giảm tối đa phải lớn hơn số tiền giảm tối thiểu");
            }

            return ValidationResult.Success;
        }
    }
    public class CustomUserSelectionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CreateVoucherVM)validationContext.ObjectInstance;

            // Nếu chọn áp dụng cho tất cả khách hàng, không cần kiểm tra SelectedUser
            if (model.ApplyToAllUsers)
            {
                return ValidationResult.Success;
            }

            // Nếu không chọn áp dụng cho tất cả khách hàng và SelectedUser rỗng, trả lỗi
            if (!model.ApplyToAllUsers && (model.SelectedUser == null || model.SelectedUser.Count == 0))
            {
                return new ValidationResult("Vui lòng chọn ít nhất một khách hàng.");
            }

            return ValidationResult.Success;
        }
    }
    public class ValidateReducedValueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CreateVoucherVM)validationContext.ObjectInstance;

            // Nếu không có giá trị cho ReducedValue, không kiểm tra
            if (model.ReducedValue <= 0)
            {
                return ValidationResult.Success;
            }

            // Kiểm tra theo loại giảm giá
            if (model.Type == Types.Percent)
            {
                if (model.ReducedValue <= 0 || model.ReducedValue > 100)
                {
                    return new ValidationResult("Số tiền giảm phải lớn hơn 0 và nhỏ hơn hoặc bằng 100%");
                }
            }
            else if (model.Type == Types.Cash)
            {
                if (model.ReducedValue <= 0 || model.ReducedValue > 100000000)
                {
                    return new ValidationResult("Số tiền giảm phải lớn hơn 0 và nhỏ hơn hoặc bằng 100000000 đồng");
                }
            }

            return ValidationResult.Success;
        }
    }
    public class ValidateReducedValueUpdateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateVC)validationContext.ObjectInstance;

            // Nếu không có giá trị cho ReducedValue, không kiểm tra
            if (model.ReducedValue <= 0)
            {
                return ValidationResult.Success;
            }

            // Kiểm tra theo loại giảm giá
            if (model.Type == Types.Percent)
            {
                if (model.ReducedValue <= 0 || model.ReducedValue > 100)
                {
                    return new ValidationResult("Số tiền giảm phải lớn hơn 0 và nhỏ hơn hoặc bằng 100%");
                }
            }
            else if (model.Type == Types.Cash)
            {
                if (model.ReducedValue <= 0 || model.ReducedValue > 100000000)
                {
                    return new ValidationResult("Số tiền giảm phải lớn hơn 0 và nhỏ hơn hoặc bằng 100000000 đồng");
                }
            }

            return ValidationResult.Success;
        }
    }
    public class voucherUserSelectionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateVC)validationContext.ObjectInstance;

            // Nếu chọn áp dụng cho tất cả khách hàng, không cần kiểm tra SelectedUser
            if (model.ApplyToAllUsers)
            {
                return ValidationResult.Success;
            }

            // Nếu không chọn áp dụng cho tất cả khách hàng và SelectedUser rỗng, trả lỗi
            if (!model.ApplyToAllUsers && (model.SelectedUser == null || model.SelectedUser.Count == 0))
            {
                return new ValidationResult("Vui lòng chọn ít nhất một khách hàng.");
            }

            return ValidationResult.Success;
        }
    }
    public class NumericAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            // Kiểm tra nếu giá trị là số nguyên
            if (int.TryParse(value.ToString(), out _))
            {
                return true;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Trường {name} phải là số nguyên.";
        }
    }
    public class CustomDecimalLessThanAttribute : ValidationAttribute
    {
        private readonly string _maximumAmountPropertyName;

        public CustomDecimalLessThanAttribute(string maximumAmountPropertyName)
        {
            _maximumAmountPropertyName = maximumAmountPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var maximumAmountProperty = validationContext.ObjectType.GetProperty(_maximumAmountPropertyName);
            if (maximumAmountProperty == null)
            {
                return new ValidationResult($"Unknown property: {_maximumAmountPropertyName}");
            }

            var maximumAmount = (decimal)maximumAmountProperty.GetValue(validationContext.ObjectInstance);

            if (value is decimal minimumAmount && minimumAmount < maximumAmount)
            {
                return new ValidationResult(ErrorMessage ?? "Số tiền giảm tối thiểu phải lớn hơn hoặc bằng số tiền giảm tối đa");
            }

            return ValidationResult.Success;
        }
    }

}
