using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using hots_quick_build_finder.Models;
using hots_quick_build_finder.Repositories.Interfaces;
using hots_quick_build_finder.Services.Interfaces;
using hots_quick_build_finder.ViewModels;
using HtmlAgilityPack;
using Build = hots_quick_build_finder.Models.Build;

namespace hots_quick_build_finder.Services
{
    public class IcyVeinsService: IHeroBuildService
    {
        private readonly ISettingsRepository _settingsRepository;

        private const string HeroUrl = @"http://www.icy-veins.com/heroes";

        private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;

        private readonly HttpClient _client = new HttpClient();

        private readonly WebClient _downloader = new WebClient();

        // 

        public IcyVeinsService(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
            _downloader.Headers["User-Agent"] = "hots-quick-build-finder";
            _client.DefaultRequestHeaders.Add("User-Agent", "hots-quick-build-finder");
        }

        // 

        public async Task Refresh(string name, Action<bool> isLoading)
        {
            var hero = _settingsRepository.Find(name);

            if (hero == null)
                return;

            if ((DateTime.Now - hero.LastUpdated).TotalDays > 5)
            {
                isLoading(true);

                hero.Builds = await GetBuilds(hero);
                hero.LastUpdated = DateTime.Now;
                _settingsRepository.Save();

                isLoading(false);
            }
        }

        public async Task<IEnumerable<Build>> BuildsFor(string name, Action<bool> isLoading)
        {
            var hero = _settingsRepository.Find(name);

            if (hero == null)
                return null;

            // Get new builds or update old builds
            if (hero.Builds.Count == 0 || (DateTime.Now - hero.LastUpdated).TotalDays > 30)
            {
                isLoading(true);

                hero.Builds = await GetBuilds(hero);
                hero.LastUpdated = DateTime.Now;
                _settingsRepository.Save();

                isLoading(false);
            }

            return hero.Builds;
        }

        public async Task<List<Hero>> HeroList()
        {
            var document = new HtmlDocument();
            var response = await _client.GetAsync(HeroUrl);
            document.LoadHtml(await response.Content.ReadAsStringAsync());
            var heroes = document.DocumentNode.Descendants("a")
                .Where(a => a.Attributes["href"]?.Value.Contains("build-guide") == true)
                .Select(build => string.Join("-",
                    build.Attributes["href"].Value
                        .Split('/')[4]
                        .Split('-')
                        .TakeWhile(s => !s.Equals("build"))))
                .Distinct()
                .Select(name => new Hero
                {
                    Name = TextInfo.ToTitleCase(name.Replace("-", " ")),
                    InternalName = name
                })
                .OrderBy(hero => hero.Name);

            return heroes.ToList();
        }

        public string HeroPage(string name)
        {
            var hero = _settingsRepository.Find(name);
            return hero == null ? null : $"{HeroUrl}/{hero.InternalName}-build-guide";
        }

        // 

        private async Task DownloadTalentImage(Talent talent)
        {
            var filename = Path.GetFileName(new Uri(talent.Image).AbsolutePath);
            var downloadPath = Path.Combine(MainWindowViewModel.ApplicationPath, "images", filename);
            if (!File.Exists(downloadPath))
                await _downloader.DownloadFileTaskAsync(talent.Image, downloadPath);
            talent.Image = downloadPath;
        }

        private static Build ParseBuild(HtmlNode titleNode, HtmlNode talentsNode)
        {
            return new Build
            {
                Title = titleNode.InnerText.Replace(" (talent calculator link)", "").Replace("\n", "").Split(':')[0],
                Talents = ParseTalents(talentsNode)
            };
        }

        private static List<Talent> ParseTalents(HtmlNode talentsNode)
        {
            return talentsNode
                .Descendants("a")
                .Select(a =>
                {
                    var visualTree = a
                        .Descendants("span")
                        .First(span => span.Attributes["class"]?.Value.Equals("heroes_tldr_talent_tier_visual") == true)
                        .Descendants("span")
                        .ToList();

                    var header = a.Descendants("span").First().InnerText;
                    var image = "http://" + a.Descendants("img").First().Attributes["src"].Value.Substring(2);
                    var position = visualTree.TakeWhile(span => span.Attributes["class"].Value.Contains("no")).Count();
                    var slots = visualTree.Count;

                    return new Talent
                    {
                        Header = header,
                        Image = image,
                        Position = position,
                        Slots = slots
                    };
                })
                .ToList();
        }

        private async Task<List<Build>> GetBuilds(Hero hero)
        {
            var link = $"{HeroUrl}/{hero.InternalName}-build-guide";

            // Load the page
            var document = new HtmlDocument();
            var response = await _client.GetAsync(link);
            document.LoadHtml(await response.Content.ReadAsStringAsync());

            // Get the subsection where the talents are
            var tldr = document.DocumentNode
                .Descendants("div")
                .First(div => div.Attributes["class"]?.Value.Equals("heroes_tldr") == true);
            
            // Get the generic nodes and get the build objects
            var titleNodes =
               tldr.Descendants("h4").Any(div => div.Attributes["class"]?.Value.Equals("toc_no_parsing") == true)
                   ? tldr.Descendants("h4").Where(h4 => h4.Attributes["class"]?.Value.Equals("toc_no_parsing") == true)
                   : tldr.Descendants("p");
            var talentsNodes = tldr
                .Descendants("div")
                .Where(h4 => h4.Attributes["class"]?.Value.Equals("heroes_tldr_talents") == true);
            var builds = titleNodes.Zip(talentsNodes, ParseBuild).ToList();

            // Download images
            foreach (var build in builds)
                foreach (var talent in build.Talents)
                    await DownloadTalentImage(talent);

            return builds;
        }
    }
}
