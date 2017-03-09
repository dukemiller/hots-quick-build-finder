using System;
using System.Collections.Generic;
using hots_quick_build_finder.Models;

namespace hots_quick_build_finder.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        /// <summary>
        ///     The name of the service currently registered.
        /// </summary>
        string RegisteredBuildService { get; set; }

        /// <summary>
        ///     The date of the last time that the hero list was updated.
        /// </summary>
        DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The list of saved heroes with builds.
        /// </summary>
        List<Hero> Heroes { get; set; }

        /// <summary>
        ///     Easily retrieve from the hero list.
        /// </summary>
        Hero Find(string name);

        /// <summary>
        ///     Save the settings to disk.
        /// </summary>
        void Save();
    }
}