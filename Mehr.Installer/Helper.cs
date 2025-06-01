using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using System.Drawing;
using Mehr.Installer.Properties;

namespace Mehr.Installer
{
    internal sealed class EditorHelpers
    {
        public EditorHelpers()
        {

        }
        public static RepositoryItemImageComboBox CreateLanguagePrefixImageComboBox(RepositoryItemImageComboBox edit = null, RepositoryItemCollection collection = null)
        {
            RepositoryItemImageComboBox ret = CreateEnumImageComboBox<LanguagePrefix>(edit, collection);
            ret.SmallImages = CreatePersonPrefixImageCollection();
            if (edit == null)
                ret.GlyphAlignment = HorzAlignment.Center;
            return ret;
        }
        public static RepositoryItemImageComboBox CreateEnumImageComboBox<TEnum>(RepositoryItemImageComboBox edit = null, RepositoryItemCollection collection = null, Converter<TEnum, string> displayTextConverter = null)
        {
            return CreatEdit<RepositoryItemImageComboBox>(edit, collection, e => e.Items.AddEnum<TEnum>(displayTextConverter));
        }

        public static TEdit CreatEdit<TEdit>(TEdit edit = null, RepositoryItemCollection collection = null, Action<TEdit> initialize = null) where TEdit : RepositoryItem, new()
        {
            edit = edit ?? new TEdit();
            if (collection != null)
                collection.Add(edit);
            if (initialize != null)
                initialize(edit);
            return edit;
        }
        private static ImageCollection CreatePersonPrefixImageCollection()
        {
            ImageCollection ret = new ImageCollection();
            ret.ImageSize = new Size(16, 16);
            ret.AddImage(Resources.English);
            ret.AddImage(Resources.Persian);
            ret.AddImage(Resources.Arabic);
            return ret;
        }
    }
    public enum LanguagePrefix
    {
        English = 0,
        Persian = 1,
        Arabic = 2
    }
}
