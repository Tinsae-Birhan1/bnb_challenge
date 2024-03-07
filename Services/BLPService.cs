using HtmlAgilityPack;
using System.Net;

namespace BLBServices;
public class BLPService
{
    static async Task<string> ScrapBLPHolding(string address)
    {
        string url = $"https://bscscan.com/address/{address}";

        // Scraping the data
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:123.0) Gecko/20100101 Firefox/123.0");
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseBody);

            string bullHref = "/token/0xfe1d7f7a8f0bda6e415593a2e4f82c64b446d404?a=0xe9e7CEA3DedcA5984780Bafc599bD69ADd087D56";
            HtmlNode spanNode = doc.DocumentNode.SelectSingleNode($"//a[@href='{bullHref}']//span[@class='text-muted']");
            if (spanNode == null)
            {
                return "0";
            }
            string res = spanNode.InnerText.Trim();

            res = res.Replace("BLP", "");
            res = res.Replace(" ", "");
            res = res.Replace(",", "");
            return res;
        }
    }

    private static async Task<decimal> GetTotalNonCirculating()
    {
        // loop through the address and get the total non circulating supply
        string[] addresses = new string[]
        {
          "0x000000000000000000000000000000000000dEaD",
          "0xe9e7CEA3DedcA5984780Bafc599bD69ADd087D56",
          "0xfE1d7f7a8f0bdA6E415593a2e4F82c64b446d404",
          "0x71F36803139caC2796Db65F373Fb7f3ee0bf3bF9",
          "0x62D6d26F86F2C1fBb65c0566Dd6545ae3F9A63F1",
          "0x83a7152317DCfd08Be0F673Ab614261b4D1e1622",
          "0x5A749B82a55f7d2aCEc1d71011442E221f55A537",
          "0x9eBbBE47def2F776D6d2244AcB093AB2fD1B2C2A",
          "0xcdD80c6F317898a8aAf0ec7A664655E25E4833a2",
          "0x456F20bb4d89d10A924CE81b7f0C89D5711CE05B",
        };

        decimal total = 0;
        foreach (var address in addresses)
        {
            string res = await ScrapBLPHolding(address);
            total += decimal.Parse(res);
        }

        return total;
    }

    // get total supply
    private static async Task<decimal> ScrapeTotalSupply()
    {
        string url = "https://coinmarketcap.com/currencies/bullperks/";

        // Scraping the data
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:123.0) Gecko/20100101 Firefox/123.0");
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseBody);

            string selector = "dl.sc-f70bb44c-0 > div:nth-child(5) > div:nth-child(1) > dd:nth-child(2)";
            HtmlNode node = doc.DocumentNode.SelectSingleNode(selector);

            if (node == null)
            {
                return 0;
            }

            string res = node.InnerText.Trim();
            res = res.Replace("BLP", "");
            res = res.Replace(" ", "");
            res = res.Replace(",", "");

            return decimal.Parse(res);
        }
    }

    private static async Task<decimal> ScrapCirculatingSupply()
    {

        string url = "https://coinmarketcap.com/currencies/bullperks/";

        // Scraping the data
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:123.0) Gecko/20100101 Firefox/123.0");
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseBody);

            string selector = "dl.sc-f70bb44c-0 > div:nth-child(4) > div:nth-child(1) > dd:nth-child(2)";

            HtmlNode node = doc.DocumentNode.SelectSingleNode(selector);

            if (node == null)
            {
                return 0;
            }

            string res = node.InnerText.Trim();
            res = res.Replace("BLP", "");
            res = res.Replace(" ", "");
            res = res.Replace(",", "");

            return decimal.Parse(res);
        }
    }

    private static async Task<decimal> GetCirculatingSupply()
    {
        decimal totalSupply = await ScrapeTotalSupply();
        decimal nonCirculatingSupply = await GetTotalNonCirculating();

        return totalSupply - nonCirculatingSupply;
    }



    // get total and circulating token
    public static async Task<(decimal, decimal)> GetTotalAndCirculatingSupply()
    {

        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("https://bnbflaskscrape-production.up.railway.app/");

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            string[] parts = responseBody.Split(',');

            string circulatingString = parts[0].Split(':')[1].Trim();
            string supplyString = parts[1].Split(':')[1].Trim();

            int circulating = int.Parse(circulatingString);
            int supply = int.Parse(supplyString);

            client.Dispose();
            return (circulating, supply);
        }
        else
        {
            return (0, 0);
        }

    }
}
