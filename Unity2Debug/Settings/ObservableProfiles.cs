using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Unity2Debug.Common.SettingsService;

namespace Unity2Debug.Settings
{
    public partial class ObservableProfiles : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ObservableProfile> _profiles;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentProfile))]
        private int _currentProfileIndex;
        partial void OnCurrentProfileIndexChanged(int value)
        {
            CurrentProfile?.DebugSettings.NotifyAllChanged();
            CurrentProfile?.DecompileSettings.NotifyAllChanged();
        }

        public ObservableProfile? CurrentProfile
        {
            get
            {
                if (CurrentProfileIndex < 0 || CurrentProfileIndex >= Profiles.Count)
                    return null;

                return Profiles[CurrentProfileIndex];
            }
        }

        public ObservableCollection<string> ProfileNames
        {
            get => [.. Profiles.Select(x => x.Name)];
        }

        public ObservableProfiles(Common.SettingsService.Settings settings)
        {
            Profiles = [];

            foreach (var profile in settings.Profiles)
                Profiles.Add(new(profile.Key, new(profile.Value.DecompileSettings), new(profile.Value.DebugSettings)));

            Profiles.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ProfileNames));
        }

        public void AddProfile(string profileName)
        {
            if (!string.IsNullOrEmpty(profileName) && !Contains(profileName))
                Profiles.Add(new(profileName));
        }

        public void RemoveProfile()
        {
            if (CurrentProfileIndex >= 0 && CurrentProfileIndex < Profiles.Count)
                Profiles.RemoveAt(CurrentProfileIndex);
        }

        public void SelectProfile(string profileName)
        {
            int index = -1;

            if (!string.IsNullOrEmpty(profileName))
            {
                for (int i = 0; i < Profiles.Count; i++)
                {
                    if (string.Equals(Profiles[i].Name, profileName))
                    {
                        index = i;
                        break;
                    }
                }
            }

            CurrentProfileIndex = index;
        }

        public bool Contains(string profileName) => Profiles.Any(x => string.Equals(x.Name, profileName));

        public void Save()
        {
            var settings = new Dictionary<string, SettingProfile>();
            foreach (var profile in Profiles)
            {
                var decompile = profile.DecompileSettings.ToNonObservableSettings();
                var debug = profile.DebugSettings.ToNonObservableSettings();

                if (!settings.ContainsKey(profile.Name))
                    settings.Add(profile.Name, new(profile.Name, decompile, debug));
            }

            Common.SettingsService.Settings.Instance.Save(settings);
        }
    }
}
