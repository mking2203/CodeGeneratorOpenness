using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorOpenness
{
    // HINWEIS: Für den generierten Code ist möglicherweise mindestens .NET Framework 4.5 oder .NET Core/Standard 2.0 erforderlich.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class MultilingualText
    {

        private MultilingualTextMultilingualTextItem[] objectListField;

        private string idField;

        private string compositionNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("MultilingualTextItem", IsNullable = false)]
        public MultilingualTextMultilingualTextItem[] ObjectList
        {
            get
            {
                return this.objectListField;
            }
            set
            {
                this.objectListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CompositionName
        {
            get
            {
                return this.compositionNameField;
            }
            set
            {
                this.compositionNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MultilingualTextMultilingualTextItem
    {

        private MultilingualTextMultilingualTextItemAttributeList attributeListField;

        private string idField;

        private string compositionNameField;

        /// <remarks/>
        public MultilingualTextMultilingualTextItemAttributeList AttributeList
        {
            get
            {
                return this.attributeListField;
            }
            set
            {
                this.attributeListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CompositionName
        {
            get
            {
                return this.compositionNameField;
            }
            set
            {
                this.compositionNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MultilingualTextMultilingualTextItemAttributeList
    {

        private string cultureField;

        private string textField;

        /// <remarks/>
        public string Culture
        {
            get
            {
                return this.cultureField;
            }
            set
            {
                this.cultureField = value;
            }
        }

        /// <remarks/>
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }
}
