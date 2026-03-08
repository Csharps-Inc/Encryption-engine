using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        var visited = new HashSet<string>();
        var toVisit = new Queue<string>();
        toVisit.Enqueue("https://www.google.com");
        int maxPages = 50;
        
        // Create a list to store ALL found URLs
        List<string> allFoundUrls = new List<string>();

        while (toVisit.Count > 0 && visited.Count < maxPages)
        {
            string url = toVisit.Dequeue();
            if (!visited.Add(url)) continue;
            
            Console.WriteLine($"🌍 Visiting: {url}");
            
            try 
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);
                
                var links = doc.DocumentNode.SelectNodes("//a[@href]");
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        string href = link.GetAttributeValue("href", "");
                        try 
                        { 
                            Uri baseUri = new Uri(url);
                            Uri fullUri = new Uri(baseUri, href);
                            string fullUrl = fullUri.ToString();
                            
                            if (fullUrl.StartsWith("http") && !visited.Contains(fullUrl) && !toVisit.Contains(fullUrl))
                            {
                                toVisit.Enqueue(fullUrl);
                                allFoundUrls.Add(fullUrl); // Add to our list
                                Console.WriteLine($"  ➕ Added: {fullUrl}");
                            }
                        } 
                        catch { }
                    }
                }
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine($"  ❌ Error: {ex.Message}");
            }
            
            // Save after each page (in case it crashes)
            File.WriteAllLines("found_urls.txt", allFoundUrls);
            
            Thread.Sleep(500);
        }

        // Final save with ALL data
        File.WriteAllLines("found_urls.txt", allFoundUrls);
        
        // Also save visited pages separately
        File.WriteAllLines("visited_pages.txt", visited);
        
        Console.WriteLine($"\n🎉 DONE!");
        Console.WriteLine($"Visited {visited.Count} pages");
        Console.WriteLine($"Found {allFoundUrls.Count} total URLs");
        Console.WriteLine($"\n📁 Files created:");
        Console.WriteLine($"   - found_urls.txt (ALL URLs found)");
        Console.WriteLine($"   - visited_pages.txt (pages actually crawled)");
    }
}
