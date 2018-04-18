﻿using System;

namespace Core.Model.Validation
{
    public interface IRule
    {
        bool IsSuppressed { get; }
        bool IsValid { get; }
        IValidationError ValidationError { get; }

        IRule ExceptWhen(Func<bool> conditionExpression);

        IRule When(Func<bool> conditionExpression);
    }
}