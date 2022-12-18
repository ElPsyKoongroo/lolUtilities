using System.IO;
using System.Windows.Media.Imaging;
using LeagueUtilities.DTO;
using LeagueUtilities.Models;
using System.Collections.Generic;

namespace lolClientUtilities.Model;

public class ChampWithBitmap
{
    public int id { get; set; }
    public string alias { get; set; }
    public string name { get; set; }
    public string squarePortraitPath { get; set; }
    public List<string> roles { get; set; }
    public BitmapImage image { get; set; }

    public ChampWithBitmap(ChampsJSON champ, byte[] img)
    {
        id = champ.id;
        alias = champ.alias;
        name = champ.name;
        roles = champ.roles;
        image = LoadImage(img);
    }
    
    public ChampWithBitmap(Champ champ)
    {
        id = champ.id;
        alias = champ.alias;
        name = champ.name;
        roles = champ.roles;
        image = LoadImage(champ.image);
    }
    public ChampWithBitmap(ChampWithBitmap champ, BitmapImage img)
    {
        id = champ.id;
        alias = champ.alias;
        name = champ.name;
        roles = champ.roles;
        image = img;
    }

    public ChampsJSON AsChampJSON()
    {
        return new ChampsJSON()
        {
            alias = alias,
            name = name,
            id = id,
            roles  = roles,
            squarePortraitPath = squarePortraitPath
        };
    }
    
    private static BitmapImage LoadImage(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0) return null;
        var image = new BitmapImage();
        using (var mem = new MemoryStream(imageData))
        {
            mem.Position = 0;
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = null;
            image.StreamSource = mem;
            image.EndInit();
        }
        image.Freeze();
        return image;
    }
}

