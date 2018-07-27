using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Angular
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DropdownAttribute : Attribute
    {
        /// <summary>
        /// 對應 CodeFile.ItemType
        /// </summary>
        public string CodeMap { get; set; }

        /// <summary>
        /// Optional：對應 Parent 的 CodeFile.ItemType，主要目的在於取出數值
        /// 參考用途： CodeServiceHelper.GetCodeValue()
        /// </summary>
        public string ParentCodeMap { get; set; }

        private string _parentFieldName;

        /// <summary>
        /// Optional：主要重點在於對應 Parent 的欄位名稱，如果 ParentCodeMap 有值，但沒有設定 FieldName 就會以 ParentCodeMap 回傳
        /// 參考用途： CodeServiceHelper.GetCodeValue()
        /// </summary>
        public string ParentFieldName
        {
            get
            {
                return (string.IsNullOrEmpty(ParentCodeMap)) ? string.Empty : _parentFieldName ?? ParentCodeMap;
            }
            set
            {
                _parentFieldName = value;
            }
        }

        public DropdownAttribute(string code)
        {
            this.CodeMap = code;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DropdownForAttribute : Attribute
    {
        /// <summary>
        /// 對應 CodeFile.ItemType
        /// </summary>
        public string CodeMap { get; set; }

        /// <summary>
        /// 對應 Child 的 CodeFile.ItemType
        /// </summary>
        public string CodeMapFor { get; set; }

        /// <summary>
        /// 對應 Class 宣告中的 ChildField name
        /// </summary>
        public string ChildFieldName { get; set; }

        public DropdownForAttribute(string code)
        {
            this.CodeMap = code;
        }
    }
}
