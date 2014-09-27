﻿using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace MediatR.Extensions.FluentValidation
{
    public class ValidationRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _innerHander;
        private readonly IValidator<TRequest>[] _validators;

        public ValidationRequestHandler(IRequestHandler<TRequest, TResponse> innerHandler, IValidator<TRequest>[] validators)
        {
            _validators = validators;
            _innerHander = innerHandler;
        }

        public TResponse Handle(TRequest message)
        {
            var context = new ValidationContext(message);

            var failures =
                _validators.Select(v => v.Validate(context)).SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return _innerHander.Handle(message);
        }
    }

    public class AsyncValidationRequestHandler<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse> where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _innerHander;
        private readonly IValidator<TRequest>[] _validators;

        public AsyncValidationRequestHandler(IAsyncRequestHandler<TRequest, TResponse> innerHandler, IValidator<TRequest>[] validators)
        {
            _validators = validators;
            _innerHander = innerHandler;
        }

        public async Task<TResponse> Handle(TRequest message)
        {
            var context = new ValidationContext(message);

            var failures =
                _validators.Select(v => v.Validate(context)).SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return await _innerHander.Handle(message);
        }
    }

    
}