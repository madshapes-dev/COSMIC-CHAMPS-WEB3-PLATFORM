using System;
using System.Text.RegularExpressions;
using CosmicChamps.Signals;
using ThirdParty.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;

namespace CosmicChamps.HomeScreen
{
    public class SignUpValidator
    {
        [Serializable]
        public struct Errors
        {
            [SerializeField]
            private LocalizedString _emptyEmailError;

            [SerializeField]
            private LocalizedString _invalidEmailError;

            [SerializeField]
            private LocalizedString _emptyPasswordError;

            [SerializeField]
            private LocalizedString _passwordNotMatchError;

            [SerializeField]
            private LocalizedString _passwordNotMeetPolicy;

            public LocalizedString EmptyEmailError => _emptyEmailError;
            public LocalizedString InvalidEmailError => _invalidEmailError;
            public LocalizedString EmptyPasswordError => _emptyPasswordError;
            public LocalizedString PasswordNotMatchError => _passwordNotMatchError;
            public LocalizedString PasswordNotMeetPolicy => _passwordNotMeetPolicy;
        }

        private readonly Errors _errors;
        private readonly IMessageBroker _messageBroker;

        public SignUpValidator (Errors errors, IMessageBroker messageBroker)
        {
            _errors = errors;
            _messageBroker = messageBroker;
        }

        private void FireErrorSignal (string message, string stacktrace) =>
            _messageBroker.Publish (new ErrorSignal (message, stacktrace, false, false, false));


        public bool Validate (string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty (email))
            {
                FireErrorSignal (_errors.EmptyEmailError.GetLocalizedString (), string.Empty);
                return false;
            }

            if (!email.IsValidEmail ())
            {
                FireErrorSignal (_errors.InvalidEmailError.GetLocalizedString (), string.Empty);
                return false;
            }

            if (string.IsNullOrEmpty (password))
            {
                FireErrorSignal (_errors.EmptyPasswordError.GetLocalizedString (), string.Empty);
                return false;
            }

            if (password != confirmPassword)
            {
                FireErrorSignal (_errors.PasswordNotMatchError.GetLocalizedString (), string.Empty);
                return false;
            }

            var passwordRegex = new Regex (
                "^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[\\^$*.[\\]{}()?\"!@#%&/\\\\,><':;|_~`=+-]).{8,256}$");

            if (!passwordRegex.IsMatch (password))
            {
                FireErrorSignal (_errors.PasswordNotMeetPolicy.GetLocalizedString (), string.Empty);
                return false;
            }

            return true;
        }
    }
}