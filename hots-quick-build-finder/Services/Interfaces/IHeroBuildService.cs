using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hots_quick_build_finder.Models;

namespace hots_quick_build_finder.Services.Interfaces
{
    public interface IHeroBuildService
    {
        /// <summary>
        ///     Gather builds for hero. If already exists take from local storage,
        ///     otherwise retrieve from remote service. (30 day refresh time minimum)
        /// </summary>
        Task<IEnumerable<Build>> BuildsFor(string name, Action<bool> isLoading);

        /// <summary>
        ///     Refresh an already existing heroes build page. (5 day refresh time minimum)
        /// </summary>
        Task Refresh(string name, Action<bool> isLoading);

        /// <summary>
        ///     The list of heroes from the remote service.
        /// </summary>
        Task<List<Hero>> HeroList();

        /// <summary>
        ///     Return the URL for the heroes page.
        /// </summary>
        string HeroPage(string name);
    }
}