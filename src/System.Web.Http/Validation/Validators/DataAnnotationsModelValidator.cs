﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Web.Http.Metadata;

namespace System.Web.Http.Validation.Validators
{
    // [SecuritySafeCritical] because it has ctor and properties exposing DataAnnotations types
    [SecuritySafeCritical]
    public class DataAnnotationsModelValidator : ModelValidator
    {
        public DataAnnotationsModelValidator(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, ValidationAttribute attribute)
            : base(metadata, validatorProviders)
        {
            if (attribute == null)
            {
                throw Error.ArgumentNull("attribute");
            }

            Attribute = attribute;
        }

        protected internal ValidationAttribute Attribute { get; private set; }

        protected internal string ErrorMessage
        {
            get { return Attribute.FormatErrorMessage(Metadata.GetDisplayName()); }
        }

        public override bool IsRequired
        {
            // [SecuritySafeCritical] because it uses DataAnnotations type RequiredAttribute
            [SecuritySafeCritical]
            get { return Attribute is RequiredAttribute; }
        }

        internal static ModelValidator Create(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, ValidationAttribute attribute)
        {
            return new DataAnnotationsModelValidator(metadata, validatorProviders, attribute);
        }

        // [SecuritySafeCritical] because is uses DataAnnotations type ValidationContext
        [SecuritySafeCritical]
        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            // Per the WCF RIA Services team, instance can never be null (if you have
            // no parent, you pass yourself for the "instance" parameter).
            ValidationContext context = new ValidationContext(container ?? Metadata.Model, null, null);
            context.DisplayName = Metadata.GetDisplayName();

            ValidationResult result = Attribute.GetValidationResult(Metadata.Model, context);

            if (result != ValidationResult.Success)
            {
                return new ModelValidationResult[] { new ModelValidationResult { Message = result.ErrorMessage } };
            }

            return new ModelValidationResult[0];
        }
    }
}
