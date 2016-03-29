using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FontCollection.ViewModels
{

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string FilePath = "data.json";

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [JsonIgnore]
        public IEnumerable<FontFamily> InstalledFonts { get; private set; }

        public List<FontCollection> FontCollections { get; set; } = new List<FontCollection>();

        [JsonIgnore]
        public IEnumerable<FontFamily> FilteredFontList { get; private set; }

        [JsonIgnore]
        public List<FontCollection> FontCollectionsUI
        {
            get
            {
                return new List<FontCollection>()
                {
                    new FontCollection() { Name = "ALL", },
                }.Concat(this.FontCollections)
                .ToList();
            }
        }

        [JsonIgnore]
        public List<FontCollectionBinding> CurrentFontBinding { get; private set; }

        public MainWindowViewModel()
        {
            var installedFont = new InstalledFontCollection();
            this.InstalledFonts = installedFont.Families
                .Select(q => new FontFamily(q.Name))
                .ToList();

            this.FilterFontList(null);
        }

        public void AddCollection(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Name must be entered");
            }

            foreach (var collection in this.FontCollectionsUI)
            {
                if (collection.Name.ToLower() == name.ToLower())
                {
                    throw new InvalidOperationException("Name already exist");
                }
            }

            this.FontCollections.Add(new FontCollection()
            {
                Name = name,
            });

            this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.FontCollections)));
            this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.FontCollectionsUI)));
        }

        public void DeleteCollection(FontCollection collection)
        {
            this.FontCollections.Remove(collection);

            this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.FontCollections)));
            this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.FontCollectionsUI)));
        }

        public void FilterFontList(FontCollection collection)
        {
            if (collection == null || collection.Name == "ALL")
            {
                this.FilteredFontList = this.InstalledFonts.ToList();
            }
            else
            {
                this.FilteredFontList = this.InstalledFonts
                    .Where(q => collection.FontFamilies.Contains(q.Source))
                    .ToList();
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.FilteredFontList)));
        }

        public void SelectFont(FontFamily fontFamily)
        {
            this.CurrentFontBinding = this.FontCollections.Select(q => new FontCollectionBinding()
            {
                CollectionName = q.Name,
                Binded = q.FontFamilies.Contains(fontFamily.Source),
                CollectionAndFamilyName = new KeyValuePair<string, string>(q.Name, fontFamily.FamilyNames.First().Value),
            }).ToList();

            this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.CurrentFontBinding)));
        }

        public void BindFontAndCollection(string fontFamily, string collectionName, bool bind)
        {
            var collection = this.FontCollections.First(q => q.Name == collectionName);

            if (bind)
            {
                collection.FontFamilies.Add(fontFamily);
            }
            else
            {
                collection.FontFamilies.Remove(fontFamily);
            }
        }

        public static MainWindowViewModel Load()
        {
            if (File.Exists(FilePath))
            {
                return JsonConvert.DeserializeObject<MainWindowViewModel>(
                    File.ReadAllText(FilePath, Encoding.UTF8));
            }
            else
            {
                return new MainWindowViewModel();
            }
        }

        public void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this), Encoding.UTF8);
        }

    }

    public class FontCollection
    {
        public string Name { get; set; }
        public HashSet<string> FontFamilies { get; set; } = new HashSet<string>();

        public override string ToString()
        {
            return this.Name;
        }

    }

    public class FontCollectionBinding
    {
        public string CollectionName { get; set; }
        public bool Binded { get; set; }
        public KeyValuePair<string, string> CollectionAndFamilyName { get; set; }
    }

}
