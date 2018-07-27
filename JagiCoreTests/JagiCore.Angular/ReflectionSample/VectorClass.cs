using JagiCore.Angular;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace JagiCoreTests
{
    public class VectorClass
    {
        [Range(minimum: 10, maximum: 20)]
        public int Id { get; set; }
        [StringLength(30), Required, Display(Name = "名字", Description = "此為必須輸入的欄位，且不可以超過30個字", Prompt = "輸入客戶名稱...")]
        public string FirstName { get; set; }
        [StringLength(10, MinimumLength = 5), Display(Name = "姓氏", Description = "輸入字數限制在 5 -10 個字")]
        public string LastName { get; set; }
        [Required, StringLength(4), DropdownFor("County", CodeMapFor = "Hospital", ChildFieldName = "ChildCode")]
        public string Code { get; set; }
        [Dropdown("Hospital")]
        public string ChildCode { get; set; }
        public DateTime Date { get; set; }
    }

    public class VectorClass2
    {
        [HiddenInput]
        public int Id { get; set; }
        public bool Check { get; set; }
        public RadioOption Radio { get; set; }
        public RadioStart1Option PreferRadioStyle { get; set; }
    }

    public enum RadioOption
    {
        Option1,
        Option2,
        Option3
    }

    /// <summary>
    /// 建議的方式（如果有需要空值的話），透過第一個設定 1，讓 0 變成預設值
    /// （資料庫設定為 short int）
    /// </summary>
    public enum RadioStart1Option
    {
        Option1 = 1,
        Option2 = 2,
        Option3 = 3
    }
}