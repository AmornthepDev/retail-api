﻿namespace Retail.Contracts.Validation
{
    public class ValidationFailureResponse
    {
        public required IEnumerable<ValidationResponse> Errors { get; init; } 
    }
}
