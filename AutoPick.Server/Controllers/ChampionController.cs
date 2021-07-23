namespace AutoPick.Server.Controllers
{
    using System.IO;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    public class ChampionController : ControllerBase
    {
        private readonly IChampionRetriever _championRetriever;

        public ChampionController(IChampionRetriever championRetriever)
        {
            _championRetriever = championRetriever;
        }

        [HttpGet("/champions/checksum")]
        public IActionResult GetChampionsChecksum()
        {
            if (!_championRetriever.IsLoaded)
            {
                return NotFound();
            }

            return Ok(new MemoryStream(_championRetriever.Checksum));
        }

        [HttpGet("/champions")]
        public IActionResult GetChampions()
        {
            if (!_championRetriever.IsLoaded)
            {
                return NotFound();
            }

            return Ok(_championRetriever.ChampionNames);
        }

        [HttpGet("/champions/{championName}/image")]
        public IActionResult GetImage(string championName)
        {
            if (!_championRetriever.IsLoaded)
            {
                return NotFound();
            }

            return Ok(new MemoryStream(_championRetriever.ChampionImage(championName)));
        }
    }
}