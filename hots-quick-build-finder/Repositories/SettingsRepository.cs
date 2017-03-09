using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using hots_quick_build_finder.Models;
using hots_quick_build_finder.Repositories.Interfaces;
using hots_quick_build_finder.ViewModels;
using Newtonsoft.Json;

namespace hots_quick_build_finder.Repositories
{
    [Serializable]
    public class SettingsRepository: ISettingsRepository
    {
        private static readonly string SavePath = Path.Combine(MainWindowViewModel.ApplicationPath, "settings.json");

        public SettingsRepository()
        {
            if (!Directory.Exists(MainWindowViewModel.ApplicationPath))
                Directory.CreateDirectory(MainWindowViewModel.ApplicationPath);

            if (!Directory.Exists(Path.Combine(MainWindowViewModel.ApplicationPath, "images")))
                Directory.CreateDirectory(Path.Combine(MainWindowViewModel.ApplicationPath, "images"));
        }

        [JsonProperty("service")]
        public string RegisteredBuildService { get; set; } = "";

        [JsonProperty("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Today;

        [JsonProperty("heroes")]
        public List<Hero> Heroes { get; set; } = new List<Hero>();
        
        public Hero Find(string name)
        {
            return Heroes.FirstOrDefault(hero => hero.InternalName.Equals(name) || hero.Name.Equals(name));
        }

        public void Save()
        {
            using (var stream = new StreamWriter(SavePath))
                stream.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static SettingsRepository Load()
        {
            if (!Directory.Exists(MainWindowViewModel.ApplicationPath))
                Directory.CreateDirectory(MainWindowViewModel.ApplicationPath);

            if (File.Exists(SavePath))
                using (var stream = new StreamReader(SavePath))
                    return JsonConvert.DeserializeObject<SettingsRepository>(stream.ReadToEnd());

            return new SettingsRepository();
        }
    }
}