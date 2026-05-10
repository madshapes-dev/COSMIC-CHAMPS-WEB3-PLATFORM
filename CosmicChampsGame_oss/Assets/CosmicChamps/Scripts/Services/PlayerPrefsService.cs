using CosmicChamps.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace CosmicChamps.Services
{
    public class PlayerPrefsService
    {
        public abstract class Property<T>
        {
            protected readonly string _key;

            public abstract T Value { get; set; }

            protected Property (string key)
            {
                _key = key;
            }

            public void Clear (bool save = true)
            {
                PlayerPrefs.DeleteKey (_key);
                if (save)
                    PlayerPrefs.Save ();
            }
        }

        public class StringProperty : Property<string>
        {
            public StringProperty (string key) : base (key)
            {
            }

            public override string Value
            {
                get => PlayerPrefs.GetString (_key, null);
                set
                {
                    PlayerPrefs.SetString (_key, value);
                    PlayerPrefs.Save ();
                }
            }
        }

        public class IntProperty : Property<int>
        {
            public IntProperty (string key) : base (key)
            {
            }


            public override int Value
            {
                get => PlayerPrefs.GetInt (_key, 0);
                set
                {
                    PlayerPrefs.SetInt (_key, value);
                    PlayerPrefs.Save ();
                }
            }
        }

        public class TokensProperty : Property<Tokens>
        {
            public TokensProperty (string key) : base (key)
            {
            }

            public override Tokens Value
            {
                get => PlayerPrefs.HasKey (_key)
                    ? JsonConvert.DeserializeObject<Data.Tokens> (PlayerPrefs.GetString (_key))
                    : null;
                set => PlayerPrefs.SetString (_key, JsonConvert.SerializeObject (value));
            }
        }

        public class BoolProperty : Property<bool>
        {
            public BoolProperty (string key) : base (key)
            {
            }

            public override bool Value
            {
                get => PlayerPrefs.GetInt (_key, 0) != 0;
                set
                {
                    PlayerPrefs.SetInt (_key, value ? 1 : 0);
                    PlayerPrefs.Save ();
                }
            }
        }

        private const string TokensKey = "Tokens";
        private const string MuteSoundKey = "SoundsService.Mute";
        private const string DisclaimerShownKey = "DisclaimerShown";
        private const string WebGLDeviceIdKey = "CosmicChamps.DeviceId";

        public readonly TokensProperty Tokens = new(TokensKey);
        public readonly BoolProperty MuteSound = new(MuteSoundKey);
        public readonly BoolProperty DisclaimerShown = new(DisclaimerShownKey);
        public readonly StringProperty WebGLDeviceId = new(WebGLDeviceIdKey);

        public void ClearAll ()
        {
            Tokens.Clear (false);
            MuteSound.Clear (false);
            DisclaimerShown.Clear (false);
            WebGLDeviceId.Clear (false);

            PlayerPrefs.Save ();
        }
    }
}